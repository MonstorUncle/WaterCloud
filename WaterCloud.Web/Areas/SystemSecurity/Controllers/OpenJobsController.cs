using WaterCloud.Service.SystemSecurity;
using WaterCloud.Code;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service;
using System.Linq;

namespace WaterCloud.Web.Areas.SystemSecurity.Controllers
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [Area("SystemSecurity")]
    public class OpenJobsController : ControllerBase
    {

        public OpenJobsService _service { get; set; }

        //获取详情
        [HttpGet]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data = await _service.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(OpenJobEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_EnabledMark = false;
                entity.F_DeleteMark = false;
            }
            else
            {
                entity.F_EnabledMark = null;
            }
            try
            {
                await _service.SubmitForm(entity, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                await _service.DeleteForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }

        /// <summary>
        /// 获取本地可执行的任务列表
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> QueryLocalHandlers()
        {
            var data = _service.QueryLocalHandlers();
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(Pagination pagination, string keyword)
        {
            pagination.order = "desc";
            pagination.sort = "F_EnabledMark";
            //导出全部页使用
            if (pagination.rows == 0 && pagination.page == 0)
            {
                pagination.rows = 99999999;
                pagination.page = 1;
            }
            var data = await _service.GetLookList(pagination, keyword);
            return Success(pagination.records, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetLogJson(string keyValue, string keyword)
        {
            var data = await _service.GetLogList(keyValue);
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(a => a.F_Description.Contains(keyword)).ToList();
            }
            return Success(data.Count, data);
        }
        /// <summary>
        /// 改变任务状态，启动/停止
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> ChangeStatus(string keyValue, int status)
        {
            try
            {
                await _service.ChangeJobStatus(keyValue, status);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> DoNow(string keyValue)
        {
            try
            {
                await _service.DoNow(keyValue);
                return await Success("操作成功。", "", null, null);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", null);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> DeleteLogForm(string keyValue)
        {
            try
            {
                await _service.DeleteLogForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }
    }
}
