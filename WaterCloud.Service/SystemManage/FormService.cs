using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.SystemManage;
using Serenity.Data;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.SystemManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-08 14:33
    /// 描 述：表单设计服务类
    /// </summary>
    public class FormService : DataFilterService<FormEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_formdata_";
        
        public FormService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<FormEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_Name.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<FormEntity>> GetLookList(string ItemId="", string keyword = "")
        {
            var query = GetQuery().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(ItemId))
            {
                query = query.Where(u => u.F_OrganizeId == ItemId || u.F_OrganizeId == null || u.F_OrganizeId == "");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_Name.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return query.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<FormEntity>> GetLookList(Pagination pagination,string keyword = "")
        {
            //获取数据权限
            var query = GetQuery().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_Name.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return await repository.OrderList(query, pagination);
        }
        private IQuery<FormEntity> GetQuery()
        {
            var query = repository.IQueryable()
                .LeftJoin<OrganizeEntity>((a, b) => a.F_OrganizeId == b.F_Id)
                .Select((a, b) => new FormEntity
                {
                    F_Id = a.F_Id,
                    F_OrganizeName = b.F_FullName,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_Description = a.F_Description,
                    F_EnabledMark = a.F_EnabledMark,
                    F_OrganizeId = a.F_OrganizeId,
                    F_Content=a.F_Content,
                    F_ContentData=a.F_ContentData,
                    F_ContentParse=a.F_ContentParse,
                    F_DbName=a.F_DbName,
                    F_DeleteMark=a.F_DeleteMark,
                    F_Fields=a.F_Fields,    
                    F_FrmType=a.F_FrmType,
                    F_Name=a.F_Name,
                    F_SortCode=a.F_SortCode,    
                    F_WebId=a.F_WebId,  
                });
            return query;
        }
        public async Task<FormEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        #endregion

        public async Task<FormEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }

        #region 提交数据
        public async Task SubmitForm(FormEntity entity, string keyValue)
        {
            if (entity.F_FrmType!=1)
            {
                var temp = FormUtil.SetValue(entity.F_Content);
                entity.F_ContentData =string.Join(',', temp.ToArray()) ;
                entity.F_Fields = temp.Count();
            }
            else
            {
                var temp = FormUtil.SetValueByWeb(entity.F_WebId);
                entity.F_ContentData = string.Join(',', temp.ToArray());
                entity.F_Fields = temp.Count();
            }
            if (string.IsNullOrEmpty(keyValue))
            {
                //此处需修改
                entity.F_DeleteMark = false;
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
