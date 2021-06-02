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
    public interface IRepositoryBase : IDisposable
    {
        IDbContext GetDbContext();
        IRepositoryBase BeginTrans();
        void Commit();
        void Rollback();
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        Task<TEntity> Insert<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitys"></param>
        Task<int> Insert<TEntity>(List<TEntity> entitys) where TEntity : class;
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<int> Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content) where TEntity : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        /// <summary>
        /// 查询单个对象根据主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        Task<TEntity> FindEntity<TEntity>(object keyValue) where TEntity : class;
        /// <summary>
        /// 查询单个对象根据条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> FindEntity<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQuery<TEntity> IQueryable<TEntity>(LockType locktype = LockType.NoLock) where TEntity : class;
        /// <summary>
        /// 查询根据条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQuery<TEntity> IQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate, LockType locktype = LockType.NoLock) where TEntity : class;
        /// <summary>
        /// SQL查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="strSql"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindList<TEntity>(string strSql) where TEntity : class;
        /// <summary>
        /// sql查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="strSql"></param>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindList<TEntity>(string strSql, DbParam[] dbParameter) where TEntity : class;
        /// <summary>
        /// 查询根据分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pagination"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindList<TEntity>(Pagination pagination) where TEntity : class, new();
        /// <summary>
        /// 查询根据条件和分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindList<TEntity>(Expression<Func<TEntity, bool>> predicate, Pagination pagination) where TEntity : class, new();
        /// <summary>
        /// 对IQuery参数分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        Task<List<T>> OrderList<T>(IQuery<T> query, Pagination pagination);
        /// <summary>
        /// soultable-后端筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        Task<List<T>> OrderList<T>(IQuery<T> query, SoulPage<T> pagination);
        /// <summary>
        /// 缓存单个对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="keyValue"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        Task<TEntity> CheckCache<TEntity>(string cacheKey, object keyValue, long old = 0) where TEntity : class;
        /// <summary>
        /// 缓存查询列表(大数据表谨慎使用)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        Task<List<TEntity>> CheckCacheList<TEntity>(string cacheKey, long old = 0) where TEntity : class;

    }
}
