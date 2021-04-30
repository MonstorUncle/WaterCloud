using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.FlowManage;
using WaterCloud.Service;
using WaterCloud.Service.FlowManage;

namespace WaterCloud.Web.Areas.FlowManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-14 09:18
    /// 描 述：我的流程控制器类
    /// </summary>
    [Area("FlowManage")]
    public class FlowinstanceController :  ControllerBase
    {

        public FlowinstanceService _service {get;set;}

        /// <summary>
        /// 待处理的流程
        /// </summary>
        public ActionResult ToDoFlow()
        {
            return View();
        }

        /// <summary>
        /// 已完成的流程
        /// </summary>
        public ActionResult DoneFlow()
        {
            return View();
        }
        /// <summary>
        /// 处理界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Verification()
        {
            return View();
        }
        #region 获取数据
        /// <summary>
        /// 获取一个流程实例的操作历史记录
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> QueryHistories(string keyValue)
        {
            var data =await _service.QueryHistories(keyValue);
            return Success(data.Count, data);
        }
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<FlowinstanceEntity> pagination, string type, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination, type, keyword);
            return Content(pagination.setData(data).ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyword)
        {
            var data = await _service.GetList(keyword);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data = await _service.GetForVerification(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(FlowinstanceEntity entity, string keyValue)
        {
            try
            {
                if (string.IsNullOrEmpty(keyValue))
                {
                    await _service.CreateInstance(entity);
                }
                else
                {
                    entity.F_Id = keyValue;
                    await _service.UpdateInstance(entity);
                }
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> Verification(VerificationExtend entity)
        {
            try
            {
                await _service.Verification(entity);
                return await Success("操作成功。", "", entity.F_FlowInstanceId,DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "",entity.F_FlowInstanceId, DbLogType.Submit);
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
        #endregion
    }
}
