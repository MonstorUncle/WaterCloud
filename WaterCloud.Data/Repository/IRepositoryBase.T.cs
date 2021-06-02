/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using Chloe;
using WaterCloud.Code;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WaterCloud.DataBase
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IRepositoryBase<TEntity> where TEntity : class, new()
    {
        IDbContext GetDbContext();
        Task<TEntity> Insert(TEntity entity);
        Task<int> Insert(List<TEntity> entitys);
        Task<int> Update(TEntity entity);
        Task<int> Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content);
        Task<int> Delete(TEntity entity);
        Task<int> Delete(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindEntity(object keyValue);
        Task<TEntity> FindEntity(Expression<Func<TEntity, bool>> predicate);
        IQuery<TEntity> IQueryable(LockType locktype = LockType.NoLock);
        IQuery<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate, LockType locktype = LockType.NoLock);
        Task<List<TEntity>> FindList(string strSql);
        Task<List<TEntity>> FindList(string strSql, DbParam[] dbParameter);
        Task<List<TEntity>> FindList(Pagination pagination);
        Task<List<TEntity>> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination);
        Task<List<T>> OrderList<T>(IQuery<T> query, Pagination pagination);
        Task<List<T>> OrderList<T>(IQuery<T> query, SoulPage<T> pagination);
        /// <summary>
        /// 缓存查询列表(大数据表谨慎使用)
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        Task<List<TEntity>> CheckCacheList(string cacheKey, long old = 0);
        Task<TEntity> CheckCache(string cacheKey, object keyValue, long old = 0);
    }
}
