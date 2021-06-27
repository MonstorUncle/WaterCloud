﻿/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using Chloe;
using WaterCloud.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace WaterCloud.DataBase
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class, new()
    {
        private readonly DbContext _dbBase;
        private readonly IUnitOfWork _unitOfWork;
        public IDbContext Db
        {
            get { return _dbBase; }
        }
        public IUnitOfWork unitOfWork
        {
            get { return _unitOfWork; }
        }
        public RepositoryBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbBase = unitOfWork.GetDbContext();
        }
        public async Task<TEntity> Insert(TEntity entity)
        {
           return await _dbBase.InsertAsync(entity);
        }
        public async Task<int> Insert(List<TEntity> entitys)
        {
            int i = 1;
            await _dbBase.InsertRangeAsync(entitys);
            return i;
        }
        public async Task<int> Update(TEntity entity)
        {
            //反射对比更新对象变更
            TEntity newentity = _dbBase.QueryByKey<TEntity>(entity);
            _dbBase.TrackEntity(newentity);
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
            return await _dbBase.UpdateAsync(newentity);
        }
        public async Task<int> Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content)
        {
            return await _dbBase.UpdateAsync(predicate, content);
        }
        public async Task<int> Delete(TEntity entity)
        {
            return await _dbBase.DeleteAsync(entity);
        }
        public async Task<int> Delete(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbBase.DeleteAsync(predicate);
        }
        public async Task<TEntity> FindEntity(object keyValue)
        {
            return await _dbBase.QueryByKeyAsync<TEntity>(keyValue);
        }
        public async Task<TEntity> FindEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbBase.Query<TEntity>().FirstOrDefault(predicate);
        }
        public IQuery<TEntity> IQueryable(LockType locktype = LockType.NoLock)
        {
            return _dbBase.Query<TEntity>(locktype);
        }
        public IQuery<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate, LockType locktype = LockType.NoLock)
        {
            return _dbBase.Query<TEntity>(locktype).Where(predicate);
        }
        public async Task<List<TEntity>> FindList(string strSql)
        {
            return await _dbBase.SqlQueryAsync<TEntity>(strSql);
        }
        public async Task<List<TEntity>> FindList(string strSql, DbParam[] dbParameter)
        {
            return await _dbBase.SqlQueryAsync<TEntity>(strSql, dbParameter);
        }
        public async Task<List<TEntity>> FindList(Pagination pagination)
        {
            var tempData = _dbBase.Query<TEntity>();
            pagination.records = tempData.Count();
            tempData = tempData.OrderBy(pagination.sort);
            tempData = tempData.TakePage(pagination.page, pagination.rows);
            return tempData.ToList();
        }
        public async Task<List<TEntity>> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        {
            var tempData = _dbBase.Query<TEntity>().Where(predicate);
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
            if (filterSos!=null && filterSos.Count>0)
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
        public async Task<List<TEntity>> CheckCacheList(string cacheKey)
        {
            var cachedata =await CacheHelper.Get<List<TEntity>>(cacheKey);
            if (cachedata == null || cachedata.Count() == 0)
            {
                cachedata = _dbBase.Query<TEntity>().ToList();
                await CacheHelper.Set(cacheKey, cachedata);
            }
            return cachedata;
        }
        public async Task<TEntity> CheckCache(string cacheKey, object keyValue)
        {
            var cachedata = await CacheHelper.Get<TEntity>(cacheKey + keyValue);
            if (cachedata == null)
            {
                cachedata = await _dbBase.QueryByKeyAsync<TEntity>(keyValue);
                if (cachedata != null)
                {
                    await CacheHelper.Set(cacheKey + keyValue, cachedata);
                }
            }
            return cachedata;
        }
    }
}
