/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Code;
using WaterCloud.Domain.SystemOrganize;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using WaterCloud.Service.SystemManage;
using Chloe;

namespace WaterCloud.Service.SystemOrganize
{
    public class RoleService : DataFilterService<RoleEntity>, IDenpendency
    {
        private ModuleService moduleApp;
        private ModuleButtonService moduleButtonApp;
        private ModuleFieldsService moduleFieldsApp;
        private ItemsDataService itemsApp;
        /// <summary>
        /// 缓存操作类
        /// </summary>

        private string cacheKey = "watercloud_roledata_";
        private string authorizecacheKey = "watercloud_authorizeurldata_";// +权限
        private string initcacheKey = "watercloud_init_";
        //获取类名
        
        public RoleService(IDbContext context) : base(context)
        {
            moduleApp = new ModuleService(context);
            moduleButtonApp = new ModuleButtonService(context);
            moduleFieldsApp = new ModuleFieldsService(context);
            itemsApp = new ItemsDataService(context);
        }

        public async Task<List<RoleExtend>> GetList( string keyword = "")
        {
            var cachedata = GetQuery();
            if (!string.IsNullOrEmpty(keyword))
            {
                cachedata = cachedata.Where(t => t.F_FullName.Contains(keyword) || t.F_EnCode.Contains(keyword));
            }
            return cachedata.ToList();
        }
        public async Task<List<RoleExtend>> GetLookList(SoulPage<RoleExtend> pagination, string keyword = "")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            var setList = await itemsApp.GetItemList("RoleType");
            Dictionary<string, string> messageTypeTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                messageTypeTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_Type", messageTypeTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query =  GetQuery();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_FullName.Contains(keyword) || u.F_EnCode.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return await repository.OrderList(query, pagination);
        }
        public async Task<RoleEntity> GetForm(string keyValue)
        {
            var cachedata =await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task<RoleEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        private IQuery<RoleExtend> GetQuery()
        {
            var query = repository.IQueryable(u => u.F_DeleteMark == false && u.F_Category == 1)
                .LeftJoin<SystemSetEntity>((a, b) => a.F_OrganizeId == b.F_Id)
                .Select((a, b) => new RoleExtend
                {
                    F_Id = a.F_Id,
                    F_AllowDelete = a.F_AllowDelete,
                    F_AllowEdit = a.F_AllowEdit,
                    F_Category = a.F_Category,
                    F_CompanyName = b.F_CompanyName,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_Description = a.F_Description,
                    F_DeleteMark = a.F_DeleteMark,
                    F_EnabledMark = a.F_EnabledMark,
                    F_EnCode = a.F_EnCode,
                    F_FullName = a.F_FullName,
                    F_OrganizeId=a.F_OrganizeId,
                    F_SortCode=a.F_SortCode,
                    F_Type=a.F_Type,
                });
            return query;
        }
        public async Task DeleteForm(string keyValue)
        {
            if (uniwork.IQueryable<UserEntity>(a => a.F_RoleId.Contains(keyValue)).Count() > 0 )
            {
                throw new Exception("角色使用中，无法删除");
            }
            uniwork.BeginTrans();
            await repository.Delete(t => t.F_Id == keyValue);
            await uniwork.Delete<RoleAuthorizeEntity>(t => t.F_ObjectId == keyValue);
            uniwork.Commit();
            await CacheHelper.Remove(cacheKey + keyValue);
            await CacheHelper.Remove(cacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "authorize_list");
            await CacheHelper.Remove(initcacheKey + "modulebutton_list");
            await CacheHelper.Remove(initcacheKey + "modulefields_list");
            await CacheHelper.Remove(initcacheKey + "list");
        }
        public async Task SubmitForm(RoleEntity roleEntity, string[] permissionIds,string[] permissionfieldsIds, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.F_Id = keyValue;

            }
            else
            {
                roleEntity.F_DeleteMark = false;
                roleEntity.F_AllowEdit = false;
                roleEntity.F_AllowDelete = false;
                roleEntity.Create();

            }
            var moduledata =await moduleApp.GetList();
            var buttondata =await moduleButtonApp.GetList();
            var fieldsdata = await moduleFieldsApp.GetList();
            List<RoleAuthorizeEntity> roleAuthorizeEntitys = new List<RoleAuthorizeEntity>();
            foreach (var itemId in permissionIds)
            {
                RoleAuthorizeEntity roleAuthorizeEntity = new RoleAuthorizeEntity();
                roleAuthorizeEntity.F_Id = Utils.GuId();
                roleAuthorizeEntity.F_ObjectType = 1;
                roleAuthorizeEntity.F_ObjectId = roleEntity.F_Id;
                roleAuthorizeEntity.F_ItemId = itemId;
                if (moduledata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 1;
                    roleAuthorizeEntitys.Add(roleAuthorizeEntity);
                }
                if (buttondata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 2;
                    roleAuthorizeEntitys.Add(roleAuthorizeEntity);
                }
            }
            foreach (var itemId in permissionfieldsIds)
            {
                RoleAuthorizeEntity roleAuthorizeEntity = new RoleAuthorizeEntity();
                roleAuthorizeEntity.F_Id = Utils.GuId();
                roleAuthorizeEntity.F_ObjectType = 1;
                roleAuthorizeEntity.F_ObjectId = roleEntity.F_Id;
                roleAuthorizeEntity.F_ItemId = itemId;
                if (fieldsdata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 3;
                    roleAuthorizeEntitys.Add(roleAuthorizeEntity);
                }
            }
            uniwork.BeginTrans();
            if (!string.IsNullOrEmpty(keyValue))
            {
                await repository.Update(roleEntity);
            }
            else
            {
                roleEntity.F_Category = 1;
                await repository.Insert(roleEntity);
            }
            await uniwork.Delete<RoleAuthorizeEntity>(t => t.F_ObjectId == roleEntity.F_Id);
            await uniwork.Insert(roleAuthorizeEntitys);
            uniwork.Commit();
            await CacheHelper.Remove(cacheKey + keyValue);
            await CacheHelper.Remove(cacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "authorize_list");
            await CacheHelper.Remove(initcacheKey + "modulebutton_list");
            await CacheHelper.Remove(initcacheKey + "modulefields_list");
            await CacheHelper.Remove(initcacheKey + "list");
        }
    }
}
