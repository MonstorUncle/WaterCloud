/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Domain.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;

namespace WaterCloud.Service.SystemManage
{
    public class AreaService : DataFilterService<AreaEntity>, IDenpendency
    {
        public AreaService(IDbContext context) : base(context)
        {
        }
        //获取类名
        
        /// <summary>
        /// 缓存操作类
        /// </summary>
        private string cacheKey = "watercloud_areadata_";// 区域
        public async Task<List<AreaEntity>> GetList(int layers = 0)
        {
            var list = new List<AreaEntity>();
            list = await repository.CheckCacheList(cacheKey + "list");
            if (layers != 0)
            {
                list = list.Where(t => t.F_Layers == layers).ToList();
            }
            return list.Where(t => t.F_DeleteMark == false && t.F_EnabledMark == true).OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<List<AreaEntity>> GetLookList(int layers=0)
        {
            var query =repository .IQueryable ().Where(t => t.F_DeleteMark == false && t.F_EnabledMark == true);
            if (layers!=0)
            { 
                query = query.Where(t => t.F_Layers == layers);
            }
            query = GetDataPrivilege("u","", query);
            return query.OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<AreaEntity> GetLookForm(string keyValue)
        {
            var cachedata =await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        public async Task<AreaEntity> GetForm(string keyValue)
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
            }
            await CacheHelper.Remove(cacheKey + keyValue);
            await CacheHelper.Remove(cacheKey + "list");
        }
        public async Task SubmitForm(AreaEntity mEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                mEntity.Modify(keyValue);
                await repository.Update(mEntity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                mEntity.F_DeleteMark = false;
                mEntity.Create();
                await repository.Insert(mEntity);
                await CacheHelper.Remove(cacheKey + "list");
            }
        }
    }
}
