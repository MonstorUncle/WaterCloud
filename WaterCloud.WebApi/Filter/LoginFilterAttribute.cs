using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using WaterCloud.Code;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.SystemOrganize;

namespace WaterCloud.WebApi
{
    /// <summary>
    /// 验证token
    /// </summary>
    public class LoginFilterAttribute : ActionFilterAttribute
    {
        private readonly RoleAuthorizeService _service;
        public LoginFilterAttribute(RoleAuthorizeService service)
        {
            _service = service;
        }
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (!GlobalContext.SystemConfig.Debug)
            {
                string token = context.HttpContext.Request.Headers[GlobalContext.SystemConfig.TokenName].ParseToString();
                OperatorModel user = OperatorProvider.Provider.GetCurrent();
                var description =
                (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;

                //添加有允许匿名的Action，可以不用登录访问，如Login/Index
                //控制器整体忽略或者单独方法忽略
                var anonymous = description.ControllerTypeInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
                var methodanonymous = description.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
                if (user != null && RoleAuthorize())
                {
                    //延长过期时间
                    int LoginExpire = GlobalContext.SystemConfig.LoginExpire;
                    string cacheKeyOperator = "watercloud_operator_";// +登录者token
                    await CacheHelper.Expire(cacheKeyOperator + token, LoginExpire);
                    await CacheHelper.Expire(cacheKeyOperator + "api_" + user.UserId, LoginExpire);
                    // 根据传入的Token，添加token和客户参数
                    if (context.ActionArguments != null && context.ActionArguments.Count > 0)
                    {
                        PropertyInfo property = context.ActionArguments.FirstOrDefault().Value.GetType().GetProperty("Token");
                        if (property != null)
                        {
                            property.SetValue(context.ActionArguments.FirstOrDefault().Value, token, null);
                        }
                        switch (context.HttpContext.Request.Method.ToUpper())
                        {
                            case "GET":
                                break;

                            case "POST":
                                property = context.ActionArguments.FirstOrDefault().Value.GetType().GetProperty("CustomerId");
                                if (property != null)
                                {
                                    property.SetValue(context.ActionArguments.FirstOrDefault().Value, user.UserId, null);
                                }
                                break;
                        }
                    }
                }
                else if (anonymous == null && methodanonymous == null)
                {
                    AlwaysResult obj = new AlwaysResult();
                    obj.message = "抱歉，没有操作权限";
                    obj.state = ResultType.error.ToString();
                    context.Result = new JsonResult(obj);
                    return;
                }
            }
            var resultContext = await next();

            sw.Stop();

        }
        private bool RoleAuthorize()
        {
            try
            {
                return _service.RoleValidate().GetAwaiter().GetResult();
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                return false;
            }
        }
    }
}
