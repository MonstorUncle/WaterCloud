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
/// 防重复锁
/// </summary>
namespace WaterCloud.Web
{
    public class HandlerLockAttribute : ActionFilterAttribute
    {
        public HandlerLockAttribute()
        {
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (OperatorProvider.Provider.GetCurrent() == null)
            {
                WebHelper.WriteCookie("WaterCloud_login_error", "overdue");
                //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408" + "';if(document.all) window.event.returnValue = false;</script>");
                OperatorProvider.Provider.EmptyCurrent("pc_").GetAwaiter().GetResult();
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=408");
                return;
            }
            else
            {
                string token = filterContext.HttpContext.Request.Cookies["pc_" + GlobalContext.SystemConfig.TokenName];
                string cacheToken = CacheHelper.Get<string>("pc_" + GlobalContext.SystemConfig.TokenName + "_" + OperatorProvider.Provider.GetCurrent().UserId + "_" + OperatorProvider.Provider.GetCurrent().LoginTime).GetAwaiter().GetResult();
                if (string.IsNullOrWhiteSpace(token))
                {
                    filterContext.Result = new JsonResult(new AlwaysResult { state = ResultType.error.ToString(), message = "token不能空" });
                    return;
                }
                if (string.IsNullOrWhiteSpace(cacheToken))
                {
                    filterContext.Result = new JsonResult(new AlwaysResult { state = ResultType.error.ToString(), message = "token不能空" });
                    return;
                }
                if (token != cacheToken)
                {
                    filterContext.Result = new JsonResult(new AlwaysResult { state = ResultType.error.ToString(), message = "请求异常" });
                    return;
                }
                //固定加锁5秒
                bool result = CacheHelper.SetNx(token, token, 5).GetAwaiter().GetResult();
                if (!result)
                {
                    filterContext.Result = new JsonResult(new AlwaysResult { state = ResultType.error.ToString(), message = "请求太频繁，请稍后" });
                    return;
                }
            }
            //随机值
            base.OnActionExecuting(filterContext);
        }
    }
}