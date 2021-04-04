using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WaterCloud.Code
{
    public class ReflectionHelper
    {
        private static ConcurrentDictionary<string, object> dictCache = new ConcurrentDictionary<string, object>();

        #region 得到类里面的属性集合
        /// <summary>
        /// 得到类里面的属性集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type, string[] columns = null)
        {
            PropertyInfo[] properties = null;
            if (dictCache.ContainsKey(type.FullName))
            {
                properties = dictCache[type.FullName] as PropertyInfo[];

            }
            else
            {
                properties = type.GetProperties();
                dictCache.TryAdd(type.FullName, properties);
            }

            if (columns != null && columns.Length > 0)
            {
                //  按columns顺序返回属性
                var columnPropertyList = new List<PropertyInfo>();
                foreach (var column in columns)
                {
                    var columnProperty = properties.Where(p => p.Name == column).FirstOrDefault();
                    if (columnProperty != null)
                    {
                        columnPropertyList.Add(columnProperty);
                    }
                }
                return columnPropertyList.ToArray();
            }
            else
            {
                return properties;
            }
        }

        public static object GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(propertyname);
            if (property == null) return string.Empty;
            object o = property.GetValue(t, null);
            if (o == null) return string.Empty;
            return o;
        }
        #endregion

        /// <summary>
        /// StackTrace获取模块名(此方法为上上层调用)
        /// </summary>
        /// <returns></returns>
        public static string GetModuleName(int count = 2)
        {
            try
            {
                if (GlobalContext.SystemConfig.Debug == true && count == 2)
                {
                    count++;
                }
                string className = new StackFrame(count, true).GetMethod().DeclaringType.FullName;
                className = className.Split('+')[0];
                className = className.Split('.').LastOrDefault();
                string moduleName = className.Substring(0, className.Length - 7);
                return moduleName;
            }
            catch (Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                return "";
            }
        }
        /// <summary>
        /// StackTrace获取方法名(此方法为上上上上层调用)
        /// </summary>
        /// <returns></returns>
        public static string GetClassName(int count = 4)
        {
            try
            {
                if (GlobalContext.SystemConfig.Debug == true && count == 4)
                {
                    count++;
                }
                string className = new StackFrame(count, true).GetMethod().DeclaringType.FullName;
                className = className.Split('+')[0];
                className = className.Split('.').LastOrDefault();
                return className;
            }
            catch (Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                return "";
            }
        }
    }
}
