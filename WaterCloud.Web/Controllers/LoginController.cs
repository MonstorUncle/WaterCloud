/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/

using System;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Service;
using WaterCloud.Service.SystemSecurity;
using WaterCloud.Code;
using WaterCloud.Domain.SystemSecurity;
using System.Threading.Tasks;
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Domain.SystemOrganize;
using Chloe;

namespace WaterCloud.Web.Controllers
{
    public class LoginController : Controller
    {
        public FilterIPService _filterIPService { get; set; }
        public UserService _userService { get; set; }
        public LogService _logService { get; set; }
        public SystemSetService _setService { get; set; }
        public IDbContext _context { get; set; }
        [HttpGet]
        public virtual async Task<ActionResult> Index()
        {
            //登录页获取logo和项目名称
            try
            {
                var systemset = await _setService.GetFormByHost("");
                if (GlobalContext.SystemConfig.Demo)
                {
                    ViewBag.UserName = GlobalContext.SystemConfig.SysemUserCode;
                    ViewBag.Password = GlobalContext.SystemConfig.SysemUserPwd;
                }
                ViewBag.ProjectName = systemset.F_ProjectName;
                ViewBag.LogoIcon = ".." + systemset.F_Logo;
                return View();
            }
            catch (Exception)
            {
                ViewBag.ProjectName = "水之云信息系统";
                ViewBag.LogoIcon = "../icon/favicon.ico";
                return View();
            }

        }
        /// <summary>
        /// 验证码获取（此接口已弃用）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCodeHelper().GetVerifyCode(), @"image/Gif");
        }
        [HttpGet]
        public async Task<ActionResult> OutLogin()
        {
            await _logService.WriteDbLog(new LogEntity
            {
                F_ModuleName = "系统登录",
                F_Type = DbLogType.Exit.ToString(),
                F_Account = _setService.currentuser.UserCode,
                F_NickName = _setService.currentuser.UserName,
                F_Result = true,
                F_Description = "安全退出系统",
            });
            await OperatorProvider.Provider.EmptyCurrent("pc_");
            return Redirect("/Login/Index");
        }
        /// <summary>
        /// 验证登录状态请求接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CheckLoginState()
        {
            try
            {
                if (_setService.currentuser.UserId == null)
                {
                    return Content(new AlwaysResult { state = ResultType.error.ToString() }.ToJson());
                }
                //登录检测      
                if ((await OperatorProvider.Provider.IsOnLine("pc_")).stateCode<=0)
                {
                    await OperatorProvider.Provider.EmptyCurrent("pc_");
                    return Content(new AlwaysResult { state = ResultType.error.ToString() }.ToJson());
                }
                else
                {
                    return Content(new AlwaysResult { state = ResultType.success.ToString() }.ToJson());
                }
            }
            catch (Exception)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString() }.ToJson());
            }

        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户</param>
        /// <param name="password">密码</param>
        /// <param name="localurl">域名</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CheckLogin(string username, string password,string localurl)
        {
            //根据域名判断租户
            LogEntity logEntity = new LogEntity();
            logEntity.F_ModuleName ="系统登录";
            logEntity.F_Type = DbLogType.Login.ToString();
            if (GlobalContext.SystemConfig.Debug)
            {
                localurl = "";
            }
            try
            {
                if (!await CheckIP())
                {
                    throw new Exception("IP受限");
                }
                UserEntity userEntity =await _userService.CheckLogin(username, password, localurl);
                OperatorModel operatorModel = new OperatorModel();
                operatorModel.UserId = userEntity.F_Id;
                operatorModel.UserCode = userEntity.F_Account;
                operatorModel.UserName = userEntity.F_RealName;
                operatorModel.CompanyId = userEntity.F_OrganizeId;
                operatorModel.DepartmentId = userEntity.F_DepartmentId;
                operatorModel.RoleId = userEntity.F_RoleId;
                operatorModel.LoginIPAddress = WebHelper.Ip;
                if (GlobalContext.SystemConfig.LocalLAN != false)
                {
                    operatorModel.LoginIPAddressName = "本地局域网";
                }
                else
                {
                    operatorModel.LoginIPAddressName = WebHelper.GetIpLocation(operatorModel.LoginIPAddress);
                }
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.DdUserId = userEntity.F_DingTalkUserId;
                operatorModel.WxOpenId = userEntity.F_WxOpenId;
                //各租户的管理员也是当前数据库的全部权限
                operatorModel.IsSystem = userEntity.F_IsAdmin.Value;
                operatorModel.IsAdmin = userEntity.F_IsAdmin.Value;
                operatorModel.IsBoss = userEntity.F_IsBoss.Value;
                operatorModel.IsLeaderInDepts = userEntity.F_IsLeaderInDepts.Value;
                operatorModel.IsSenior = userEntity.F_IsSenior.Value;
                SystemSetEntity setEntity = await _setService.GetForm(userEntity.F_OrganizeId);
                operatorModel.DbString = setEntity.F_DbString;
                operatorModel.DBProvider = setEntity.F_DBProvider;
                if (userEntity.F_Account == GlobalContext.SystemConfig.SysemUserCode)
                {
                    operatorModel.IsSystem = true;
                }
                else
                {
                    operatorModel.IsSystem = false;
                }
                //缓存保存用户信息
                await OperatorProvider.Provider.AddLoginUser(operatorModel, "","pc_");
                //防重复token
                string token = Utils.GuId();
                HttpContext.Response.Cookies.Append("pc_" + GlobalContext.SystemConfig.TokenName, token);
                await CacheHelper.Set("pc_" + GlobalContext.SystemConfig.TokenName + "_" + operatorModel.UserId + "_" + operatorModel.LoginTime, token, GlobalContext.SystemConfig.LoginExpire, true);
                logEntity.F_Account = userEntity.F_Account;
                logEntity.F_NickName = userEntity.F_RealName;
                logEntity.F_Result = true;
                logEntity.F_Description = "登录成功";
                await _logService.WriteDbLog(logEntity);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "登录成功。"}.ToJson());
            }
            catch (Exception ex)
            {
                logEntity.F_Account = username;
                logEntity.F_NickName = username;
                logEntity.F_Result = false;
                logEntity.F_Description = "登录失败，" + ex.Message;
                await _logService.WriteDbLog(logEntity);
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = ex.Message }.ToJson());
            }
        }
        private async Task<bool> CheckIP()
        {
            string ip = WebHelper.Ip;
            return await _filterIPService.CheckIP(ip);
        }
    }
}
