/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Domain.SystemSecurity;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;

namespace WaterCloud.Service.SystemSecurity
{
    public class FilterIPService : DataFilterService<FilterIPEntity>, IDenpendency
    {
        /// <summary>
        /// 缓存操作类
        /// </summary>

        private string cacheKey = "watercloud_filterip_";// IP过滤
        //获取类名
        
        public FilterIPService(IDbContext context) : base(context)
        {
        }
        public async Task<List<FilterIPEntity>> GetList(string keyword)
        {
            var list = new List<FilterIPEntity>();
            list = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(t => t.F_StartIP.Contains(keyword) || t.F_EndIP.Contains(keyword)).ToList();

            }
            return list.Where(a => a.F_DeleteMark == false).OrderBy(t => t.F_CreatorTime).ToList();
        }
        public async Task<List<FilterIPEntity>> GetLookList(string keyword)
        {
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_StartIP.Contains(keyword) || t.F_EndIP.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_CreatorTime).ToList();
        }
        public async Task<FilterIPEntity> GetLookForm(string keyValue)
        {
            var cachedata =await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        public async Task<FilterIPEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(",");
            await repository.Delete(t => ids.Contains(t.F_Id));
			foreach (var item in ids)
			{
                await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
        public async Task<bool> CheckIP(string ip)
        {
            var list =await GetList("");
            list = list.Where(a => a.F_EnabledMark == true&&a.F_DeleteMark==false).ToList();
            foreach (var item in list)
            {
                if (item.F_Type == false)
                {
                    long start = IP2Long(item.F_StartIP);
                    long end = IP2Long(item.F_EndIP);
                    long ipAddress = IP2Long(ip);
                    bool inRange = (ipAddress >= start && ipAddress <= end);
                    if (inRange)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }
        public async Task SubmitForm(FilterIPEntity filterIPEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                filterIPEntity.Modify(keyValue);
                await repository.Update(filterIPEntity);
                await CacheHelper.Remove(cacheKey + keyValue);
            }
            else
            {
                filterIPEntity.Create();
                await repository.Insert(filterIPEntity);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
    }
}
