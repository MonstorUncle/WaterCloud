using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using WaterCloud.Domain.ContentManage;
using Chloe;

namespace WaterCloud.Service.ContentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-06-09 19:42
    /// 描 述：新闻管理服务类
    /// </summary>
    public class ArticleNewsService : DataFilterService<ArticleNewsEntity>, IDenpendency
    {
        public ArticleNewsService(IDbContext context) : base(context)
        {

        }
        private string cacheKey = "watercloud_cms_articlenewsdata_";
        
        #region 获取数据
        public async Task<List<ArticleNewsEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_Title.Contains(keyword) || t.F_Tags.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<ArticleNewsEntity>> GetLookList(SoulPage<ArticleNewsEntity> pagination, string keyword = "", string CategoryId="")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            Dictionary<string, string> isTrue = new Dictionary<string, string>();
            enabledTemp.Add("是", "1");
            enabledTemp.Add("否", "0");
            dic.Add("F_IsTop", isTrue);
            dic.Add("F_IsHot", isTrue);
            pagination = ChangeSoulData(dic, pagination);
            //获取新闻列表
            var query = repository.IQueryable(a => a.F_EnabledMark == true)
            .LeftJoin<ArticleCategoryEntity>((a, b) => a.F_CategoryId == b.F_Id && b.F_EnabledMark == true)
            .Select((a, b) => new ArticleNewsEntity
            {
                F_Id = a.F_Id,
                F_CategoryId = a.F_CategoryId,
                F_CategoryName = b.F_FullName,
                F_Title = a.F_Title,
                F_LinkUrl = a.F_LinkUrl,
                F_ImgUrl = a.F_ImgUrl,
                F_SeoTitle = a.F_SeoTitle,
                F_SeoKeywords = a.F_SeoKeywords,
                F_SeoDescription = a.F_SeoDescription,
                F_Tags = a.F_Tags,
                F_Zhaiyao = a.F_Zhaiyao,
                F_Description = a.F_Description,
                F_SortCode = a.F_SortCode,
                F_IsTop = a.F_IsTop,
                F_IsHot = a.F_IsHot,
                F_IsRed = a.F_IsRed,
                F_Click = a.F_Click,
                F_Source = a.F_Source,
                F_Author = a.F_Author,
                F_EnabledMark = a.F_EnabledMark,
                F_CreatorTime = a.F_CreatorTime,
                F_CreatorUserId = a.F_CreatorUserId,
                F_DeleteMark = a.F_DeleteMark,
                F_DeleteTime = a.F_DeleteTime,
                F_DeleteUserId = a.F_DeleteUserId,
                F_LastModifyTime = a.F_LastModifyTime,
                F_LastModifyUserId = a.F_LastModifyUserId,
            });
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_Title.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(CategoryId))
            {
                query = query.Where(a => a.F_CategoryId.Contains(CategoryId));
            }
            query = query.Where(u => u.F_DeleteMark == false);
            //权限过滤
            query = GetDataPrivilege<ArticleNewsEntity>("u", "",query);           
            return await repository.OrderList(query, pagination);
        }
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public async Task<ArticleNewsEntity> GetForm(string keyValue)
        {
            var query = repository.IQueryable(a => a.F_EnabledMark == true)
                .LeftJoin<ArticleCategoryEntity>((a, b) => a.F_CategoryId == b.F_Id && b.F_EnabledMark == true)
                .Select((a, b) => new ArticleNewsEntity
                {
                    F_Id = a.F_Id,
                    F_CategoryId = a.F_CategoryId,
                    F_CategoryName = b.F_FullName,
                    F_Title = a.F_Title,
                    F_LinkUrl = a.F_LinkUrl,
                    F_ImgUrl = a.F_ImgUrl,
                    F_SeoTitle = a.F_SeoTitle,
                    F_SeoKeywords = a.F_SeoKeywords,
                    F_SeoDescription = a.F_SeoDescription,
                    F_Tags = a.F_Tags,
                    F_Zhaiyao = a.F_Zhaiyao,
                    F_Description = a.F_Description,
                    F_SortCode = a.F_SortCode,
                    F_IsTop = a.F_IsTop,
                    F_IsHot = a.F_IsHot,
                    F_IsRed = a.F_IsRed,
                    F_Click = a.F_Click,
                    F_Source = a.F_Source,
                    F_Author = a.F_Author,
                    F_EnabledMark = a.F_EnabledMark,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_DeleteMark = a.F_DeleteMark,
                    F_DeleteTime = a.F_DeleteTime,
                    F_DeleteUserId = a.F_DeleteUserId,
                    F_LastModifyTime = a.F_LastModifyTime,
                    F_LastModifyUserId = a.F_LastModifyUserId,
                });
            if (!string.IsNullOrEmpty(keyValue))
            {
                query = query.Where(a => a.F_Id == keyValue);
            }
            //字段权限处理
            return GetFieldsFilterData(query.FirstOrDefault());
        }
        #endregion

        #region 提交数据
        public async Task SubmitForm(ArticleNewsEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(entity.F_Zhaiyao))
            {
                entity.F_Zhaiyao = WaterCloud.Code.TextHelper.GetSubString(WaterCloud.Code.WebHelper.NoHtml(entity.F_Description),255);
            }
            if (string.IsNullOrEmpty(entity.F_SeoTitle))
            {
                entity.F_SeoTitle = entity.F_Title;
            }
            if (string.IsNullOrEmpty(entity.F_SeoKeywords))
            {
                entity.F_SeoKeywords = entity.F_Zhaiyao;
            }
            if (string.IsNullOrEmpty(entity.F_SeoDescription))
            {
                entity.F_SeoDescription = entity.F_Zhaiyao;
            }

            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
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
