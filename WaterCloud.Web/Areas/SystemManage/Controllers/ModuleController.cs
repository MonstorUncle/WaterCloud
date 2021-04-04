/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Service.SystemManage;
using WaterCloud.Code;
using WaterCloud.Domain.SystemManage;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Service;
using System;
using System.Threading.Tasks;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class ModuleController : ControllerBase
    {

        public ModuleService _service { get; set; }
        public ModuleButtonService _moduleButtonService { get; set; }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeSelectJson()
        {
            var data =await _service.GetList();
            data = data.Where(a => a.F_Target == "expand"&&a.F_IsExpand==true).ToList();
            var treeList = new List<TreeSelectModel>();
            foreach (ModuleEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_FullName;
                treeModel.parentId = item.F_ParentId;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeGridJson(string keyword)
        {
            var data =await _service.GetLookList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_FullName.Contains(keyword));
            }
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetSelectJson()
        {
            var data = await _service.GetList();
            data = data.Where(a => a.F_Target == "expand" && a.F_IsExpand == true).ToList();
            var treeList = new List<TreeSelectModel>();
            foreach (ModuleEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_EnCode;
                treeModel.parentId = item.F_ParentId;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetSelectMunuJson(string keyword)
        {
            var data = (await _service.GetList()).Where(a => a.F_Target=="iframe").ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(a => a.F_FullName.Contains(keyword)).ToList();
            }
            List<object> list = new List<object>();
            foreach (var item in data)
            {
                list.Add(new { id = item.F_Id, text = item.F_FullName });
            }
            return Content(list.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetSelectMunuBesidesJson(string keyword)
        {
            var data = await _service.GetBesidesList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(a => a.F_FullName.Contains(keyword)).ToList();
            }
            List<object> list = new List<object>();
            foreach (var item in data)
            {
                list.Add(new { id = item.F_Id, text = item.F_FullName });
            }
            return Content(list.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data =await _service.GetLookForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAdminAttribute))]
        public async Task<ActionResult> SubmitForm(ModuleEntity moduleEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                moduleEntity.F_DeleteMark = false;
                moduleEntity.F_AllowEdit = false;
                moduleEntity.F_AllowDelete = false;
                moduleEntity.F_IsPublic = false;
            }
            else
            {
                if (keyValue==moduleEntity.F_ParentId)
                {
                    throw new Exception("父级不能是自身");
                }
                //前端传值为null，更新的时候null不更新
                if (moduleEntity.F_Icon==null)
                {
                    moduleEntity.F_Icon = "";
                }
            }
            try
            {
                if (moduleEntity.F_ParentId == "0")
                {
                    moduleEntity.F_Layers = 1;
                }
                else
                {
                    moduleEntity.F_Layers =(await _service.GetForm(moduleEntity.F_ParentId)).F_Layers + 1;
                }
                if (!string.IsNullOrEmpty(moduleEntity.F_UrlAddress))
                {
                    var templist = await _service.GetList();
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        templist = templist.Where(a => a.F_Id != keyValue).ToList();
                    }
                    if(templist.Find(a=>a.F_UrlAddress==moduleEntity.F_UrlAddress)!=null)
                    throw new Exception("菜单地址不能重复！");
                }
                else
                {
                    moduleEntity.F_UrlAddress = null;
                }
                await _service.SubmitForm(moduleEntity, keyValue);
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
        [ServiceFilter(typeof(HandlerAdminAttribute))]
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
    }
}
