using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Domain.SystemManage;
using Chloe;

namespace WaterCloud.Service.SystemOrganize
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-06-01 09:44
    /// 描 述：数据权限服务类
    /// </summary>
    public class DataPrivilegeRuleService : DataFilterService<DataPrivilegeRuleEntity>,IDenpendency
    {
        private string cacheKey = "watercloud_dataprivilegeruledata_";
        public DataPrivilegeRuleService(IDbContext context) : base(context)
        {
        }
        //获取类名
        
        #region 获取数据
        public async Task<List<DataPrivilegeRuleEntity>> GetList(string keyword = "")
        {
            var list = new List<DataPrivilegeRuleEntity>();
            list = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(t => t.F_ModuleCode.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return list.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<DataPrivilegeRuleEntity>> GetLookList(SoulPage<DataPrivilegeRuleEntity> pagination, string keyword = "")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            pagination = ChangeSoulData(dic, pagination);
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_ModuleCode.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<DataPrivilegeRuleEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        public async Task<DataPrivilegeRuleEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        #endregion

        #region 提交数据
        public async Task SubmitForm(DataPrivilegeRuleEntity entity, string keyValue)
        {
            entity.F_ModuleCode = (await uniwork.FindEntity<ModuleEntity>(entity.F_ModuleId)).F_EnCode;
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.Create();
                await repository.Insert(entity);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                entity.Modify(keyValue); 
                await repository.Update(entity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            await repository.Delete(t => t.F_Id == keyValue);
            await CacheHelper.Remove(cacheKey + keyValue);
            await CacheHelper.Remove(cacheKey + "list");
        }
        #endregion

    }
}
