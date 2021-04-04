using Chloe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.DataBase;
using WaterCloud.DataBase.Extensions;
using WaterCloud.Domain;

namespace WaterCloud.Service.CommonService
{
    public class DatabaseTableSqlServerService : RepositoryBase, IDatabaseTableService
    {
        public DatabaseTableSqlServerService(IDbContext context) : base(context)
        {

        }
        #region 获取数据
        public async Task<List<TableInfo>> GetTableList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT id Id,name TableName FROM sysobjects WHERE xtype = 'u' order by name");
            IEnumerable<TableInfo> list =await FindList<TableInfo>(strSql.ToString());
            if (!tableName.IsEmpty())
            {
                list = list.Where(p => p.TableName.Contains(tableName));
            }
            await SetTableDetail(list);
            return list.ToList();
        }

        public async Task<List<TableInfo>> GetTablePageList(string tableName, Pagination pagination)
        {
            StringBuilder strSql = new StringBuilder();
            var parameter = new List<DbParam>();
            strSql.Append(@"SELECT id Id,name TableName,crdate CreateTime FROM sysobjects WHERE xtype = 'u'");

            if (!tableName.IsEmpty())
            {
                strSql.Append(" AND name like @TableName ");
                parameter.Add(new DbParam("@TableName", '%' + tableName + '%'));
            }

            IEnumerable<TableInfo> list =await FindList<TableInfo>(strSql.ToString(), parameter.ToArray());
            pagination.records = list.Count();
            var tempData = list.OrderByDescending(a=>a.CreateTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable().ToList();
            await SetTableDetail(tempData);
            return tempData;
        }

        public async Task<List<TableFieldInfo>> GetTableFieldList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  
                                  TableColumn = rtrim(b.name),  
                                  TableIdentity = CASE WHEN h.id IS NOT NULL  THEN 'Y' ELSE '' END,  
                                  Datatype = type_name(b.xusertype)+CASE WHEN b.colstat&1=1 THEN '[ID(' + CONVERT(varchar, ident_seed(a.name))+','+CONVERT(varchar,ident_incr(a.name))+')]' ELSE '' END,  
                                  FieldLength = b.length,   
                                  IsNullable = CASE b.isnullable WHEN 0 THEN 'N' ELSE 'Y' END,   
                                  FieldDefault = ISNULL(e.text, ''),
                                  Remark = (SELECT ep.value FROM sys.columns sc LEFT JOIN sys.extended_properties ep ON ep.major_id = sc.object_id AND ep.minor_id = sc.column_id
										                    WHERE sc.object_id = a.id AND sc.name = b.name)
                            FROM sysobjects a, syscolumns b  
                            LEFT OUTER JOIN syscomments e ON b.cdefault = e.id  
                            LEFT OUTER JOIN (Select g.id, g.colid FROM sysindexes f, sysindexkeys g Where (f.id=g.id)AND(f.indid=g.indid)AND(f.indid>0)AND(f.indid<255)AND(f.status&2048)<>0) h ON (b.id=h.id)AND(b.colid=h.colid)  
                            Where (a.id=b.id)AND(a.id=object_id(@TableName))   
                                  ORDER BY b.colid");
            var parameter = new List<DbParam>();
            parameter.Add(new DbParam("@TableName", tableName));
            var list =await FindList<TableFieldInfo>(strSql.ToString(), parameter.ToArray());
            return list.ToList();
        }
        #endregion

        #region 公有方法
        public async Task<bool> DatabaseBackup(string backupPath)
        {
            string database = HtmlHelper.Resove(GlobalContext.SystemConfig.DBConnectionString, "Initial Catalog = ", ";");
            var result = DbHelper.ExecuteSqlCommand(database, backupPath);
            return result > 0 ? true : false;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取所有表的主键、主键名称、记录数
        /// </summary>
        /// <returns></returns>
        private async Task<List<TableInfo>> GetTableDetailList()
        {
            string strSql = @"SELECT syscolumns.id Id,syscolumns.name TableKey,sysobjects.name TableKeyName,sysindexes.rows TableCount,sysobjects.crdate CreateTime
                                     FROM syscolumns,sysobjects,sysindexes,sysindexkeys 
                                     WHERE sysobjects.xtype = 'PK' 
                                           AND sysobjects.parent_obj = syscolumns.id 
                                           AND sysindexes.id = syscolumns.id 
                                           AND sysobjects.name = sysindexes.name AND sysindexkeys.id = syscolumns.id 
                                           AND sysindexkeys.indid = sysindexes.indid 
                                           AND syscolumns.colid = sysindexkeys.colid";

            IEnumerable<TableInfo> list =await FindList<TableInfo>(strSql.ToString());
            return list.ToList();
        }

        /// <summary>
        /// 赋值表的主键、主键名称、记录数
        /// </summary>
        /// <param name="list"></param>
        private async Task SetTableDetail(IEnumerable<TableInfo> list)
        {
            List<TableInfo> detailList =await GetTableDetailList();
            foreach (TableInfo table in list)
            {
                table.TableKey = string.Join(",", detailList.Where(p => p.Id == table.Id).Select(p => p.TableKey));
                table.TableKeyName = detailList.Where(p => p.Id == table.Id).Select(p => p.TableKeyName).FirstOrDefault();
                table.TableCount = detailList.Where(p => p.Id == table.Id).Select(p => p.TableCount).FirstOrDefault();
                table.CreateTime = detailList.Where(p => p.Id == table.Id).Select(p => p.CreateTime).FirstOrDefault();
            }
        }
        #endregion
    }
}
