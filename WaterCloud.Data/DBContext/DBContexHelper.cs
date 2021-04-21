using Chloe;
using Chloe.MySql;
using Chloe.Oracle;
using Chloe.SqlServer;
using System.Data.SqlClient;
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
            IDbContext context;
            switch (providerName)
            {
                case Define.DBTYPE_SQLSERVER:
                    SqlConnection.ClearAllPools();
                    context = new MsSqlContext(ConnectStr);
                    context.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                    break;
                case Define.DBTYPE_MYSQL:
                    context = new MySqlContext(new MySqlConnectionFactory(ConnectStr));
                    context.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                    break;
                case Define.DBTYPE_ORACLE:
                    var con = new OracleContext(new OracleConnectionFactory(ConnectStr));
                    con.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                    con.ConvertToUppercase = true;
                    context = con;
                    break;
                default:
                    context = new MsSqlContext(ConnectStr);
                    context.Session.CommandTimeout = int.Parse(DBCommandTimeout);
                    break;
            }
            return context;
        }
    }
}
