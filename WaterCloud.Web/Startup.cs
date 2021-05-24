using CSRedis;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaterCloud.Code;
using WaterCloud.Code.Model;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Newtonsoft.Json.Serialization;
using WaterCloud.Service.AutoJob;
using WaterCloud.DataBase;
using System.Reflection;
using System.Linq;
using WaterCloud.Service;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using Chloe.Infrastructure.Interception;

namespace WaterCloud.Web
{
	public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
            GlobalContext.LogWhenStart(env);
            GlobalContext.HostingEnvironment = env;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSession();
            //代替HttpContext.Current
            services.AddHttpContextAccessor();
            //缓存缓存选择
            if (Configuration.GetSection("SystemConfig:CacheProvider").Value!= Define.CACHEPROVIDER_REDIS)
            {
                services.AddMemoryCache();
            }
            else
            {
                //redis 注入服务
                string redisConnectiong = Configuration.GetSection("SystemConfig:RedisConnectionString").Value;
                // 多客户端 1、基础 2、操作日志
                var redisDB1 = new CSRedisClient(redisConnectiong + ",defaultDatabase=" + 0);
                BaseHelper.Initialization(redisDB1);
                var redisDB2 = new CSRedisClient(redisConnectiong + ",defaultDatabase=" + 1);
                HandleLogHelper.Initialization(redisDB2);
                services.AddSingleton(redisDB1);
                services.AddSingleton(redisDB2);
            }
            //雪花id初始化,示例IDGenerator.NextId()
            var options = new IDGeneratorOptions(ushort.Parse(Configuration.GetSection("SystemConfig:WorkId").Value));
            IDGenerator.SetIdGenerator(options);

