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
    public class DatabaseTableOracleService : RepositoryBase, IDatabaseTableService
    {
        public DatabaseTableOracleService(IDbContext context) : base(context)
        {

        }
        #region 获取数据
        public async Task<List<TableInfo>> GetTableList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            //select TABLE_NAME Id,TABLE_NAME from user_tab_comments utc where utc.table_type='TABLE'
            strSql.Append(@"select a.TABLE_NAME TableName,b.CREATED CreateTime from sys.user_tables a left join user_objects b on b.object_name=upper(a.TABLE_NAME) where a.table_name not like '%$%' and a.table_name not like '%LOGMNR%'");
            IEnumerable<TableInfo> list = await FindList<TableInfo>(strSql.ToString());
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
            strSql.Append(@"select a.TABLE_NAME TableName,b.CREATED CreateTime from sys.user_tables a left join user_objects b on b.object_name=upper(a.TABLE_NAME) where a.table_name not like '%$%' and a.table_name not like '%LOGMNR%'");
            //select a.TABLE_NAME TableName,b.CREATED CreateTime from sys.user_tables a,user_objects b where b.object_name=upper(a.TABLE_NAME) and a.table_name not like '%$%' and a.table_name not like '%LOGMNR%'
            if (!tableName.IsEmpty())
            {
                strSql.Append(" AND TABLE_NAME like :TableName ");
                parameter.Add(new DbParam(":TableName", '%' + tableName + '%'));
            }

            IEnumerable<TableInfo> list = await FindList<TableInfo>(strSql.ToString(), parameter.ToArray());
            pagination.records = list.Count();
            var tempData = list.OrderByDescending(a => a.CreateTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable().ToList();
            await SetTableDetail(tempData);
            return tempData;
        }

        /// <summary>
        /// 获取表的字段信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public async Task<List<TableFieldInfo>> GetTableFieldList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  
                                  b.column_name TableColumn,
                                  (CASE WHEN c.COLUMN_NAME=b.column_name THEN 'Y' ELSE '' END) TableIdentity,  
                                   b.data_type Datatype,  
                                   b.data_length FieldLength,   
                                   b.NULLABLE IsNullable,   
                                   '' FieldDefault,
                                   a.comments Remark
        FROM all_col_comments a, user_tab_columns b  
        LEFT JOIN user_cons_columns c on b.TABLE_NAME=c.TABLE_NAME
        WHERE a.table_name = b.table_name and a.Column_name = b.Column_name and a.table_name =:TableName
        and c.constraint_name in (select constraint_name from user_constraints where  constraint_type='P' and a.table_name =:TableName)
         ORDER BY COLUMN_ID ASC");

            var parameter = new List<DbParam>();
            parameter.Add(new DbParam(":TableName", tableName));
            var list = await FindList<TableFieldInfo>(strSql.ToString(), parameter.ToArray());
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

            //--P-主键；R-外键；U-唯一约束
            string strSql = @"Select  b.Constraint_Name Id,a.table_name TableName, b.Column_Name TableKey,b.Constraint_Name TableKeyName,0 TableCount,c.CREATED CreateTime
                                     From user_Constraints a,user_Cons_Columns b left join user_objects c on c.object_name=upper(b.table_name)
                                     WHERE a.Constraint_Type = 'P'
                                     and a.Constraint_Name = b.Constraint_Name 　　
　　                                  And a.Owner = b.Owner 　　
　　                                  And a.table_name = b.table_name AND a.table_name not like '%$%' and a.table_name not like '%LOGMNR%'";

            IEnumerable<TableInfo> list = await FindList<TableInfo>(strSql.ToString());
            return list.ToList();
        }

        /// <summary>
        /// 赋值表的主键、主键名称、记录数
        /// </summary>
        /// <param name="list"></param>
        private async Task SetTableDetail(IEnumerable<TableInfo> list)
        {
            List<TableInfo> detailList = await GetTableDetailList();
            foreach (TableInfo table in list)
            {
                table.TableKey = string.Join(",", detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableKey));
                table.TableKeyName = detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableKeyName).FirstOrDefault();
                table.TableCount = detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableCount).FirstOrDefault();
                table.CreateTime = detailList.Where(p => p.TableName == table.TableName).Select(p => p.CreateTime).FirstOrDefault();
            }
        }
        #endregion
    }
}
