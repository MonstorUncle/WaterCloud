using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-10 08:49
    /// 描 述：流程设计控制器类
    /// </summary>
    [Area("SystemManage")]
    public class FlowschemeController :  ControllerBase
    {

        public FlowschemeService _service {get;set;}
        public FormService _formService { get; set; }
        [HttpGet]
        public virtual ActionResult NodeInfo()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult LineInfo()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult AreaInfo()
        {
            return View();
        }
        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeGridJson(string keyword)
        {
            var data = await _service.GetLookList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_SchemeCode.Contains(keyword)|| t.F_SchemeName.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (var item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                treeModel.id = item.F_Id;
                treeModel.title = item.F_SchemeName;
                treeModel.parentId = "0";
                treeModel.self = item;
                treeList.Add(treeModel);
            }
            return DTreeResult(treeList.TreeList());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(string ItemId, string keyword)
        {
            var data = await _service.GetLookList(ItemId, keyword);
            return Success(data.Count, data);
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
            var data = await _service.GetFormExtend(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(FlowschemeEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.F_OrganizeId = (await _formService.GetForm(entity.F_FrmId)).F_OrganizeId;
                entity.F_CreatorUserName = _service.currentuser.UserName;
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
        #endregion
    }
}
