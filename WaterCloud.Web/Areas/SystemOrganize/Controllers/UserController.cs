/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Code;
using WaterCloud.Domain.SystemOrganize;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Service;
using System;
using System.Threading.Tasks;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Web.Areas.SystemOrganize.Controllers
{
    [Area("SystemOrganize")]
    public class UserController : ControllerBase
    {

        public UserService _service { get; set; }
        public UserLogOnService _userLogOnService { get; set; }
        public ModuleService _moduleService { get; set; }
        public RoleService _roleService { get; set; }
        public OrganizeService _orgService { get; set; }
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<UserExtend> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_DepartmentId";
                pagination.order = "asc";
            }
            var data = await _service.GetLookList(pagination, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpGet]
        public virtual ActionResult AddForm()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyword,string ids)
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
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data =await _service.GetLookForm(keyValue);
            if (!string.IsNullOrEmpty(data.F_DepartmentId))
            {
                List<string> str = new List<string>();
                foreach (var item in data.F_DepartmentId.Split(','))
                {
                    var temp = await _orgService.GetForm(item);
                    if (temp != null)
                    {
                        str.Add(temp.F_FullName);
                    }
                }
                data.F_DepartmentName = string.Join("  ", str.ToArray());
            }
            if (!string.IsNullOrEmpty(data.F_RoleId))
            {
                List<string> str = new List<string>();
                foreach (var item in data.F_RoleId.Split(','))
                {
                    var temp = await _roleService.GetForm(item);
                    if (temp != null)
                    {
                        str.Add(temp.F_FullName);
                    }
                }
                data.F_RoleName = string.Join("  ", str.ToArray());
            }
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetUserFormJson()
        {
            var data =await _service.GetFormExtend(_service.currentuser.UserId);
            if (!string.IsNullOrEmpty(data.F_DepartmentId))
            {
                List<string> str = new List<string>();
                foreach (var item in data.F_DepartmentId.Split(','))
                {
                    var temp = await _orgService.GetForm(item);
                    if (temp != null)
                    {
                        str.Add(temp.F_FullName);
                    }
                }
                data.F_DepartmentName = string.Join("  ", str.ToArray());
            }
            if (!string.IsNullOrEmpty(data.F_RoleId))
            {
                List<string> str = new List<string>();
                foreach (var item in data.F_RoleId.Split(','))
                {
                    var temp = await _roleService.GetForm(item);
                    if (temp != null)
                    {
                        str.Add(temp.F_FullName);
                    }
                }
                data.F_RoleName = string.Join("  ", str.ToArray());
            }
            return Content(data.ToJson("yyyy-MM-dd"));
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerLockAttribute))]
        public async Task<ActionResult> SubmitUserForm(string F_Account,string F_RealName, bool F_Gender,DateTime F_Birthday,string F_MobilePhone,string F_Email,string F_Description)
        {
            try
            {
                var userEntity = new UserEntity();
                userEntity.F_Account = F_Account;
                userEntity.F_RealName = F_RealName;
                userEntity.F_Gender = F_Gender;
                userEntity.F_Birthday = F_Birthday;
                userEntity.F_MobilePhone = F_MobilePhone;
                userEntity.F_Email = F_Email;
                userEntity.F_Description = F_Description;
                userEntity.F_Id = _service.currentuser.UserId;
                await _service.SubmitUserForm(userEntity);
                return await Success("操作成功。", "", userEntity.F_Id);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", _service.currentuser.UserId);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                userEntity.F_IsAdmin = false;
                userEntity.F_DeleteMark = false;
                userEntity.F_IsBoss = false;
                userEntity.F_OrganizeId = _service.currentuser.CompanyId;
            }
            else
            {
                if (_service.currentuser.UserId == keyValue)
                {
                    return Error("操作失败，不能修改用户自身");
                }
            }
            try
            {
                await _service.SubmitForm(userEntity, userLogOnEntity, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
        [HttpPost]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        [HandlerAjaxOnly]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                if (_service.currentuser.UserId == keyValue)
                {
                    return Error("操作失败，不能删除用户自身");
                }
                await _service.DeleteForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }
        [HttpGet]
        public ActionResult RevisePassword()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitRevisePassword(string F_UserPassword, string keyValue)
        {
            try
            {
                await _userLogOnService.RevisePassword(F_UserPassword, keyValue);
                return await Success("重置密码成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error("重置密码失败," + ex.Message, "", keyValue);
            }
        }
        [HttpGet]
        public ActionResult ReviseSelfPassword()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitReviseSelfPassword(string F_UserPassword)
        {
            try
            {
                await _userLogOnService.ReviseSelfPassword(F_UserPassword, _service.currentuser.UserId);
                return await Success("重置密码成功。", "", _service.currentuser.UserId);
            }
            catch (Exception ex)
            {
                return await Error("重置密码失败," + ex.Message, "", _service.currentuser.UserId);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DisabledAccount(string keyValue)
        {
            try
            {
                UserEntity userEntity = new UserEntity();
                userEntity.F_Id = keyValue;
                userEntity.F_EnabledMark = false;
                if (_service.currentuser.UserId == keyValue)
                {
                    return Error("操作失败，不能修改用户自身");
                }
                await _service.UpdateForm(userEntity);
                return await Success("账户禁用成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error("账户禁用失败," + ex.Message, "", keyValue);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> EnabledAccount(string keyValue)
        {
            try
            {
                UserEntity userEntity = new UserEntity();
                userEntity.F_Id = keyValue;
                userEntity.F_EnabledMark = true;
                if (_service.currentuser.UserId == keyValue)
                {
                    return Error("操作失败，不能修改用户自身");
                }
                await _service.UpdateForm(userEntity);
                return await Success("账户启用成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error("账户启用失败,"+ex.Message, "", keyValue);
            }
        }
    }
}
