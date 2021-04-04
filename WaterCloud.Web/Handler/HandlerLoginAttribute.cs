using WaterCloud.Service.SystemManage;
using WaterCloud.Code;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Serenity.Web;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using WaterCloud.Service.SystemOrganize;
using Chloe;
using WaterCloud.DataBase;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
/// <summary>
/// 登录验证
/// </summary>
namespace WaterCloud.Web
{
    public class HandlerLoginAttribute : ActionFilterAttribute
    {
        private readonly RoleAuthorizeService _service;
        public HandlerLoginAttribute(RoleAuthorizeService service)
        {
            _service = service;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var description =
                (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor;

            //添加有允许匿名的Action，可以不用登录访问，如Login/Index
            //控制器整体忽略或者单独方法忽略
            var anonymous = description.ControllerTypeInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
            var methodanonymous = description.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
            if (anonymous != null|| methodanonymous!=null)
            {
                return;
            }
            if (OperatorProvider.Provider.GetCurrent() == null)
            {
                WebHelper.WriteCookie("WaterCloud_login_error", "overdue");
                //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408" + "';if(document.all) window.event.returnValue = false;</script>");
                OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408");
                return;
            }
            //登录检测
            if (!this.LoginAuthorize(filterContext))
            {
                return;
            }
            //管理员跳过检测(管理员授权或者关闭需要清理缓存保证安全)
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                return;
            }
            //用户和角色检测
            if (!this.RoleAuthorize())
            {
                OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=403" + "';if(document.all) window.event.returnValue = false;</script>");
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
        private bool LoginAuthorize(ActionExecutingContext filterContext)
        {
            try
            {

                OperatorResult result = OperatorProvider.Provider.IsOnLine("pc_").GetAwaiter().GetResult();
                switch (result.stateCode)
                {
                    case 1:
                        return true;
                    case 0:
                        OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                        //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408" + "';if(document.all) window.event.returnValue = false;</script>");
                        filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408");
                        return false;
                    case -1:
                        OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                        //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408" + "';if(document.all) window.event.returnValue = false;</script>");
                        filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408");
                        return false;
                    case -2:
                        OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                        //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=401" + "';if(document.all) window.event.returnValue = false;</script>");
                        filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=401");
                        return false;
                    default:
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                return false;
            }

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