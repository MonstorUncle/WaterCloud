using Chloe;
using Chloe.MySql;
using Chloe.Oracle;
using Chloe.SqlServer;
using WaterCloud.Code;

namespace WaterCloud.DataBase
{
	public class DBContexHelper
    {
        private static string dbType = GlobalContext.SystemConfig.DBProvider;
        private static string dbConnectionString = GlobalContext.SystemConfig.DBConnectionString;
        private static string DBCommandTimeout = GlobalContext.SystemConfig.DBCommandTimeout;

        public static IDbContext Contex(string ConnectStr = "", string providerName = "")
        {
            ConnectStr = string.IsNullOrEmpty(ConnectStr) ? dbConnectionString : ConnectStr;
            providerName = string.IsNullOrEmpty(providerName) ? dbType : providerName;
            if (providerName == Define.DBTYPE_SQLSERVER)
            {
                var dbContext = new MsSqlContext(ConnectStr);
                //2012以上版本切换使用 OFFSET FETCH 分页方式
                //dbContext.PagingMode = PagingMode.OFFSET_FETCH;
                dbContext.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                return dbContext;
            }
            else if (providerName == Define.DBTYPE_MYSQL)
            {
                var dbContext = new MySqlContext(new MySqlConnectionFactory(ConnectStr));
                dbContext.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                return dbContext;
            }
            else if (providerName == Define.DBTYPE_ORACLE)
            {
                var dbContext = new OracleContext(new OracleConnectionFactory(ConnectStr));
                dbContext.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                dbContext.ConvertToUppercase = false;
                return dbContext;
            }
            else
            {
                return null;
            }
        }
    }
}