            #region 依赖注入
            //注入数据库连接
            services.AddScoped<Chloe.IDbContext>((serviceProvider) =>
            {
                return DBContexHelper.Contex();
            });
            #region 注入 Quartz调度类
            services.AddSingleton<JobExecute>();
            //注册ISchedulerFactory的实例。
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, IOCJobFactory>();
            //是否开启后台任务
            if (Configuration.GetSection("SystemConfig:OpenQuarz").Value == "True")
            {
                services.AddHostedService<JobCenter>();
            }
            #endregion
            //注入SignalR实时通讯，默认用json传输
            services.AddSignalR(options =>
            {
                //客户端发保持连接请求到服务端最长间隔，默认30秒，改成4分钟，网页需跟着设置connection.keepAliveIntervalInMilliseconds = 12e4;即2分钟
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(4);
                //服务端发保持连接请求到客户端间隔，默认15秒，改成2分钟，网页需跟着设置connection.serverTimeoutInMilliseconds = 24e4;即4分钟
                options.KeepAliveInterval = TimeSpan.FromMinutes(2);
            });
            ////注册html解析
            //services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            ////注册特性
            //services.AddScoped<HandlerLoginAttribute>();
            //services.AddScoped<HandlerAuthorizeAttribute>();
            ////ajax不能使用注入
            ////services.AddScoped<HandlerAjaxOnlyAttribute>();
            //services.AddScoped<HandlerAdminAttribute>();
            //////定时任务（已废除）
            ////services.AddBackgroundServices();
            #endregion
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader()
                       .WithOrigins(Configuration.GetSection("SystemConfig:AllowCorsSite").Value.Split(","))
                       .AllowCredentials();
            }));
            services.AddHttpClient();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
                options.Filters.Add<ModelActionFilter>();
                options.ModelMetadataDetailsProviders.Add(new ModelBindingMetadataProvider());
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson(options =>
            {
                // 返回数据首字母不小写，CamelCasePropertyNamesContractResolver是小写
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
            services.AddControllersWithViews().AddControllersAsServices();
            //调试前端可更新
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddOptions();
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(GlobalContext.HostingEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "DataProtection"));
            GlobalContext.SystemConfig = Configuration.GetSection("SystemConfig").Get<SystemConfig>();
            GlobalContext.Services = services;
            GlobalContext.Configuration = Configuration;
            //更新数据库管理员和主系统
            try
            {
                using (var context = DBContexHelper.Contex())
                {
                    var _setService = new Service.SystemOrganize.SystemSetService(context);
                    Domain.SystemOrganize.SystemSetEntity temp = new Domain.SystemOrganize.SystemSetEntity();
                    temp.F_AdminAccount = GlobalContext.SystemConfig.SysemUserCode;
                    temp.F_AdminPassword = GlobalContext.SystemConfig.SysemUserPwd;
                    _setService.SubmitForm(temp, GlobalContext.SystemConfig.SysemMasterProject).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex);
            }
            //清理缓存
            //CacheHelper.FlushAll().GetAwaiter().GetResult();
        }
        //AutoFac注入
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var assemblys = Assembly.Load("WaterCloud.Service");//Service是继承接口的实现方法类库名称
            var baseType = typeof(IDenpendency);//IDenpendency 是一个接口（所有要实现依赖注入的借口都要继承该接口）
            builder.RegisterAssemblyTypes(assemblys).Where(m => baseType.IsAssignableFrom(m) && m != baseType)
              .InstancePerLifetimeScope()//生命周期，这里没有使用接口方式
              .PropertiesAutowired() ;//属性注入
            //Controller中使用属性注入
            var controllerBaseType = typeof(Controller);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            .PropertiesAutowired();

            ////注入redis
            //if (Configuration.GetSection("SystemConfig:CacheProvider").Value== Define.CACHEPROVIDER_REDIS)
            //{
            //    //redis 注入服务
            //    string redisConnectiong = Configuration.GetSection("SystemConfig:RedisConnectionString").Value;
            //    // 多客户端
            //    var redisDB = new CSRedisClient(redisConnectiong + ",defaultDatabase=" + 0);
            //    RedisHelper.Initialization(redisDB);
            //    builder.RegisterInstance(redisDB).SingleInstance();//生命周期只能单例
            //}
            //注册html解析
            builder.RegisterInstance(HtmlEncoder.Create(UnicodeRanges.All)).SingleInstance();
            //注册特性
            builder.RegisterType(typeof(HandlerLoginAttribute)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(HandlerAuthorizeAttribute)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(HandlerAdminAttribute)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(HandlerLockAttribute)).InstancePerLifetimeScope();
            ////注册ue编辑器
            //Config.ConfigFile = "config.json";
            //Config.noCache = true;
            //var actions = new UEditorActionCollection();
            //builder.RegisterInstance(actions).SingleInstance();
            //builder.RegisterInstance(typeof(UEditorService)).SingleInstance();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //由于.Net Core默认只会从wwwroot目录加载静态文件，其他文件夹的静态文件无法正常访问。
            //而我们希望将图片上传到网站根目录的upload文件夹下，所以需要额外在Startup.cs类的Configure方法中
            //string resource = Path.Combine(Directory.GetCurrentDirectory(), "upload");
            //if (!FileHelper.IsExistDirectory(resource))
            //{
            //    FileHelper.CreateFolder(resource);
            //}
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(resource),
            //    RequestPath = "/upload",
            //    OnPrepareResponse = ctx =>
            //    {
            //        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
            //    }
            //});
            //虚拟目录 
            //如需使用，所有URL修改，例："/Home/Index"改成'@Url.Content("~/Home/Index")'，部署访问首页必须带虚拟目录;
            //if (!string.IsNullOrEmpty(GlobalContext.SystemConfig.VirtualDirectory))
            //{
            //    app.UsePathBase(new PathString(GlobalContext.SystemConfig.VirtualDirectory)); // 让 Pathbase 中间件成为第一个处理请求的中间件， 才能正确的模拟虚拟路径
            //}
            //实时通讯跨域
            app.UseCors("CorsPolicy");
            if (WebHostEnvironment.IsDevelopment())
            {
                //打印sql
                IDbCommandInterceptor interceptor = new DbCommandInterceptor();
                DbInterception.Add(interceptor);
                GlobalContext.SystemConfig.Debug = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error?msg=404");
            }
            //文件地址Resource
            //静态资源wwwroot
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new CustomerFileExtensionContentTypeProvider(),
                OnPrepareResponse = GlobalContext.SetCacheControl
            });
            //session
            app.UseSession();
            //路径
            app.UseRouting();
            //MVC路由
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MessageHub>("/chatHub");
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Login}/{action=Index}/{id?}");
            });
            GlobalContext.ServiceProvider = app.ApplicationServices;
        }
    }
}
