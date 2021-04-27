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
using System.Text;
using System;
using WaterCloud.Domain;
using WaterCloud.Service.SystemSecurity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Service.InfoManage;

namespace WaterCloud.Web.Controllers
{
    [ServiceFilter(typeof(HandlerLoginAttribute))]
    public class ClientsDataController : Controller
    {
        /// <summary>
        /// 缓存操作类
        /// </summary>
        private string cacheKey = "watercloud_quickmoduledata_";
        private string initcacheKey = "watercloud_init_";
        private string cacheKeyOperator = "watercloud_operator_";// +登录者token
        public QuickModuleService _quickModuleService { get; set; }
        public NoticeService _noticeService { get; set; }
        public UserService _userService { get; set; }
        public ModuleService _moduleService { get; set; }
        public LogService _logService { get; set; }
        public RoleAuthorizeService _roleAuthorizeService { get; set; }
        public ItemsDataService _itemsDetailService { get; set; }
        public ItemsTypeService _itemsService { get; set; }
        public SystemSetService _setService { get; set; }
        public MessageService _msgService { get; set; }
        /// <summary>
        /// 初始数据加载请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetClientsDataJson()
        {
            var data = new
            {
                dataItems =await this.GetDataItemList(),
                authorizeButton = await this.GetMenuButtonListNew(),
                moduleFields = await this.GetMenuFields(),
                authorizeFields = await this.GetMenuFieldsListNew(),
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 清空缓存请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ClearCache()
        {
            try
            {
                if (_setService.currentuser.UserId != GlobalContext.SystemConfig.SysemUserId)
                {
                    return Content(new { code = 0, msg = "此功能需要管理员权限" }.ToJson());
                }
                await CacheHelper.FlushAll();
                await OperatorProvider.Provider.EmptyCurrent("pc_");
                return Content(new { code = 1, msg = "服务端清理缓存成功" }.ToJson());
            }
            catch (Exception)
            {
                return Content(new { code = 0, msg = "此功能需要管理员权限" }.ToJson());
            }
        }
        /// <summary>
        /// 模块字段权限
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetMenuFields()
        {
            var roleId = _userService.currentuser.RoleId;
            if (roleId == null && _userService.currentuser.IsSystem)
            {
                roleId = "admin";
            }
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            var list= await _roleAuthorizeService.GetMenuList(roleId);
            foreach (ModuleEntity item in list.Where(a=>a.F_UrlAddress!=null))
            {
                dictionary.Add(item.F_UrlAddress, item.F_IsFields??false);
            }
            return dictionary;
        }
        /// <summary>
        /// 快捷菜单列表
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetQuickModuleList()
        {
            var currentuser = _userService.currentuser;
            if (currentuser.UserId == null)
            {
                return null;
            }
            var userId = currentuser.UserId;
            var data =await CacheHelper.Get<Dictionary<string,List<QuickModuleExtend>>>(cacheKey + "list");
            if (data==null)
            {
                data = new Dictionary<string, List<QuickModuleExtend>>();
                data.Add(userId,await _quickModuleService.GetQuickModuleList(userId));
            }
            else
            {
                if (data.ContainsKey(userId))
                {
                    data[userId] =await _quickModuleService.GetQuickModuleList(userId);
                }
                else
                {
                    data.Add(userId,await _quickModuleService.GetQuickModuleList(userId));
                }
            }
            await CacheHelper.Remove(cacheKey + "list");
            await CacheHelper.Set(cacheKey + "list", data);
            return data[userId];
        }
        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetNoticeList()
        {
            var data = (await _noticeService.GetList("")).Where(a => a.F_EnabledMark == true).OrderByDescending(a => a.F_CreatorTime).Take(6).ToList();
            return data;
        }
        /// <summary>
        /// 初始菜单列表请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetInitDataJson()
        {
            var currentuser = _userService.currentuser;
            var userId = currentuser.UserId;
            if (currentuser.UserId == null)
            {
                return Content("");
            }
            Dictionary<string, string > data =await CacheHelper.Get<Dictionary<string, string>>(initcacheKey + "list");
            if (data == null)
            {
                data =new Dictionary <string, string>();
                data.Add(userId, await this.GetMenuListNew());
            }
            else
            {
                if (data.ContainsKey(userId))
                {
                    data[userId] = await this.GetMenuListNew();
                }
                else
                {
                    data.Add(userId, await this.GetMenuListNew());
                }
            }
            await CacheHelper.Remove(initcacheKey + "list");
            await CacheHelper.Set(initcacheKey + "list",data);
            return Content(data[userId]);
        }
        /// <summary>
        /// 获取公告信息请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetNoticeInfo()
        {
            var data =await this.GetNoticeList();
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取当前用户信息请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetUserCode()
        {
            var currentuser = _userService.currentuser;
            if (currentuser.UserId==null)
            {
                return Content("");
            }
            var data =await _userService.GetFormExtend(currentuser.UserId);
            var msglist= await _msgService.GetUnReadListJson();
            data.MsgCout = msglist.Count();
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取快捷菜单请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetQuickModule()
        {
            try
            {
                var data =await this.GetQuickModuleList();
                return Content(data.ToJson());
            }
            catch (Exception)
            {
                return Content("");
            }
        }
        /// <summary>
        /// 获取数据信息接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetCoutData()
        {
            var currentuser = _userService.currentuser;
            if (currentuser.UserId == null)
            {
                return Content("");
            }
            int usercout =(await _userService.GetUserList("")).Count();
            var temp =await CacheHelper.Get<OperatorUserInfo>(cacheKeyOperator + "info_" + currentuser.UserId);
            int logincout = temp!=null&&temp.F_LogOnCount!=null? (int)temp.F_LogOnCount : 0;
            int modulecout =(await _moduleService.GetList()).Where(a => a.F_EnabledMark == true && a.F_UrlAddress != null).Count();
            int logcout = (await _logService.GetList()).Count();
            var data= new { usercout = usercout, logincout = logincout, modulecout = modulecout, logcout = logcout };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 菜单按钮信息
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetMenuListNew()
        {
            var currentuser = _userService.currentuser;
            var roleId = currentuser.RoleId;
            StringBuilder sbJson = new StringBuilder();
            InitEntity init = new InitEntity();
            init.homeInfo = new HomeInfoEntity();
            init.homeInfo.href = GlobalContext.SystemConfig.HomePage;
            init.logoInfo = new LogoInfoEntity();
            var systemset =await _setService.GetForm(currentuser.CompanyId);
            //修改主页及logo参数
            init.logoInfo.title = systemset.F_LogoCode;
            init.logoInfo.image = ".."+systemset.F_Logo;
            init.menuInfo = new List<MenuInfoEntity>();
            init.menuInfo = ToMenuJsonNew(await _roleAuthorizeService.GetMenuList(roleId), "0");
            sbJson.Append(init.ToJson());
            return sbJson.ToString() ;
        }
        /// <summary>
        /// 菜单信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<MenuInfoEntity> ToMenuJsonNew(List<ModuleEntity> data, string parentId)
        {
            List<MenuInfoEntity> list = new List<MenuInfoEntity>();
            List<ModuleEntity> entitys = data.FindAll(t => t.F_ParentId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    MenuInfoEntity munu = new MenuInfoEntity();
                    munu.title = item.F_FullName;
                    munu.icon = item.F_Icon;
                    munu.href = item.F_UrlAddress;
                    switch (item.F_Target)
                    {
                        case "iframe":
                            munu.target = "_self";
                            break;
                        case "open":
                            munu.target = "_open";
                            break;
                        case "blank":
                            munu.target = "_blank";
                            break;
                        default:
                            munu.target = "_self";
                            break;
                    }                    
                    if (data.FindAll(t => t.F_ParentId == item.F_Id).Count>0)
                    {
                        munu.child = new List<MenuInfoEntity>();
                        munu.child = ToMenuJsonNew(data, item.F_Id);
                    }
                    if (item.F_IsMenu ==true)
                    {
                        list.Add(munu);
                    }

                };
            }
            return list;
        }
        /// <summary>
        /// 字段信息
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetDataItemList()
        {
            var itemdata =await _itemsDetailService.GetList();
            Dictionary<string, object> dictionaryItem = new Dictionary<string, object>();
            var itemlist = await _itemsService.GetList();
            foreach (var item in itemlist.Where(a=>a.F_EnabledMark==true).ToList())
            {
                var dataItemList = itemdata.FindAll(t => t.F_ItemId==item.F_Id);
                Dictionary<string, string> dictionaryItemList = new Dictionary<string, string>();
                foreach (var itemList in dataItemList)
                {
                    dictionaryItemList.Add(itemList.F_ItemCode, itemList.F_ItemName);
                }
                dictionaryItem.Add(item.F_EnCode, dictionaryItemList);
            }

            return dictionaryItem;
        }
        /// <summary>
        /// 菜单按钮信息
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetMenuButtonListNew()
        {
            var currentuser = _userService.currentuser;
            var roleId = currentuser.RoleId;
            if (roleId==null&& currentuser.IsSystem)
            {
                roleId = "admin";
            }
            var rolelist = roleId.Split(',');
            Dictionary<string, Dictionary<string, List<ModuleButtonEntity>>> dictionary = await CacheHelper.Get<Dictionary<string, Dictionary<string, List<ModuleButtonEntity>>>>(initcacheKey + "modulebutton_list");
            var dictionarylist = new Dictionary<string, List<ModuleButtonEntity>>();
            if (currentuser.UserId == null)
            {
                return dictionarylist;
            }
            foreach (var roles in rolelist)
            {
                var dictionarytemp = new Dictionary<string, List<ModuleButtonEntity>>();
                var data = await _roleAuthorizeService.GetButtonList(roles);
                var dataModuleId = data.Where(a => a.F_ModuleId != null && a.F_ModuleId != "").Distinct(new ExtList<ModuleButtonEntity>("F_ModuleId"));
                foreach (ModuleButtonEntity item in dataModuleId)
                {
                    var buttonList = data.Where(t => t.F_ModuleId == item.F_ModuleId).ToList();
                    dictionarytemp.Add(item.F_ModuleId, buttonList);
                    if (dictionarylist.ContainsKey(item.F_ModuleId))
                    {
                        dictionarylist[item.F_ModuleId].AddRange(buttonList);
                        dictionarylist[item.F_ModuleId]= dictionarylist[item.F_ModuleId].GroupBy(p => p.F_Id).Select(q => q.First()).ToList();
                    }
                    else
                    {
                        dictionarylist.Add(item.F_ModuleId, buttonList);
                    }
                }
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, Dictionary<string, List<ModuleButtonEntity>>>();
                    dictionary.Add(roles, dictionarytemp);
                }
                else
                {
                    if (dictionary.ContainsKey(roles))
                    {
                        dictionary[roles] = dictionarytemp;
                    }
                    else
                    {
                        dictionary.Add(roles, dictionarytemp);
                    }
                }
            }
            await CacheHelper.Remove(initcacheKey + "modulebutton_list");
            await CacheHelper.Set(initcacheKey + "modulebutton_list", dictionary);
            return dictionarylist;
        }
        /// <summary>
        /// 菜单字段信息
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetMenuFieldsListNew()
        {
            var currentuser = _userService.currentuser;
            var roleId = currentuser.RoleId;
            if (roleId == null && currentuser.IsSystem)
            {
                roleId = "admin";
            }
            var rolelist = roleId.Split(',');
            Dictionary<string, Dictionary<string, List<ModuleFieldsEntity>>> dictionary = await CacheHelper.Get<Dictionary<string, Dictionary<string, List<ModuleFieldsEntity>>>>(initcacheKey + "modulefields_list");
            var dictionarylist = new Dictionary<string, List<ModuleFieldsEntity>>();
            if (currentuser.UserId == null)
            {
                return dictionarylist;
            }
            foreach (var roles in rolelist)
            {
                var dictionarytemp = new Dictionary<string, List<ModuleFieldsEntity>>();
                var data = await _roleAuthorizeService.GetFieldsList(roles);
                var dataModuleId = data.Where(a => a.F_ModuleId != null && a.F_ModuleId != "").Distinct(new ExtList<ModuleFieldsEntity>("F_ModuleId"));
                foreach (ModuleFieldsEntity item in dataModuleId)
                {
                    var buttonList = data.Where(t => t.F_ModuleId == item.F_ModuleId).ToList();
                    dictionarytemp.Add(item.F_ModuleId, buttonList);
                    if (dictionarylist.ContainsKey(item.F_ModuleId))
                    {
                        dictionarylist[item.F_ModuleId].AddRange(buttonList);
                        dictionarylist[item.F_ModuleId] = dictionarylist[item.F_ModuleId].GroupBy(p => p.F_Id).Select(q => q.First()).ToList();
                    }
                    else
                    {
                        dictionarylist.Add(item.F_ModuleId, buttonList);
                    }
                }
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, Dictionary<string, List<ModuleFieldsEntity>>>();
                    dictionary.Add(roles, dictionarytemp);
                }
                else
                {
                    if (dictionary.ContainsKey(roles))
                    {
                        dictionary[roles] = dictionarytemp;
                    }
                    else
                    {
                        dictionary.Add(roles, dictionarytemp);
                    }
                }
            }
            await CacheHelper.Remove(initcacheKey + "modulefields_list");
            await CacheHelper.Set(initcacheKey + "modulefields_list", dictionary);
            return dictionarylist;
        }
    }
}
