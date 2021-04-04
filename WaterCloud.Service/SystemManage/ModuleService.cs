/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Domain.SystemManage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.SystemManage
{
    public class ModuleService : DataFilterService<ModuleEntity>, IDenpendency
    {
        /// <summary>
        /// 缓存操作类
        /// </summary>

        private string cacheKey = "watercloud_moduleldata_";
        private string quickcacheKey = "watercloud_quickmoduledata_";
        private string initcacheKey = "watercloud_init_";
        private string modulebuttoncacheKey = "watercloud_modulebuttondata_";
        private string modulefieldscacheKey = "watercloud_modulefieldsdata_";
        private string authorizecacheKey = "watercloud_authorizeurldata_";// +权限
        //获取类名

        public ModuleService(IDbContext context) : base(context)
        {
        }

        public async Task<List<ModuleEntity>> GetList()
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            return cachedata.Where(a => a.F_DeleteMark == false).OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<List<ModuleEntity>> GetBesidesList()
        {
            var moduleList = uniwork.IQueryable<DataPrivilegeRuleEntity>().Select(a => a.F_ModuleId).ToList();
            var query = repository.IQueryable().Where(a => !moduleList.Contains(a.F_Id) && a.F_EnabledMark == true && a.F_Target == "iframe");
            return query.OrderBy(a => a.F_SortCode).ToList();
        }
        public async Task<List<ModuleEntity>> GetLookList()
        {
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            query = GetDataPrivilege("u", "", query);
            return query.OrderBy(u => u.F_SortCode).ToList();
        }
        public async Task<ModuleEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        public async Task<ModuleEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task DeleteForm(string keyValue)
        {
            if (repository.IQueryable(t => t.F_ParentId.Equals(keyValue)).Count() > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                uniwork.BeginTrans();
                await repository.Delete(a => a.F_Id == keyValue);
                await uniwork.Delete<ModuleButtonEntity>(a => a.F_ModuleId == keyValue);
                await uniwork.Delete<ModuleFieldsEntity>(a => a.F_ModuleId == keyValue);
                uniwork.Commit();
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
                await CacheHelper.Remove(quickcacheKey + "list");
                await CacheHelper.Remove(initcacheKey + "list");
                await CacheHelper.Remove(initcacheKey + "modulebutton_list");
                await CacheHelper.Remove(initcacheKey + "modulefields_list");
                await CacheHelper.Remove(authorizecacheKey + "list");
                await CacheHelper.Remove(authorizecacheKey + "authorize_list");
                await CacheHelper.Remove(modulebuttoncacheKey + "list");
                await CacheHelper.Remove(modulefieldscacheKey + "list");
            }
        }

        public async Task<List<ModuleEntity>> GetListByRole(string roleid)
        {
            var moduleList = uniwork.IQueryable<RoleAuthorizeEntity>(a => a.F_ObjectId == roleid && a.F_ItemType == 1).Select(a => a.F_ItemId).ToList();
            var query = repository.IQueryable().Where(a => (moduleList.Contains(a.F_Id) || a.F_IsPublic == true) && a.F_DeleteMark == false && a.F_EnabledMark == true);
            return query.OrderBy(a => a.F_SortCode).ToList();
        }

        public async Task SubmitForm(ModuleEntity moduleEntity, string keyValue)
        {
			if (!string.IsNullOrEmpty(moduleEntity.F_Authorize))
			{
                moduleEntity.F_Authorize = moduleEntity.F_Authorize.ToLower();
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                moduleEntity.Modify(keyValue);
                await repository.Update(moduleEntity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                moduleEntity.Create();
                await repository.Insert(moduleEntity);
                await CacheHelper.Remove(cacheKey + "list");
            }
            await CacheHelper.Remove(quickcacheKey + "list");
            await CacheHelper.Remove(initcacheKey + "list");
            await CacheHelper.Remove(initcacheKey + "modulebutton_list");
            await CacheHelper.Remove(initcacheKey + "modulefields_list");
            await CacheHelper.Remove(authorizecacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "authorize_list");
            await CacheHelper.Remove(modulebuttoncacheKey + "list");
            await CacheHelper.Remove(modulefieldscacheKey + "list");
        }
    }
}
