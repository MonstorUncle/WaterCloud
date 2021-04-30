using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service.CommonService;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Domain.FlowManage;

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
        public async Task<FlowschemeExtend> GetFormExtend(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            var temp = cachedata.MapTo<FlowschemeExtend>();
            var form = await uniwork.FindEntity<FormEntity>(cachedata.F_FrmId);
            temp.F_WebId = form.F_WebId;
            temp.F_FrmContentData = form.F_ContentData;
            temp.F_FrmContent = form.F_Content;
            //创建运行实例
            var flowinstance = new FlowinstanceEntity();
            flowinstance.F_SchemeContent = temp.F_SchemeContent;
            var runtime = new FlowRuntime(flowinstance);
            if (runtime.nextNodeType != -1 && runtime.nextNode != null && runtime.nextNode.setInfo != null && runtime.nextNodeType != 4)
            {
                temp.NextNodeDesignateType = runtime.nextNode.setInfo.NodeDesignate;
                if (temp.NextNodeDesignateType == Setinfo.SPECIAL_USER)
                {
                    temp.NextNodeDesignates = runtime.nextNode.setInfo.NodeDesignateData.users;
                    temp.NextMakerName = string.Join(',', uniwork.IQueryable<UserEntity>(a => temp.NextNodeDesignates.Contains(a.F_Id)).Select(a => a.F_RealName).ToList());
                }
                else if (temp.NextNodeDesignateType == Setinfo.SPECIAL_ROLE)
                {
                    temp.NextNodeDesignates = runtime.nextNode.setInfo.NodeDesignateData.roles;
                    List<UserEntity> list = new List<UserEntity>();
                    List<string> users = new List<string>();
                    foreach (var item in temp.NextNodeDesignates)
                    {
                        var usertemp = uniwork.IQueryable<UserEntity>(a => a.F_RoleId.Contains(item)).ToList();
                        var tempList = new List<UserEntity>();
                        if (runtime.nextNode.setInfo.NodeDesignateData.currentDepart)
                        {
                            var currentDepartment = uniwork.FindEntity<UserEntity>(currentuser.UserId).GetAwaiter().GetResult().F_DepartmentId.Split(',').ToList();
                            foreach (var user in usertemp)
                            {
                                var nextCurrentDepartment = user.F_DepartmentId.Split(',').ToList();
                                if (TextHelper.IsArrayIntersection(currentDepartment, nextCurrentDepartment))
                                {
                                    tempList.Add(user);
                                }
                            }
                        }
                        else
                        {
                            tempList = usertemp;
                        }
                        var tempFinal = tempList.Select(a => a.F_Id).ToList();
                        users.AddRange(tempFinal);
                    }
                    users = users.Distinct().ToList();
                    temp.NextMakerName = string.Join(',', uniwork.IQueryable<UserEntity>(a => users.Contains(a.F_Id)).Select(a => a.F_RealName).ToList());
                }
            }
            return temp;
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
