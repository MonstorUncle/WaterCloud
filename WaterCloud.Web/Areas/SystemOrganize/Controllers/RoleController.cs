/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Code;
using WaterCloud.Domain.SystemOrganize;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service;
using System;
using System.Threading.Tasks;

namespace WaterCloud.Web.Areas.SystemOrganize.Controllers
{
    [Area("SystemOrganize")]
    public class RoleController : ControllerBase
    {

        public RoleService _service { get; set; }

        [HttpGet]
        public virtual ActionResult AddForm()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyword)
        {
            var data =await _service.GetList(keyword);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetSelectJson(string keyword,string ids)
        {
            var data = await _service.GetList(keyword);
            data = data.Where(a => a.F_EnabledMark == true).ToList();
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var item in ids.Split(','))
                {
                    var temp = data.Find(a => a.F_Id == item);
                    if (temp != null)
                    {
                        temp.LAY_CHECKED = true;
                    }
                }
            }
            return Success(data.Count, data);
        }
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<RoleExtend> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination, keyword);
            return Content(pagination.setData(data).ToJson());
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
        public async Task<ActionResult> SubmitForm(RoleEntity roleEntity, string permissionbuttonIds, string permissionfieldsIds, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue)&& _service.currentuser.RoleId == keyValue)
            {
                return Error("操作失败，不能修改用户当前角色");
            }
            try
            {
                await _service.SubmitForm(roleEntity,string.IsNullOrEmpty(permissionbuttonIds) ?new string[0]: permissionbuttonIds.Split(','), string.IsNullOrEmpty(permissionfieldsIds) ? new string[0] : permissionfieldsIds.Split(','), keyValue);
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
                if (_service.currentuser.RoleId == keyValue)
                {
                    return Error("操作失败，不能删除用户当前角色");
                }
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
