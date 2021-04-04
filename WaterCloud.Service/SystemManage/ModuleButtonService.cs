/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Code;
using WaterCloud.Domain.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.SystemManage
{
    public class ModuleButtonService : DataFilterService<ModuleButtonEntity>, IDenpendency
    {
        //获取类名
        
        /// <summary>
        /// 缓存操作类
        /// </summary>

        private string cacheKey = "watercloud_modulebuttondata_";
        private string initcacheKey = "watercloud_init_";
        private string authorizecacheKey = "watercloud_authorizeurldata_";// +权限
        public ModuleButtonService(IDbContext context) : base(context)
        {
        }
        public async Task<List<ModuleButtonEntity>> GetList(string moduleId = "")
        {
            var list = new List<ModuleButtonEntity>();
            list = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(moduleId))
            {
                list = list.Where(t => t.F_ModuleId == moduleId).ToList();
            }
            return list.Where(a => a.F_DeleteMark == false).OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<List<ModuleButtonEntity>> GetLookList(string moduleId = "", string keyword = "")
        {
            var query = repository.IQueryable().Where(a => a.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(moduleId))
            {
                query = query.Where(t => t.F_ModuleId == moduleId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_FullName.Contains(keyword) || t.F_EnCode.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<ModuleButtonEntity> GetLookForm(string keyValue)
        {
            var cachedata =await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        public async Task<ModuleButtonEntity> GetForm(string keyValue)
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
                await repository.Delete(t => t.F_Id == keyValue);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
            await CacheHelper.Remove(initcacheKey + "modulebutton_" + "list");
            await CacheHelper.Remove(authorizecacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "authorize_list");
        }

        public async Task<List<ModuleButtonEntity>> GetListByRole(string roleid)
        {
            var moduleList = uniwork.IQueryable<RoleAuthorizeEntity>(a => a.F_ObjectId == roleid && a.F_ItemType == 2).Select(a => a.F_ItemId).ToList();
            var query = repository.IQueryable().Where(a => (moduleList.Contains(a.F_Id) || a.F_IsPublic == true) && a.F_DeleteMark == false && a.F_EnabledMark == true);
            return query.OrderBy(a => a.F_SortCode).ToList();
        }

        public async Task SubmitForm(ModuleButtonEntity moduleButtonEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(moduleButtonEntity.F_Authorize))
            {
                moduleButtonEntity.F_Authorize = moduleButtonEntity.F_Authorize.ToLower();
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                moduleButtonEntity.Modify(keyValue);
                await repository.Update(moduleButtonEntity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                moduleButtonEntity.F_DeleteMark = false;
                moduleButtonEntity.F_AllowEdit = false;
                moduleButtonEntity.F_AllowDelete = false;
                var module = await uniwork.FindEntity<ModuleEntity>(a => a.F_Id == moduleButtonEntity.F_ModuleId);
                if (string.IsNullOrEmpty(module.F_UrlAddress) || module.F_Target != "iframe")
                {
                    throw new Exception("菜单不能创建按钮");
                }
                moduleButtonEntity.Create();
                await repository.Insert(moduleButtonEntity);
                await CacheHelper.Remove(cacheKey + "list");
            }
            await CacheHelper.Remove(initcacheKey + "modulebutton_" + "list");
            await CacheHelper.Remove(authorizecacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "authorize_list");
        }
        public async Task SubmitCloneButton(string moduleId, string Ids)
        {
            string[] ArrayId = Ids.Split(',');
            var data =await this.GetList();
            List<ModuleButtonEntity> entitys = new List<ModuleButtonEntity>();
            var module = await uniwork.FindEntity<ModuleEntity>(a => a.F_Id == moduleId);
            if (string.IsNullOrEmpty(module.F_UrlAddress)||module.F_Target!= "iframe")
            {
                throw new Exception("菜单不能创建按钮");
            }
            foreach (string item in ArrayId)
            {
                ModuleButtonEntity moduleButtonEntity = data.Find(t => t.F_Id == item);
                moduleButtonEntity.Create();
                moduleButtonEntity.F_ModuleId = moduleId;
                entitys.Add(moduleButtonEntity);
            }
            await repository.Insert(entitys);
            await CacheHelper.Remove(cacheKey + "list");
            await CacheHelper.Remove(initcacheKey + "modulebutton_" + "list");
            await CacheHelper.Remove(authorizecacheKey + "list");
            await CacheHelper.Remove(authorizecacheKey + "authorize_list");
        }

        public async Task<List<ModuleButtonEntity>> GetListNew(string moduleId = "")
        {
            var query = repository.IQueryable(a => a.F_EnabledMark == true && a.F_DeleteMark == false)
            .InnerJoin<ModuleEntity>((a, b) => a.F_ModuleId == b.F_Id && b.F_EnabledMark == true && a.F_DeleteMark == false)
            .Select((a, b) => new ModuleButtonEntity
            {
                F_Id = a.F_Id,
                F_AllowDelete = a.F_AllowDelete,
                F_AllowEdit = a.F_AllowEdit,
                F_UrlAddress = a.F_UrlAddress,
                F_CreatorTime = a.F_CreatorTime,
                F_CreatorUserId = a.F_CreatorUserId,
                F_DeleteMark = a.F_DeleteMark,
                F_DeleteTime = a.F_DeleteTime,
                F_DeleteUserId = a.F_DeleteUserId,
                F_Description = a.F_Description,
                F_EnabledMark = a.F_EnabledMark,
                F_EnCode = a.F_EnCode,
                F_FullName = a.F_FullName,
                F_Icon = a.F_Icon,
                F_IsPublic = a.F_IsPublic,
                F_JsEvent = a.F_JsEvent,
                F_LastModifyTime = a.F_LastModifyTime,
                F_LastModifyUserId = a.F_LastModifyUserId,
                F_Layers = a.F_Layers,
                F_Location = a.F_Location,
                F_ModuleId = b.F_UrlAddress,
                F_ParentId = a.F_ParentId,
                F_SortCode = a.F_SortCode,
                F_Split = a.F_Split,
            });
            if (!string.IsNullOrEmpty(moduleId))
            {
                query = query.Where(a => a.F_ModuleId == moduleId);
            }
            return query.OrderBy(a => a.F_SortCode).ToList();
        }
    }
}
