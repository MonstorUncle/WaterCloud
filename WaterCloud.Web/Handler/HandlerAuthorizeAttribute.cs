using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using WaterCloud.Code;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Service.SystemOrganize;
using System.Reflection;
/// <summary>
/// 权限验证
/// </summary>
namespace WaterCloud.Web
{
	public class HandlerAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly RoleAuthorizeService _service;
        private string _authorize { get; set; }
        public HandlerAuthorizeAttribute(RoleAuthorizeService service)
        {
            _service = service;
            _authorize = string.Empty;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //反射获取Authorize中的参数 控制器方法前加[Authorize("SystemManage:Area:Delete")]
            var description = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor;
            var methodanonymous = (AuthorizeAttribute)description.MethodInfo.GetCustomAttribute(typeof(AuthorizeAttribute));
			if (methodanonymous!=null)
			{
                _authorize = methodanonymous._authorize;
            }
            if (OperatorProvider.Provider.GetCurrent() != null&& OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                return;
            }
			if (!string.IsNullOrEmpty(_authorize)&&AuthorizeCheck(filterContext))
			{
                return;
            }
            if (!ActionAuthorize(filterContext))
            {
                OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=403" + "';if(document.all) window.event.returnValue = false;</script>");
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=403");
                return;
            }
        }
        private bool ActionAuthorize(ActionExecutingContext filterContext)
        {
            try
            {
                OperatorResult result=OperatorProvider.Provider.IsOnLine("pc_").GetAwaiter().GetResult();
                if (result.stateCode<=0)
                {

                    return false;
                }
                var action = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>().HttpContext.Request.Path;
                return _service.ActionValidate(action).GetAwaiter().GetResult();
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                return false;
            }

        }
        private bool AuthorizeCheck(ActionExecutingContext filterContext)
        {
            try
            {
                OperatorResult result = OperatorProvider.Provider.IsOnLine("pc_").GetAwaiter().GetResult();
                if (result.stateCode <= 0)
                {

                    return false;
                }
                return _service.ActionValidate(_authorize,true).GetAwaiter().GetResult();
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                return false;
            }

        }
    }
}