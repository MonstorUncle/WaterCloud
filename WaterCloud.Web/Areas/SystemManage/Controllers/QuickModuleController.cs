/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Service.SystemManage;
using WaterCloud.Code;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class QuickModuleController : Controller
    {
        public QuickModuleService _moduleService { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTransferJson()
        {
            var userId = _moduleService.currentuser.UserId;
            var data =await _moduleService.GetTransferList(userId);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(string permissionIds)
        {
            string[] temp = string.IsNullOrEmpty(permissionIds) ? null : permissionIds.Split(',');
            await _moduleService.SubmitForm(temp);
            return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
        }
    }
}
