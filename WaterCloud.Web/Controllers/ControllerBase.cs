using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Service;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Web
{
    [ServiceFilter(typeof(HandlerLoginAttribute))]
    public abstract class ControllerBase : Controller
    {
        public LogService _logService { get; set; }
        /// <summary>
        /// 演示模式过滤
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string action = context.RouteData.Values["Action"].ParseToString();
            OperatorModel user = OperatorProvider.Provider.GetCurrent();

            if (GlobalContext.SystemConfig.Demo)
            {
                if (context.HttpContext.Request.Method.ToUpper() == "POST")
                {
                    string[] allowAction = new string[] { "LoginJson", "ExportUserJson", "CodePreviewJson" };
                    if (!allowAction.Select(p => p.ToUpper()).Contains(action.ToUpper()))
                    {

                        string Message = "演示模式，不允许操作";
                        context.Result = new JsonResult(new AlwaysResult
                        {
                            state = ResultType.error.ToString(),
                            message = Message

                        });
                        return;
                    }
                }
            }
            var resultContext = await next();
            sw.Stop();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
        [HttpGet]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public virtual ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public virtual ActionResult Form()
        {
            return View();
        }
        [HttpGet]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public virtual ActionResult Details()
        {
            return View();
        }
        protected virtual async Task<ActionResult> Success(string message, string className = "", object keyValue = null, DbLogType? logType = null)
        {
            className = string.IsNullOrEmpty(className) ? ReflectionHelper.GetClassName() : className;
            await _logService.WriteLog(message, className, keyValue != null && keyValue.ToString() != "0" ? keyValue .ToString():"", logType);
            return Content(new AlwaysResult { state = ResultType.success.ToString(), message = message }.ToJson());
        }
        protected virtual ActionResult Success(string message)
        {
            return Content(new AlwaysResult { state = ResultType.success.ToString(), message = message }.ToJson());
        }
        protected virtual ActionResult Success<T>(string message, T data)
        {
            return Content(new AlwaysResult<T> { state = ResultType.success.ToString(), message = message, data = data }.ToJson());
        }
        protected virtual ActionResult Success<T>(int total, T data)
        {
            return Content(new AlwaysResult<T> { state = 0, message = "", count = total, data = data }.ToJson());
        }
        protected virtual ActionResult DTreeResult(object data)
        {
            return Content(new DTreeResult { status = new StatusInfo { code = 200, message = "操作成功" }, data = data }.ToJson());
        }
        protected virtual async Task<ActionResult> Error(string message, string className, object keyValue = null, DbLogType? logType = null)
        {
            className = string.IsNullOrEmpty(className) ? ReflectionHelper.GetClassName() : className;
            await _logService.WriteLog(message, className, keyValue != null&&keyValue.ToString()!="0" ? keyValue.ToString() : "", logType, true);
            return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(message) }.ToJson());
        }
        protected virtual ActionResult Error(string message)
        {
            return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(message) }.ToJson());
        }
    }
}
