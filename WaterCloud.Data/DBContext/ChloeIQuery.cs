using Chloe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WaterCloud.Code;

namespace WaterCloud.DataBase
{
    public static partial class ChloeIQuery
    {
        #region Filter 拼接
        /// <summary>
        /// 创建linq表达示的body部分
        /// </summary>
        public static Expression GenerateBody<T>(this ParameterExpression param, Filter filterObj)
        {
            PropertyInfo property = typeof(T).GetProperty(filterObj.Key);

            Expression left = null; //组装左边
            //组装右边
            Expression right = null;
            if (property != null)
            {
                left = Expression.Property(param, property);
                if (property.PropertyType == typeof(int))
                {
                    int i;
                    try
                    {
                        i = int.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = -99999;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<int>))
                {
                    int? i;
                    try
                    {
                        i = int.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.Value) ? null : i, typeof(int?));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime i;
                    try
                    {
                        i = DateTime.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = DateTime.Now.AddYears(-100);
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<DateTime>))
                {
                    DateTime? i;
                    try
                    {
                        i = DateTime.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.Value) ? null : i, typeof(DateTime?));
                }
                else if (property.PropertyType == typeof(string))
                {
                    right = Expression.Constant(filterObj.Value);
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    decimal i;
                    try
                    {
                        i = decimal.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = -99999;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<decimal>))
                {
                    decimal? i;
                    try
                    {
                        i = decimal.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.Value) ? null : i, typeof(decimal?));
                }
                else if (property.PropertyType == typeof(float))
                {
                    float i;
                    try
                    {
                        i = float.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = -99999;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<float>))
                {
                    float? i;
                    try
                    {
                        i = float.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.Value) ? null : i, typeof(float?));
                }
                else if (property.PropertyType == typeof(bool))
                {
                    bool i = false;
                    if (filterObj.Value.ToString().ToLower() == "1" || filterObj.Value.ToString().ToLower() == "true")
                    {
                        i = true;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<bool>))
                {
                    bool i = false;
                    if (filterObj.Value.ToString().ToLower() == "1" || filterObj.Value.ToString().ToLower() == "true")
                    {
                        i = true;
                    }
                    right = Expression.Constant(i, typeof(bool?));
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    Guid i;
                    try
                    {
                        i = Guid.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = new Guid();
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Guid?))
                {
                    Guid? i;
                    try
                    {
                        i = Guid.Parse(filterObj.Value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.Value) ? null : i, typeof(Guid?));
                }
                else
                {
                    throw new Exception("暂不能解析该Key的类型");
                }
            }
            else //如果左边不是属性，直接是值的情况
            {
                left = Expression.Constant(filterObj.Key);
                right = Expression.Constant(filterObj.Value);
            }

            //c.XXX=="XXX"
            Expression filter = Expression.Equal(left, right);
            switch (filterObj.Contrast)
            {
                case "<=":
                    filter = Expression.LessThanOrEqual(left, right);
                    break;

                case "<":
                    filter = Expression.LessThan(left, right);
                    break;

                case ">":
                    filter = Expression.GreaterThan(left, right);
                    break;

                case ">=":
                    filter = Expression.GreaterThanOrEqual(left, right);
                    break;
                case "!=":
                    filter = Expression.NotEqual(left, right);
                    break;
                case "contains":
                    filter = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                        Expression.Constant(filterObj.Value));
                    break;
                case "in":
                    var lExp = Expression.Constant(filterObj.Value.Split(',').ToList()); //数组
                    var methodInfo = typeof(List<string>).GetMethod("Contains",
                        new Type[] { typeof(string) }); //Contains语句
                    filter = Expression.Call(lExp, methodInfo, left);
                    break;
                case "not in":
                    var listExpression = Expression.Constant(filterObj.Value.Split(',').ToList()); //数组
                    var method = typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) }); //Contains语句
                    filter = Expression.Not(Expression.Call(listExpression, method, left));
                    break;
                //交集，使用交集时左值必须时固定的值
                case "intersect": //交集
                    if (property != null)
                    {
                        throw new Exception("交集模式下，表达式左边不能为变量，请调整数据规则，如:c=>\"A,B,C\" intersect \"B,D\"");
                    }

                    var rightval = filterObj.Value.Split(',').ToList();
                    var leftval = filterObj.Key.Split(',').ToList();
                    var val = rightval.Intersect(leftval);

                    filter = Expression.Constant(val.Count() > 0);
                    break;
            }

            return filter;
        }
        public static Expression<Func<T, bool>> GenerateTypeBody<T>(this ParameterExpression param, Filter filterObj)
        {
            return (Expression<Func<T, bool>>)(param.GenerateBody<T>(filterObj));
        }
        /// <summary>
        /// 转换FilterGroup为Lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parametername"></param>
        /// <param name="filterList"></param>
        /// <returns></returns>
        public static IQuery<T> GenerateFilter<T>(this IQuery<T> query, string parametername,
            List<FilterList> filterList)
        {
            var param = ExtLinq.CreateLambdaParam<T>(parametername);
            Expression result = ConvertList<T>(filterList, param);
            query = query.Where(param.GenerateTypeLambda<T>(result));
            return query;
        }
        /// <summary>
        /// 转换filterlist为表达式
        /// </summary>
        /// <param name="filterList"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression ConvertList<T>(List<FilterList> filterList, ParameterExpression param)
        {
            if (filterList == null) return null;
            Expression result = ConvertFilters<T>(JsonHelper.ToObject<Filter[]>(filterList[0].Filters), param, filterList[0].Operation);
            foreach (var item in filterList.Skip(1))
            {
                var gresult = ConvertFilters<T>(JsonHelper.ToObject<Filter[]>(item.Filters), param, item.Operation);
                result = result.Or(gresult);
            }
            return result;
        }
        /// <summary>
        /// 转换Filter数组为表达式
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="param"></param>
        /// <param name="operation"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Expression ConvertFilters<T>(Filter[] filters, ParameterExpression param, string operation)
        {
            if (filters == null || !filters.Any())
            {
                return null;
            }

            Expression result = param.GenerateBody<T>(filters[0]);

            if (filters.Length == 1)
            {
                return result;
            }

            if (operation == "and")
            {
                foreach (var filter in filters.Skip(1))
                {
                    result = result.AndAlso(param.GenerateBody<T>(filter));
                }
            }
            else
            {
                foreach (var filter in filters.Skip(1))
                {
                    result = result.Or(param.GenerateBody<T>(filter));
                }
            }

            return result;
        }
        #endregion

        #region SoulPage 拼接
        /// <summary>
        /// 创建linq表达示的body部分
        /// </summary>
        public static Expression GenerateBody<T>(this ParameterExpression param, FilterSo filterObj)
        {
            PropertyInfo property = typeof(T).GetProperty(filterObj.field);

            Expression left = null; //组装左边
            //组装右边
            Expression right = null;
            if (property != null)
            {
                left = Expression.Property(param, property);
                if (property.PropertyType == typeof(int))
                {
                    int i;
                    try
                    {
                        i = int.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = -99999;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<int>))
                {
                    int? i;
                    try
                    {
                        i = int.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.value) ? null : i, typeof(int?));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime i;
                    try
                    {
                        i = DateTime.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = DateTime.Now.AddYears(-100);
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<DateTime>))
                {
                    DateTime? i;
                    try
                    {
                        i = DateTime.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.value) ? null : i, typeof(DateTime?));
                }
                else if (property.PropertyType == typeof(string))
                {
                    right = Expression.Constant(filterObj.value);
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    decimal i;
                    try
                    {
                        i = decimal.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = -99999;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<decimal>))
                {
                    decimal? i;
                    try
                    {
                        i = decimal.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.value) ? null : i, typeof(decimal?));
                }
                else if (property.PropertyType == typeof(float))
                {
                    float i;
                    try
                    {
                        i = float.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = -99999;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<float>))
                {
                    float? i;
                    try
                    {
                        i = float.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.value) ? null : i, typeof(float?));
                }
                else if (property.PropertyType == typeof(bool))
                {
                    bool i = false;
                    if (filterObj.value.ToString().ToLower() == "1" || filterObj.value.ToString().ToLower() == "true")
                    {
                        i = true;
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Nullable<bool>))
                {
                    bool i = false;
                    if (filterObj.value.ToString().ToLower() == "1" || filterObj.value.ToString().ToLower() == "true")
                    {
                        i = true;
                    }
                    right = Expression.Constant(i, typeof(bool?));
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    Guid i;
                    try
                    {
                        i = Guid.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = new Guid();
                    }
                    right = Expression.Constant(i);
                }
                else if (property.PropertyType == typeof(Guid?))
                {
                    Guid? i;
                    try
                    {
                        i = Guid.Parse(filterObj.value);
                    }
                    catch (Exception)
                    {
                        i = null;
                    }
                    right = Expression.Constant(string.IsNullOrEmpty(filterObj.value) ? null : i, typeof(Guid?));
                }
                else
                {
                    throw new Exception("暂不能解析该Key的类型");
                }
            }
            else //如果左边不是属性，直接是值的情况
            {
                throw new Exception("暂不能解析该Key的类型");
            }
            Expression filter = Expression.Equal(left, right);
            switch (filterObj.type)
            {
                case "eq":
                    filter = Expression.Equal(left, right);
                    break;
                case "ne":
                    filter = Expression.NotEqual(left, right);
                    break;
                case "gt":
                    try
                    {
                        filter = Expression.GreaterThan(left, right);
                    }
                    catch (Exception)
                    {
                        filter = Expression.NotEqual(left, right);
                    }
                    break;
                case "ge":
                    try
                    {
                        filter = Expression.GreaterThanOrEqual(left, right);
                    }
                    catch (Exception)
                    {
                        filter = Expression.Equal(left, right);
                    }
                    break;
                case "lt":
                    try
                    {
                        filter = Expression.LessThan(left, right);
                    }
                    catch (Exception)
                    {
                        filter = Expression.NotEqual(left, right);
                    }
                    break;
                case "le":
                    try
                    {
                        filter = Expression.LessThanOrEqual(left, right);
                    }
                    catch (Exception)
                    {
                        filter = Expression.Equal(left, right);
                    }
                    break;
                case "contain":
                    try
                    {
                        filter = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                    Expression.Constant(filterObj.value));
                    }
                    catch (Exception)
                    {
                        filter = Expression.Equal(left, right);
                    }
                    break;
                case "notContain":
                    try
                    {
                        filter = Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                    Expression.Constant(filterObj.value)));
                    }
                    catch (Exception)
                    {
                        filter = Expression.NotEqual(left, right);
                    }
                    break;
                case "start":
                    try
                    {
                        filter = Expression.Call(left, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
                                Expression.Constant(filterObj.value));
                    }
                    catch (Exception)
                    {
                        filter = Expression.Equal(left, right);
                    }
                    break;
                case "end":
                    try
                    {
                        filter = Expression.Call(left, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }),
                                    Expression.Constant(filterObj.value));
                    }
                    catch (Exception)
                    {
                        filter = Expression.Equal(left, right);
                    }
                    break;
                case "null":
                    filter = Expression.Equal(left, Expression.Constant(null));
                    break;
                case "notNull":
                    filter = Expression.NotEqual(left, Expression.Constant(null));
                    break;
                default: break;
            }

            return filter;
        }
        /// <summary>
        /// 转换FilterGroup为Lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parametername"></param>
        /// <param name="filterList"></param>
        /// <returns></returns>
        public static IQuery<T> GenerateFilter<T>(this IQuery<T> query, string parametername,
            List<FilterSo> filterList)
        {
            var param = ExtLinq.CreateLambdaParam<T>(parametername);
            Expression result = ConvertList<T>(filterList, param);
            if (result == null)
            {
                return query;
            }
            query = query.Where(param.GenerateTypeLambda<T>(result));
            return query;
        }
        /// <summary>
        /// 转换filterlist为表达式
        /// </summary>
        /// <param name="filterList"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression ConvertList<T>(List<FilterSo> filterList, ParameterExpression param)
        {
            if (filterList == null) return null;
            Expression result = null;
            Expression gresult = null;
            PropertyInfo property;
            Expression left = null;
            foreach (var item in filterList)
            {
                switch (item.mode)
                {
                    case "condition":
                        gresult = param.GenerateBody<T>(item);
                        if (gresult == null)
                        {
                            break;
                        }
                        if (item.prefix == "or")
                        {
                            if (result != null)
                            {
                                result = result.Or(gresult);
                            }
                            else
                            {
                                result = gresult;
                            }
                        }
                        else
                        {
                            if (result != null)
                            {
                                result = result.AndAlso(gresult);
                            }
                            else
                            {
                                result = gresult;
                            }
                        }
                        break;
                    case "group":
                        gresult = ConvertList<T>(item.children, param);
                        if (gresult == null)
                        {
                            break;
                        }
                        if (item.prefix == "or")
                        {
                            if (result != null)
                            {
                                result = result.Or(gresult);
                            }
                            else
                            {
                                result = gresult;
                            }
                        }
                        else
                        {
                            if (result != null)
                            {
                                result = result.AndAlso(gresult);
                            }
                            else
                            {
                                result = gresult;
                            }
                        }
                        break;
                    case "in":
                        property = typeof(T).GetProperty(item.field);
                        left = Expression.Property(param, property);
                        if (item.values == null || item.values.Count == 0)
                        {
                            break;
                        }
                        if (property.PropertyType == typeof(int))
                        {
                            List<int> list = new List<int>();
                            foreach (var temp in item.values)
                            {
                                list.Add(int.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<int>).GetMethod("Contains", new Type[] { typeof(int) }), left);
                        }
                        if (property.PropertyType == typeof(Nullable<int>))
                        {
                            List<int?> list = new List<int?>();
                            foreach (var temp in item.values)
                            {
                                list.Add(int.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<int?>).GetMethod("Contains", new Type[] { typeof(int?) }), left);
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            List<DateTime> list = new List<DateTime>();
                            foreach (var temp in item.values)
                            {
                                list.Add(DateTime.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<DateTime>).GetMethod("Contains", new Type[] { typeof(DateTime) }), left);
                        }
                        else if (property.PropertyType == typeof(Nullable<DateTime>))
                        {
                            List<DateTime?> list = new List<DateTime?>();
                            foreach (var temp in item.values)
                            {
                                list.Add(DateTime.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<DateTime?>).GetMethod("Contains", new Type[] { typeof(DateTime?) }), left);
                        }
                        else if (property.PropertyType == typeof(decimal))
                        {
                            List<decimal> list = new List<decimal>();
                            foreach (var temp in item.values)
                            {
                                list.Add(decimal.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<decimal>).GetMethod("Contains", new Type[] { typeof(decimal) }), left);
                        }
                        else if (property.PropertyType == typeof(Nullable<decimal>))
                        {
                            List<decimal?> list = new List<decimal?>();
                            foreach (var temp in item.values)
                            {
                                list.Add(decimal.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<decimal?>).GetMethod("Contains", new Type[] { typeof(decimal?) }), left);
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            List<bool> list = new List<bool>();
                            foreach (var temp in item.values)
                            {
                                if (temp == "1")
                                {
                                    list.Add(true);
                                }
                                else
                                {
                                    list.Add(false);
                                }
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<bool>).GetMethod("Contains", new Type[] { typeof(bool) }), left);
                        }
                        else if (property.PropertyType == typeof(Nullable<bool>))
                        {
                            List<bool?> list = new List<bool?>();
                            foreach (var temp in item.values)
                            {
                                if (temp == "1")
                                {
                                    list.Add(true);
                                }
                                else
                                {
                                    list.Add(false);
                                }
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<bool?>).GetMethod("Contains", new Type[] { typeof(bool?) }), left);
                        }
                        else if (property.PropertyType == typeof(Nullable<float>))
                        {
                            List<float?> list = new List<float?>();
                            foreach (var temp in item.values)
                            {
                                list.Add(float.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<float?>).GetMethod("Contains", new Type[] { typeof(float?) }), left);
                        }
                        else if (property.PropertyType == typeof(float))
                        {
                            List<float> list = new List<float>();
                            foreach (var temp in item.values)
                            {
                                list.Add(float.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<float>).GetMethod("Contains", new Type[] { typeof(float) }), left);
                        }
                        else if (property.PropertyType == typeof(Nullable<double>))
                        {
                            List<double?> list = new List<double?>();
                            foreach (var temp in item.values)
                            {
                                list.Add(float.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<double?>).GetMethod("Contains", new Type[] { typeof(double?) }), left);
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            List<double> list = new List<double>();
                            foreach (var temp in item.values)
                            {
                                list.Add(float.Parse(temp));
                            }
                            gresult = Expression.Call(Expression.Constant(list), typeof(List<double>).GetMethod("Contains", new Type[] { typeof(double) }), left);
                        }
                        else
                        {
                            gresult = Expression.Call(Expression.Constant(item.values), typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) }), left);
                        }
                        if (result != null)
                        {
                            result = result.AndAlso(gresult);
                        }
                        else
                        {
                            result = gresult;
                        }
                        break;
                    case "date":
                        property = typeof(T).GetProperty(item.field);
                        left = Expression.Property(param, property);
                        bool isNull = false;
                        if (property.PropertyType == typeof(Nullable<DateTime>))
                        {
                            isNull = true;
                        }
                        DateTime? startTime = null;
                        DateTime? endTime = null;
                        switch (item.type)
                        {
                            case "yesterday":
                                startTime = DateTime.Now.Date.AddDays(-1);
                                endTime = DateTime.Now.Date;
                                break;
                            case "thisWeek":
                                startTime = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.Date.DayOfWeek.ToString("d")));  //本周周一  
                                endTime = ((DateTime)startTime).AddDays(7);  //本周周日  
                                break;
                            case "lastWeek":
                                startTime = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.Date.DayOfWeek.ToString("d")) - 7);  //上周周一  
                                endTime = ((DateTime)startTime).AddDays(7);  //上周周日  
                                break;
                            case "thisMonth":
                                startTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Date.Day);  //本月月初  
                                endTime = ((DateTime)startTime).AddMonths(1);  //本月月末  
                                break;
                            case "thisYear":
                                startTime = new DateTime(DateTime.Now.Date.Year, 1, 1); //本年年初  
                                endTime = new DateTime(DateTime.Now.Date.AddYears(1).Year, 1, 1); //本年年初
                                break;
                            case "specific":
                                var tempTime = DateTime.Parse(item.value);
                                startTime = tempTime.Date;
                                endTime = tempTime.Date.AddDays(+1);
                                break;
                            default: break;
                        }
                        if (startTime != null && endTime != null)
                        {
                            if (isNull)
                            {
                                gresult = Expression.GreaterThanOrEqual(left, Expression.Constant(startTime, typeof(DateTime?)));
                                gresult = gresult.And(Expression.LessThan(left, Expression.Constant(endTime, typeof(DateTime?))));
                            }
                            else
                            {
                                gresult = Expression.GreaterThanOrEqual(left, Expression.Constant((DateTime)startTime));
                                gresult = gresult.And(Expression.LessThan(left, Expression.Constant((DateTime)endTime)));
                            }
                        }
                        if (result != null)
                        {
                            result = result.AndAlso(gresult);
                        }
                        else
                        {
                            result = gresult;
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        #endregion
    }
}
