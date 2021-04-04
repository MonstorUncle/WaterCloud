//-----------------------------------------------------------------------
// <copyright file=" QuickModule.cs" company="JR">
// * Copyright (C) WaterCloud.Framework  All Rights Reserved
// * version : 1.0
// * author  : WaterCloud.Framework
// * FileName: QuickModule.cs
// * history : Created by T4 04/13/2020 16:51:13 
// </copyright>
//-----------------------------------------------------------------------
using WaterCloud.Domain.SystemManage;
using System.Collections.Generic;
using WaterCloud.Code;
using System.Threading.Tasks;
using System.Linq;
using Chloe;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.SystemManage
{
    public class QuickModuleService:DataFilterService<QuickModuleEntity> ,IDenpendency
    {
        /// <summary>
        /// »º´æ²Ù×÷Àà
        /// </summary>

        private string cacheKey = "watercloud_quickmoduledata_";
        public QuickModuleService(IDbContext context) : base(context)
        {
        }
        public async Task<object> GetTransferList(string userId)
        {
            var quicklist = repository.IQueryable(t => t.F_CreatorUserId == userId && t.F_EnabledMark == true).ToList();
            List<ModuleEntity> quicks = new List<ModuleEntity>();
            var user = await uniwork.FindEntity<UserEntity>(userId);
            var roleId = user.F_RoleId;
            if (user.F_Account == GlobalContext.SystemConfig.SysemUserCode || user.F_IsAdmin == true)
            {
                roleId = "admin";
            }
            var rolelist = roleId.Split(',');
            var modulelist = uniwork.IQueryable<RoleAuthorizeEntity>(a => roleId.Contains(a.F_ObjectId) && a.F_ItemType == 1).InnerJoin<ModuleEntity>((a,b)=> a.F_ItemId == b.F_Id && b.F_IsMenu == true).Select((a, b) => a.F_ItemId).ToList();
            if (roleId == "admin")
            {
                modulelist = uniwork.IQueryable<ModuleEntity>(a => a.F_EnabledMark == true && a.F_IsMenu == true && a.F_DeleteMark == false).Select(a => a.F_Id).ToList();
            }
            modulelist = modulelist.Distinct().ToList();
            quicks = uniwork.IQueryable<ModuleEntity>(a => (modulelist.Contains(a.F_Id) || a.F_IsPublic == true) && a.F_IsMenu == true && a.F_EnabledMark == true && a.F_UrlAddress != null)
                .Select(a => new ModuleEntity
                {
                    F_Id = a.F_Id,
                    F_EnabledMark = false,
                    F_FullName = a.F_FullName
                }).ToList();
            foreach (var item in quicklist)
            {
                var temp = quicks.Find(a => a.F_Id == item.F_ModuleId);
                if (temp != null)
                {
                    temp.F_EnabledMark = true;
                }
            }
            return quicks;
        }

        public async Task<List<QuickModuleExtend>> GetQuickModuleList(string userId)
        {
            var quicklist = repository.IQueryable(t => t.F_CreatorUserId == userId && t.F_EnabledMark == true);
            List<QuickModuleExtend> list = new List<QuickModuleExtend>();
            List<QuickModuleEntity> quicks = new List<QuickModuleEntity>();
            uniwork.BeginTrans();
            if (quicklist.Count() == 0)
            {
                var user = await uniwork.FindEntity<UserEntity>(userId);
                var roleId = user.F_RoleId;
                if (user.F_Account == GlobalContext.SystemConfig.SysemUserCode || user.F_IsAdmin == true)
                {
                    roleId = "admin";
                }
                var rolelist = roleId.Split(',');
                var modulelist = uniwork.IQueryable<RoleAuthorizeEntity>(a => roleId.Contains(a.F_ObjectId) && a.F_ItemType == 1).InnerJoin<ModuleEntity>((a, b) => a.F_ItemId == b.F_Id && b.F_IsMenu == true).Select((a, b) => a.F_ItemId).ToList();
                if (roleId == "admin")
                {
                    modulelist = uniwork.IQueryable<ModuleEntity>(a => a.F_EnabledMark == true && a.F_IsMenu == true && a.F_DeleteMark == false).Select(a => a.F_Id).ToList();
                }
                var temp = uniwork.IQueryable<ModuleEntity>(a => a.F_IsPublic == true && a.F_IsMenu == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Select(a => a.F_Id).ToList();
                modulelist.AddRange(temp);
                modulelist = modulelist.Distinct().ToList();
                foreach (var item in modulelist)
                {
                    var module = await uniwork.FindEntity<ModuleEntity>(a => a.F_Id == item && a.F_EnabledMark == true);
                    if (module != null && module.F_UrlAddress != null && list.Count < 8)
                    {
                        list.Add(new QuickModuleExtend
                        {
                            id = module.F_Id,
                            title = module.F_FullName,
                            href = module.F_UrlAddress,
                            icon = module.F_Icon
                        });
                        QuickModuleEntity quick = new QuickModuleEntity();
                        quick.Create();
                        quick.F_DeleteMark = false;
                        quick.F_EnabledMark = true;
                        quick.F_ModuleId = module.F_Id;
                        quicks.Add(quick);
                    }
                }
            }
            else
            {
                foreach (var item in quicklist.ToList())
                {
                    var module = await uniwork.FindEntity<ModuleEntity>(a => a.F_Id == item.F_ModuleId && a.F_EnabledMark == true);
                    if (module != null)
                    {
                        list.Add(new QuickModuleExtend
                        {
                            id = module.F_Id,
                            title = module.F_FullName,
                            href = module.F_UrlAddress,
                            icon = module.F_Icon
                        });
                    }
					else
					{
                        await repository.Delete(a => a.F_Id == item.F_Id);
                    }
                }
            }
            if (quicks.Count > 0)
            {
                await uniwork.Insert(quicks);
            }
            uniwork.Commit();
            return list;
        }

        public async Task SubmitForm(string[] permissionIds)
        {
            List<QuickModuleEntity> list = new List<QuickModuleEntity>();
            if (permissionIds != null)
            {
                foreach (var itemId in permissionIds)
                {
                    QuickModuleEntity entity = new QuickModuleEntity();
                    entity.Create();
                    entity.F_ModuleId = itemId;
                    entity.F_EnabledMark = true;
                    entity.F_DeleteMark = false;
                    list.Add(entity);
                }
            }
            uniwork.BeginTrans();
            await repository.Delete(t => t.F_CreatorUserId == currentuser.UserId);
            await repository.Insert(list);
            uniwork.Commit();
            var data =await CacheHelper.Get<Dictionary<string, List<QuickModuleExtend>>>(cacheKey + "list");
            if (data != null&&data.ContainsKey(currentuser.UserId))
            {
                data.Remove(currentuser.UserId);
            }
            await CacheHelper.Remove(cacheKey + "list");
            await CacheHelper.Set(cacheKey + "list",data);
        }

    }
}