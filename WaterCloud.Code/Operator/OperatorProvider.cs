/*******************************************************************************
 * Copyright © 2016 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
namespace WaterCloud.Code
{
    public class OperatorProvider
    {
        //登录信息保存方式
        private string LoginProvider = GlobalContext.SystemConfig.LoginProvider;
        //是否允许一个账户在多处登录
        private bool LoginMultiple = GlobalContext.SystemConfig.LoginMultiple;
        //缓存过期时间
        private int LoginExpire = GlobalContext.SystemConfig.LoginExpire;
        public static OperatorProvider Provider
        {
            get { return new OperatorProvider(); }
        }
        //watercloud_operator_pc_ PC端登录
        //watercloud_operator_info_ 登录次数
        //
        /// <summary>
        /// 缓存操作类
        /// </summary>
        private string cacheKeyOperator = "watercloud_operator_";// +登录者token
        private string cacheKeyToken = "watercloud_token_";// +登录者token
        private string cacheKeyError = "watercloud_error_";// + Mark
        /// <summary>
        /// 秘钥
        /// </summary>
        private string LoginUserToken = "watercloud_Token";
        /// <summary>
        /// 标记登录的浏览器
        /// </summary>
        private string LoginUserMarkKey = "watercloud_Mark";
        public string GetProvider(string key)
        {
            switch (LoginProvider)
            {
                case Define.PROVIDER_COOKIE:
                    return WebHelper.GetCookie(key).ToString();
                case Define.PROVIDER_SESSION:
                    return WebHelper.GetSession(key).ToString();
                case Define.PROVIDER_WEBAPI:
                    return GetToken();
                default:
                    return GetToken();
            }
        }
        public void SetProvider(string key, string value)
        {
            switch (LoginProvider)
            {
                case Define.PROVIDER_COOKIE:
                    WebHelper.WriteCookie(key, value);
                    break;
                case Define.PROVIDER_SESSION:
                    WebHelper.WriteSession(key, value);
                    break;
                case Define.PROVIDER_WEBAPI:
                    break;
                default:
                    break;
            }
        }
        public string GetToken()
        {
			try
			{
				if (GlobalContext.ServiceProvider==null)
				{
                    return null;
				}
				if (GlobalContext.ServiceProvider.GetService<IHttpContextAccessor>()==null)
				{
                    return null;
                }
                //查请求头
                string token = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext.Request.Headers[GlobalContext.SystemConfig.TokenName].ParseToString();
                if (!String.IsNullOrEmpty(token)) return token;

                //查参数
                token = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext.Request.Query[GlobalContext.SystemConfig.TokenName];
                if (!String.IsNullOrEmpty(token)) return token;

                //查cookies
                string cookie = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext.Request.Cookies[GlobalContext.SystemConfig.TokenName];
                return cookie == null ? string.Empty : cookie;

            }
            catch (Exception)
			{
                return null;
			}
        }
        public void RemoveProvider(string key)
        {
            switch (LoginProvider)
            {
                case Define.PROVIDER_COOKIE:
                    WebHelper.RemoveCookie(key);
                    break;
                case Define.PROVIDER_SESSION:
                    WebHelper.RemoveSession(key);
                    break;
                case Define.PROVIDER_WEBAPI:
                    break;
                default:
                    break;
            }
        }
        public OperatorModel GetCurrent()
        {
            OperatorModel operatorModel = new OperatorModel();
            try
            {
                string loginMark = GetProvider(LoginUserMarkKey);
                operatorModel = CacheHelper.Get<OperatorModel>(cacheKeyOperator + loginMark).GetAwaiter().GetResult();
            }
            catch
            {
                operatorModel = null;
            }
            return operatorModel;
        }
        /// <summary>
        /// 获取浏览器设配号
        /// </summary>
        /// <returns></returns>
        public string GetMark()
        {
            string cookieMark = GetProvider(LoginUserMarkKey);
            if (string.IsNullOrEmpty(cookieMark))
            {
                cookieMark = Guid.NewGuid().ToString();
                SetProvider(LoginUserMarkKey, cookieMark);
            }
            return cookieMark;
        }
        /// <summary>
        /// 登录者信息添加到缓存中
        /// </summary>
        /// <param name="userEntity">用户</param>
        /// <param name="loginMark">设备标识uid</param>
        /// <param name="facilityMark">设备类型</param>
        /// <param name="cookie">是否保存cookie，默认是</param>
        /// <returns></returns>
        public async Task<string> AddLoginUser(OperatorModel operatorModel, string loginMark, string facilityMark, bool cookie = true)
        {
            string token = Guid.NewGuid().ToString();
            try
            {
                // 填写登录信息
                operatorModel.LoginToken = token;
                //cookid登录信息更新
                if (cookie)
                {
                    string cookieMark = GetProvider(LoginUserMarkKey);
                    if (string.IsNullOrEmpty(cookieMark))
                    {
                        operatorModel.loginMark = Guid.NewGuid().ToString();
                        SetProvider(LoginUserMarkKey, operatorModel.loginMark);
                    }
                    else
                    {
                        operatorModel.loginMark = cookieMark;
                    }
                    SetProvider(LoginUserToken, token);
                }
                else
                {
                    operatorModel.loginMark = loginMark;
                }
                //redis 登录token列表更新
                Dictionary<string, string> tokenMarkList = await CacheHelper.Get<Dictionary<string, string>>(cacheKeyToken + operatorModel.UserId);
                if (tokenMarkList == null)// 此账号第一次登录
                {
                    tokenMarkList = new Dictionary<string, string>();
                    tokenMarkList.Add(operatorModel.loginMark, token);
                }
                else
                {
                    if (tokenMarkList.ContainsKey(operatorModel.loginMark))
                    {
                        tokenMarkList[operatorModel.loginMark] = token;
                    }
                    else
                    {
                        tokenMarkList.Add(operatorModel.loginMark, token);
                    }
                }

                await CacheHelper.Set(cacheKeyToken + operatorModel.UserId, tokenMarkList);
                await CacheHelper.Set(cacheKeyOperator + operatorModel.loginMark, operatorModel, LoginExpire);
                await CacheHelper.Remove(cacheKeyOperator + facilityMark + operatorModel.UserId);
                await CacheHelper.Set(cacheKeyOperator + facilityMark + operatorModel.UserId, token, LoginExpire);
                return token;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 清空当前登录信息
        /// </summary>
        /// <param name="apitoken">apitoken</param>
        /// <param name="facilityMark">设备类型</param>
        public async Task EmptyCurrent(string facilityMark)
        {
            try
            {
                string token = GetProvider(LoginUserToken);
                string loginMark = GetProvider(LoginUserMarkKey);
                await EmptyCurrent(token, facilityMark, loginMark);
                RemoveProvider(LoginUserMarkKey.Trim());
                RemoveProvider(LoginUserToken.Trim());
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 清空当前登录信息
        /// </summary>
        /// <param name="token">登录票据</param>
        /// <param name="facilityMark">登录设备</param>
        /// <param name="loginMark">登录设备标识</param>
        public async Task EmptyCurrent(string token, string facilityMark, string loginMark)
        {
            try
            {
                OperatorModel operatorInfo = await CacheHelper.Get<OperatorModel>(cacheKeyOperator + loginMark);
                if (operatorInfo != null)
                {
                    Dictionary<string, string> tokenMarkList = await CacheHelper.Get<Dictionary<string, string>>(cacheKeyToken + operatorInfo.UserId);
                    tokenMarkList.Remove(loginMark);
                    await CacheHelper.Remove(cacheKeyOperator + loginMark);
                    if (operatorInfo.LoginToken == token || LoginProvider == Define.PROVIDER_WEBAPI)
                    {
                        await CacheHelper.Remove(cacheKeyOperator + facilityMark + operatorInfo.UserId);
                    }
                    await CacheHelper.Set(cacheKeyToken + operatorInfo.UserId, tokenMarkList);
                    await CacheHelper.Remove(facilityMark + GlobalContext.SystemConfig.TokenName + "_" + operatorInfo.UserId + "_" + operatorInfo.LoginTime);
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <param name="facilityMark">登录设备</param>
        /// <param name="apitoken">apitoken</param>
        /// <returns>-1未登录,1登录成功,0登录过期,-2账号被顶</returns>
        public async Task<OperatorResult> IsOnLine(string facilityMark)
        {
            try
            {
                string token = GetProvider(LoginUserToken);
                string loginMark = GetProvider(LoginUserMarkKey);
                return await IsOnLine(token, facilityMark, loginMark);
            }
            catch (Exception)
            {
                return new OperatorResult { stateCode = -1 };
            }
        }
        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <param name="token">登录票据</param>
        /// <param name="facilityMark">登录设备</param>
        /// <param name="loginMark">登录设备标识</param>
        /// <returns>-1未登录,1登录成功,0登录过期,-2账号被顶</returns>
        public async Task<OperatorResult> IsOnLine(string token, string facilityMark, string loginMark)
        {
            OperatorResult operatorResult = new OperatorResult();
            operatorResult.stateCode = -1; // -1未登录,1登录成功,0登录过期
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginMark))
                {
                    return operatorResult;
                }
                OperatorModel operatorInfo = await CacheHelper.Get<OperatorModel>(cacheKeyOperator + loginMark);
                if (operatorInfo != null)
                {
                    Dictionary<string, string> tokenMarkList = await CacheHelper.Get<Dictionary<string, string>>(cacheKeyToken + operatorInfo.UserId);
                    if ((token == operatorInfo.LoginToken || LoginProvider == Define.PROVIDER_WEBAPI) && tokenMarkList.ContainsKey(operatorInfo.loginMark) && tokenMarkList[operatorInfo.loginMark] == operatorInfo.LoginToken)
                    {
                        ////账号被顶(排除admin)
                        if (!LoginMultiple && !operatorInfo.IsSystem && operatorInfo.LoginToken != await CacheHelper.Get<string>(cacheKeyOperator + facilityMark + operatorInfo.UserId))
                        {
                            operatorResult.stateCode = -2;
                            tokenMarkList = await CacheHelper.Get<Dictionary<string, string>>(cacheKeyToken + operatorInfo.UserId);
                            tokenMarkList.Remove(loginMark);
                            await CacheHelper.Set(cacheKeyToken + operatorInfo.UserId, tokenMarkList);
                            await CacheHelper.Remove(cacheKeyOperator + loginMark);
                        }
                        else
                        {
                            operatorResult.userInfo = operatorInfo;
                            operatorResult.stateCode = 1;
                            await CacheHelper.Expire(cacheKeyOperator + loginMark, LoginExpire);
                            await CacheHelper.Expire(cacheKeyOperator + facilityMark + operatorInfo.UserId, LoginExpire);
                            await CacheHelper.Expire(facilityMark + GlobalContext.SystemConfig.TokenName + "_" + operatorInfo.UserId + "_" + operatorInfo.LoginTime, LoginExpire);
                        }
                    }
                }
                return operatorResult;
            }
            catch (Exception)
            {
                return operatorResult;
            }
        }

        #region 登录错误次数记录
        /// <summary>
        /// 获取当前登录错误次数
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCurrentErrorNum()
        {
            int res = 0;
            try
            {
                string cookieMark = GetProvider(LoginUserMarkKey);
                if (string.IsNullOrEmpty(cookieMark))
                {
                    cookieMark = Guid.NewGuid().ToString();
                    SetProvider(LoginUserMarkKey, cookieMark);
                }
                string num = await CacheHelper.Get<string>(cacheKeyError + cookieMark);
                if (!string.IsNullOrEmpty(num))
                {
                    res = Convert.ToInt32(num);
                }
            }
            catch (Exception)
            {
            }
            return res;
        }
        /// <summary>
        /// 增加错误次数
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddCurrentErrorNum()
        {
            int res = 0;
            try
            {
                string cookieMark = GetProvider(LoginUserMarkKey);
                if (string.IsNullOrEmpty(cookieMark))
                {
                    cookieMark = Guid.NewGuid().ToString();
                    SetProvider(LoginUserMarkKey, cookieMark);
                }
                string num = await CacheHelper.Get<string>(cacheKeyError + cookieMark);
                if (!string.IsNullOrEmpty(num))
                {
                    res = Convert.ToInt32(num);
                }
                res++;
                num = res + "";
                await CacheHelper.Set(cacheKeyError + cookieMark, num, 24);
            }
            catch (Exception)
            {
            }
            return res;
        }
        /// <summary>
        /// 清除当前登录错误次数
        /// </summary>
        public async Task ClearCurrentErrorNum()
        {
            try
            {
                string cookieMark = GetProvider(LoginUserMarkKey);
                if (string.IsNullOrEmpty(cookieMark))
                {
                    cookieMark = Guid.NewGuid().ToString();
                    SetProvider(LoginUserMarkKey, cookieMark);
                }
                await CacheHelper.Remove(cacheKeyError + cookieMark);
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
