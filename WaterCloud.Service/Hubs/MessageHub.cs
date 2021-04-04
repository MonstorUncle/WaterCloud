using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Domain.InfoManage;
using WaterCloud.Service.InfoManage;
using WaterCloud.Service.SystemOrganize;

namespace WaterCloud.Service
{
    public class MessageHub : Hub
    {
        private string cacheKey = "watercloud_hubuserinfo_";
        private readonly UserService _service;
        private readonly MessageService _msgService;
        private string cacheKeyOperator = "watercloud_operator_";// +登录者tokens
        public MessageHub(UserService service, MessageService msgService)
        {
            _service = service;
            _msgService = msgService;
        }
        // <summary>
        /// 客户端登录到服务器
        /// </summary>
        /// <param name="token"></param>
        public async Task SendLogin(string token)
        {
            var user = _service.currentuser;
            if (user == null || user.UserId == null)
            {
                user = CacheHelper.Get<OperatorModel>(cacheKeyOperator + token).GetAwaiter().GetResult();
            }
            if (user != null && user.CompanyId != null)
            {
                //一个公司一个分组
                await Groups.AddToGroupAsync(Context.ConnectionId, user.CompanyId);
                //将用户信息存进缓存
                var list = await CacheHelper.Get<List<string>>(cacheKey + user.UserId);
                if (list == null)
                {
                    list = new List<string>();
                }
                list.Add(Context.ConnectionId);
                await CacheHelper.Set(cacheKey + Context.ConnectionId, user.UserId);
                await CacheHelper.Set(cacheKey + user.UserId, list);
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="reUserId">收消息的人员Id</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public async Task SendMessage(string reUserId, string message)
        {
            if (string.IsNullOrEmpty(reUserId))
            {
                return;
            }
            else
            {
                MessageEntity msg = new MessageEntity();
                msg.F_EnabledMark = true;
                msg.F_MessageType = 1;
                msg.F_CreatorUserName = _service.currentuser.UserName;
                msg.F_MessageInfo = message;
                msg.F_ToUserId = reUserId;
                msg.F_ClickRead = true;
                await _msgService.SubmitForm(msg);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //删除缓存连接
            var userId = await CacheHelper.Get<string>(cacheKey + Context.ConnectionId);
            if (!string.IsNullOrEmpty(userId))
            {
                //将用户信息存进缓存
                var list = await CacheHelper.Get<List<string>>(cacheKey + userId);
                if (list != null)
                {
                    list.Remove(Context.ConnectionId);
                    if (list.Count == 0)
                    {
                        await CacheHelper.Remove(cacheKey + userId);
                    }
                    else
                    {
                        await CacheHelper.Set(cacheKey + userId, list);
                    }
                }
                await CacheHelper.Remove(cacheKey + Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
