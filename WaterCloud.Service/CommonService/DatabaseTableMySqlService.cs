using Chloe;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.DataBase;
using WaterCloud.DataBase.Extensions;
using WaterCloud.Domain;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Domain.SystemSecurity;

namespace WaterCloud.Service.CommonService
{
    public class DatabaseTableMySqlService : RepositoryBase, IDatabaseTableService
    {
        public DatabaseTableMySqlService(IDbContext context) : base(context)
        {

        }
        #region 获取数据
        public async Task<List<TableInfo>> GetTableList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT table_name TableName FROM information_schema.tables WHERE table_schema='" + GetDatabase() + "' AND table_type='base table'");
            IEnumerable<TableInfo> list =await FindList<TableInfo>(strSql.ToString());
            if (!string.IsNullOrEmpty(tableName))
            {
                list = list.Where(p => p.TableName.Contains(tableName)).ToList();
            }
            await SetTableDetail(list);
            return list.ToList();
        }

        public async Task<List<TableInfo>> GetTablePageList(string tableName, Pagination pagination)
        {
            StringBuilder strSql = new StringBuilder();
            var parameter = new List<DbParam>();
            strSql.Append(@"SELECT table_name TableName,CREATE_TIME CreateTime FROM information_schema.tables where table_schema='" + GetDatabase() + "' and (table_type='base table' or table_type='BASE TABLE')");

            if (!string.IsNullOrEmpty(tableName))
            {
                strSql.Append(" AND table_name like @TableName ");
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
            strSql.Append(@"SELECT COLUMN_NAME TableColumn, 
		                           DATA_TYPE Datatype,
		                           (CASE COLUMN_KEY WHEN 'PRI' THEN 'Y' ELSE '' END) TableIdentity,
		                           REPLACE(REPLACE(SUBSTRING(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)),'(',''),')','') FieldLength,
	                               (CASE IS_NULLABLE WHEN 'NO' THEN 'N' ELSE 'Y' END) IsNullable,
                                   IFNULL(COLUMN_DEFAULT,'') FieldDefault,
                                   COLUMN_COMMENT Remark
                             FROM information_schema.columns WHERE table_schema='" + GetDatabase() + "' AND table_name=@TableName");
            var parameter = new List<DbParam>();
            parameter.Add(new DbParam("@TableName", tableName));
            var list =await FindList<TableFieldInfo>(strSql.ToString(), parameter.ToArray());
            return list;
        }
        #endregion

        #region 公有方法
        public async Task<bool> DatabaseBackup(string backupPath)
        {
            string database = HtmlHelper.Resove(GlobalContext.SystemConfig.DBConnectionString.ToLower(), "database=", ";");
            //不能备份
            var result = DbHelper.ExecuteSqlCommand(database, backupPath);
            return result > 0 ? true : false;
        }

        /// <summary>
        /// 仅用在WaterCloud框架里面，同步不同数据库之间的数据，以 MySql 为主库，同步 MySql 的数据到SqlServer和Oracle，保证各个数据库的数据是一样的
        /// </summary>
        /// <returns></returns>
        public async Task SyncDatabase()
        {
            #region 同步SqlServer数据库
            await SyncSqlServerTable<ModuleEntity>();
            await SyncSqlServerTable<ModuleButtonEntity>();
            await SyncSqlServerTable<ItemsEntity>();
            await SyncSqlServerTable<ItemsDetailEntity>();
            await SyncSqlServerTable<NoticeEntity>();
            await SyncSqlServerTable<OrganizeEntity>();
            await SyncSqlServerTable<QuickModuleEntity>();
            await SyncSqlServerTable<RoleAuthorizeEntity>();
            await SyncSqlServerTable<RoleEntity>();
            await SyncSqlServerTable<LogEntity>();
            await SyncSqlServerTable<RoleEntity>();
            await SyncSqlServerTable<UserEntity>();
            await SyncSqlServerTable<UserLogOnEntity>();
            await SyncSqlServerTable<ServerStateEntity>();
            await SyncSqlServerTable<FilterIPEntity>();
            await SyncSqlServerTable<DbBackupEntity>();
            #endregion
        }
        private async Task SyncSqlServerTable<T>() where T : class, new()
        {
            string sqlServerConnectionString = "192.168.1.17;Initial Catalog = WaterCloudNetDb;User ID=sa;Password=admin@12345;MultipleActiveResultSets=true";
            var list = this.IQueryable<T>().ToList();
            var context=new RepositoryBase(sqlServerConnectionString, "System.Data.SqlClient");
            await context.Delete<T>(p => true);
            foreach (var item in list)
            {
                await context.Insert<T>(item);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取所有表的主键、主键名称、记录数
        /// </summary>
        /// <returns></returns>
        private async Task<List<TableInfo>> GetTableDetailList()
        {
            string strSql = @"SELECT t1.TABLE_NAME TableName,t1.TABLE_COMMENT Remark,t1.TABLE_ROWS TableCount,t2.CONSTRAINT_NAME TableKeyName,t2.column_name TableKey,t1.CREATE_TIME CreateTime
                                     FROM information_schema.TABLES as t1 
	                                 LEFT JOIN INFORMATION_SCHEMA.`KEY_COLUMN_USAGE` as t2 on t1.TABLE_NAME = t2.TABLE_NAME
                                     WHERE t1.TABLE_SCHEMA='" + GetDatabase() + "' AND t2.TABLE_SCHEMA='" + GetDatabase() + "'";

            IEnumerable<TableInfo> list =await  FindList<TableInfo>(strSql.ToString());
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
                table.TableKey = string.Join(",", detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableKey));
                table.TableKeyName = detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableKeyName).FirstOrDefault();
                table.TableCount = detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableCount).FirstOrDefault();
                table.CreateTime = detailList.Where(p => p.TableName == table.TableName).Select(p => p.CreateTime).FirstOrDefault();
            }
        }
        private string GetDatabase()
        {
            string database = HtmlHelper.Resove(GlobalContext.SystemConfig.DBConnectionString.ToLower(), "database=", ";");
            return database;
        }
        #endregion
    }
}
