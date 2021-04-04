using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.SystemManage;

namespace WaterCloud.Service.SystemManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-10 08:49
    /// 描 述：流程设计服务类
    /// </summary>
    public class FlowschemeService : DataFilterService<FlowschemeEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_flowschemedata_";
        
        public FlowschemeService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<FlowschemeEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");

            if (!string.IsNullOrEmpty(keyword))
            {
                cachedata = cachedata.Where(t => t.F_SchemeCode.Contains(keyword) || t.F_SchemeName.Contains(keyword)).ToList();
            }
            var list = currentuser.DepartmentId.Split(',');
            if (list.Count() > 0)
            {
                return cachedata.Where(t => t.F_DeleteMark == false && (t.F_OrganizeId == "" || t.F_OrganizeId == null || list.Contains(t.F_OrganizeId))).OrderByDescending(t => t.F_CreatorTime).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false && t.F_OrganizeId == "" || t.F_OrganizeId == null).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<FlowschemeEntity>> GetLookList(string ItemId = "", string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(ItemId))
            {
                query = query.Where(u => u.F_OrganizeId == ItemId || u.F_OrganizeId == null || u.F_OrganizeId == "");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_SchemeCode.Contains(keyword) || u.F_SchemeName.Contains(keyword));
            }
            query = GetDataPrivilege("u","", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<FlowschemeEntity>> GetLookList(Pagination pagination,string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_SchemeCode.Contains(keyword) || u.F_SchemeName.Contains(keyword));
            }
            query = GetDataPrivilege("u","", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<FlowschemeEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        #endregion

        public async Task<FlowschemeEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }

        #region 提交数据
        public async Task SubmitForm(FlowschemeEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                    //此处需修改
                entity.Create();
                await repository.Insert(entity);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                    //此处需修改
                entity.Modify(keyValue); 
                await repository.Update(entity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id));
            foreach (var item in ids)
            {
            await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
        #endregion

    }
}
