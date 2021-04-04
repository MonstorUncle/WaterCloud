/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Domain.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;

namespace WaterCloud.Service.SystemManage
{
    public class ItemsDataService : DataFilterService<ItemsDetailEntity>,IDenpendency
    {
        /// <summary>
        /// 缓存操作类
        /// </summary>
        public ItemsDataService(IDbContext context) : base(context)
        {
        }
        private string cacheKey = "watercloud_itemdetaildata_";
        private string itemcacheKey = "watercloud_itemsdata_";
        //获取类名
        
        public async Task<List<ItemsDetailEntity>> GetList(string itemId = "", string keyword = "")
        {
            var list = new List<ItemsDetailEntity>();
            list = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(itemId))
            {
                list = list.Where(t => t.F_ItemId == itemId).ToList();
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(t => t.F_ItemName.Contains(keyword) || t.F_ItemCode.Contains(keyword)).ToList();
            }
            return list.Where(a => a.F_DeleteMark == false).OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<List<ItemsDetailEntity>> GetLookList(string itemId = "", string keyword = "")
        {
            var query = repository.IQueryable().Where(a => a.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(itemId))
            {
                query = query.Where(t => t.F_ItemId == itemId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_ItemName.Contains(keyword) || t.F_ItemCode.Contains(keyword));
            }
            query = GetDataPrivilege("u","", query);
            return query.OrderBy(t => t.F_SortCode).ToList();
        }
        public async Task<List<ItemsDetailEntity>> GetItemList(string enCode)
        {
            var itemcachedata =await uniwork.CheckCacheList<ItemsEntity>(itemcacheKey + "list");
            var item = itemcachedata.Find(a => a.F_EnCode == enCode);
            var cachedata =await repository.CheckCacheList(cacheKey + "list");
            cachedata = cachedata.Where(a => a.F_DeleteMark == false && a.F_EnabledMark == true && a.F_ItemId == item.F_Id).OrderBy(a => a.F_SortCode).ToList();
            return cachedata;
        }
        public async Task<ItemsDetailEntity> GetLookForm(string keyValue)
        {
            var cachedata =await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        public async Task<ItemsDetailEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task DeleteForm(string keyValue)
        {
            await repository.Delete(t => t.F_Id == keyValue);
            await CacheHelper.Remove(cacheKey + keyValue);
            await CacheHelper.Remove(cacheKey + "list");
        }
        public async Task SubmitForm(ItemsDetailEntity itemsDetailEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                itemsDetailEntity.Modify(keyValue);
                await repository.Update(itemsDetailEntity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                itemsDetailEntity.F_DeleteMark = false;
                itemsDetailEntity.Create();
                await repository.Insert(itemsDetailEntity);
                await CacheHelper.Remove(cacheKey + "list");
            }
        }
    }
}
