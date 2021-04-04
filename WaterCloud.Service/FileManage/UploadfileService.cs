using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.FileManage;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.FileManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-22 12:04
    /// 描 述：文件管理服务类
    /// </summary>
    public class UploadfileService : DataFilterService<UploadfileEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_uploadfiledata_";
        
        public UploadfileService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<UploadfileEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_FileName.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return cachedata.OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<UploadfileEntity>> GetLookList(string keyword = "")
        {
            var query = GetQuery();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_FileName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            var data = query.OrderByDesc(t => t.F_CreatorTime).ToList();
            foreach (var item in data)
            {
                string[] departments = item.F_OrganizeId.Split(',');
                item.F_OrganizeName = string.Join(',', uniwork.IQueryable<OrganizeEntity>(a => departments.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

        public async Task<List<UploadfileEntity>> GetLookList(SoulPage<UploadfileEntity> pagination, string keyword = "")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            Dictionary<string, string> fileTypeTemp = new Dictionary<string, string>();
            fileTypeTemp.Add("图片", "1");
            fileTypeTemp.Add("文件", "0");
            dic.Add("F_FileType", fileTypeTemp);
            pagination = ChangeSoulData(dic, pagination);
            var query = GetQuery();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_FileName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            //权限过滤
            query = GetDataPrivilege("u", "", query);
            var data = await repository.OrderList(query, pagination);
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            foreach (var item in data)
            {
                string[] departments = item.F_OrganizeId.Split(',');
                item.F_OrganizeName = string.Join(',', orgs.Where(a => departments.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }
        private IQuery<UploadfileEntity> GetQuery()
        {
            var query = repository.IQueryable()
                .LeftJoin<UserEntity>((a, b) => a.F_CreatorUserId == b.F_Id)
                .Select((a, b) => new UploadfileEntity
                {
                    F_Id = a.F_Id,
                    F_CreatorUserName=b.F_RealName,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_Description = a.F_Description,
                    F_EnabledMark = a.F_EnabledMark,
                    F_FileExtension=a.F_FileExtension,
                    F_FileBy=a.F_FileBy,
                    F_FileName=a.F_FileName,
                    F_FilePath=a.F_FilePath,
                    F_FileSize=a.F_FileSize,
                    F_FileType=a.F_FileType,
                    F_OrganizeId=a.F_OrganizeId,    
                });
            return query;
        }
        public async Task<UploadfileEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task<UploadfileEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        #endregion

        #region 提交数据
        public async Task SubmitForm(UploadfileEntity entity, string keyValue)
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
