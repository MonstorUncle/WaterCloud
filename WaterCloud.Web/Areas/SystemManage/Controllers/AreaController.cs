/*******************************************************************************
 * Copyright © 2018 WaterCloud 版权所有
 * Author: WaterCloud
 * Description: WaterCloud
 * Website：
*********************************************************************************/
using WaterCloud.Code;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service.SystemManage;
using WaterCloud.Service;
using System.Threading.Tasks;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class AreaController : ControllerBase
    {

        public AreaService _areaService { get; set; }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeSelectJson()
        {
            var data =await _areaService.GetList();
            //默认三级区域
            data = data.Where(a => a.F_Layers < 3).ToList();
            var treeList = new List<TreeSelectModel>();
            foreach (AreaEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_FullName;
                treeModel.parentId = item.F_ParentId;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetSelectJson(string keyValue)
        {
            var data = await _areaService.GetList();
            data = data.Where(a => a.F_ParentId== keyValue).ToList();
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeGridJson(string keyword)
        {
            var data =await _areaService.GetLookList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_FullName.Contains(keyword));
            }
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyValue, string keyword)
        {
            var data = await _areaService.GetLookList();
            var result = new List<AreaEntity>();
            if (string.IsNullOrEmpty(keyValue))
            {
                keyValue = "0";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                result = data.TreeWhere(t => t.F_FullName.Contains(keyword) || t.F_EnCode.Contains(keyword));
            }
            else
            {
                result = data;
            }
            result = result.Where(t => t.F_ParentId == keyValue).ToList();
            if (result.Count==0)
            {
                result= data.Where(t => t.F_ParentId == keyValue).ToList();
            }
            foreach (var item in result)
            {
                item.haveChild = data.Where(a => a.F_ParentId == item.F_Id).Count() > 0 ? true : false;
            }
            return Success(data.Count, result);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data =await _areaService.GetLookForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(AreaEntity areaEntity, string keyValue)
        {
            if (areaEntity.F_ParentId=="0")
            {
                areaEntity.F_Layers = 1;
            }
            else
            {
                areaEntity.F_Layers =(await _areaService.GetForm(areaEntity.F_ParentId)).F_Layers + 1;
            }
            try
            {
                await _areaService.SubmitForm(areaEntity, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [Authorize("SystemManage:Area:Delete")]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                await _areaService.DeleteForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }
    }
}
