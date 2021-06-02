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
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace WaterCloud.DataBase
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    public class RepositoryBase : IRepositoryBase, IDisposable
    {
        private IDbContext _context;
        public RepositoryBase(IDbContext context)
        {
            _context = context;
        }
        public IDbContext GetDbContext()
        {
            return _context;
        }
        public RepositoryBase(string ConnectStr, string providerName)
        {
            _context = DBContexHelper.Contex(ConnectStr, providerName);
        }
        public IRepositoryBase BeginTrans()
        {
            if (_context.Session.CurrentTransaction == null)
            {
                _context.Session.BeginTransaction();
            }
            return this;
        }
        public void Commit()
        {
            try
            {
                if (_context.Session.CurrentTransaction != null)
                {
                    _context.Session.CommitTransaction();
                }
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
            finally
            {
                this.Dispose();
            }
        }
        public void Dispose()
        {
            if (_context.Session.CurrentTransaction != null)
            {
                _context.Session.Dispose();
            }
        }
        public void Rollback()
        {
            if (_context.Session.CurrentTransaction != null)
            {
                _context.Session.RollbackTransaction();
            }
            this.Dispose();
        }
        public async Task<TEntity> Insert<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                return await _context.InsertAsync(entity);
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }
        public async Task<int> Insert<TEntity>(List<TEntity> entitys) where TEntity : class
        {
            try
            {
                await _context.InsertRangeAsync(entitys);
                return 1;
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }
        public async Task<int> Update<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                TEntity newentity = _context.QueryByKey<TEntity>(entity);
                _context.TrackEntity(newentity);
                PropertyInfo[] newprops = newentity.GetType().GetProperties();
                PropertyInfo[] props = entity.GetType().GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.GetValue(entity, null) != null)
                    {
                        PropertyInfo item = newprops.Where(a => a.Name == prop.Name).FirstOrDefault();
                        if (item != null)
                        {
                            item.SetValue(newentity, prop.GetValue(entity, null), null);
                            if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                                item.SetValue(newentity, null, null);
                        }
                    }
                }
                return await _context.UpdateAsync(newentity);
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }
        public async Task<int> Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content) where TEntity : class
        {
            try
            {
                return await _context.UpdateAsync(predicate, content);
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }

        }
        public async Task<int> Delete<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                return await _context.DeleteAsync(entity);
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }

        }
        public async Task<int> Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            {
                return await _context.DeleteAsync(predicate);
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }
        public async Task<TEntity> FindEntity<TEntity>(object keyValue) where TEntity : class
        {
            return await _context.QueryByKeyAsync<TEntity>(keyValue);
        }
        public async Task<TEntity> FindEntity<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return _context.Query<TEntity>().FirstOrDefault(predicate);
        }
        public IQuery<TEntity> IQueryable<TEntity>(LockType locktype = LockType.NoLock) where TEntity : class
        {
            return _context.Query<TEntity>(locktype);
        }
        public IQuery<TEntity> IQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate, LockType locktype = LockType.NoLock) where TEntity : class
        {
            return _context.Query<TEntity>(locktype).Where(predicate);
        }
        public async Task<List<TEntity>> FindList<TEntity>(string strSql) where TEntity : class
        {
            return await _context.SqlQueryAsync<TEntity>(strSql);
        }
        public async Task<List<TEntity>> FindList<TEntity>(string strSql, DbParam[] dbParameter) where TEntity : class
        {
            return await _context.SqlQueryAsync<TEntity>(strSql, dbParameter);
        }
        public async Task<List<TEntity>> FindList<TEntity>(Pagination pagination) where TEntity : class, new()
        {
            var tempData = _context.Query<TEntity>();
            pagination.records = tempData.Count();
            tempData = tempData.OrderBy(pagination.sort);
            tempData = tempData.TakePage(pagination.page, pagination.rows);
            return tempData.ToList();
        }
        public async Task<List<TEntity>> FindList<TEntity>(Expression<Func<TEntity, bool>> predicate, Pagination pagination) where TEntity : class, new()
        {
            var tempData = _context.Query<TEntity>().Where(predicate);
            pagination.records = tempData.Count();
            tempData = tempData.OrderBy(pagination.sort);
            tempData = tempData.TakePage(pagination.page, pagination.rows);
            return tempData.ToList();
        }
        public async Task<List<T>> OrderList<T>(IQuery<T> query, Pagination pagination)
        {
            var tempData = query;
            pagination.records = tempData.Count();
            tempData = tempData.OrderBy(pagination.sort);
            tempData = tempData.TakePage(pagination.page, pagination.rows);
            return tempData.ToList();
        }
        public async Task<List<T>> OrderList<T>(IQuery<T> query, SoulPage<T> pagination)
        {
            var tempData = query;
            List<FilterSo> filterSos = pagination.getFilterSos();
            if (filterSos != null && filterSos.Count > 0)
            {
                tempData = tempData.GenerateFilter("u", filterSos);
            }
            pagination.count = tempData.Count();
            if (pagination.order == "desc")
            {
                tempData = tempData.OrderBy(pagination.field + " " + pagination.order);
            }
            else
            {
                tempData = tempData.OrderBy(pagination.field);
            }
            tempData = tempData.TakePage(pagination.page, pagination.rows);
            return tempData.ToList();
        }
        public async Task<List<TEntity>> CheckCacheList<TEntity>(string cacheKey, long old = 0) where TEntity : class
        {
            var cachedata = await CacheHelper.Get<List<TEntity>>(cacheKey);
            if (cachedata == null || cachedata.Count() == 0)
            {
                cachedata = _context.Query<TEntity>().ToList();
                await CacheHelper.Set(cacheKey, cachedata);
            }
            return cachedata;
        }

        public async Task<TEntity> CheckCache<TEntity>(string cacheKey, object keyValue, long old = 0) where TEntity : class
        {
            var cachedata = await CacheHelper.Get<TEntity>(cacheKey + keyValue);
            if (cachedata == null)
            {
                cachedata = await _context.QueryByKeyAsync<TEntity>(keyValue);
                if (cachedata != null)
                {
                    await CacheHelper.Set(cacheKey + keyValue, cachedata);
                }
            }
            return cachedata;
        }
    }
}
