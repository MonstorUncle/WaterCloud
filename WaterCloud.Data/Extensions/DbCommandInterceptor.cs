using Chloe.Infrastructure.Interception;
using System;
using System.Data;

namespace WaterCloud.DataBase
{
    public class DbCommandInterceptor : IDbCommandInterceptor
    {
        /* 执行 DbCommand.ExecuteReader() 时调用 */
        public void ReaderExecuting(IDbCommand command, DbCommandInterceptionContext<IDataReader> interceptionContext)
        {
            DateTime startTime = DateTime.Now;
            interceptionContext.DataBag.Add("startTime", startTime);
            Console.WriteLine(startTime.ToString());
            Console.WriteLine("SQL内容:"+command.CommandText);
        }
        /* 执行 DbCommand.ExecuteReader() 后调用 */
        public void ReaderExecuted(IDbCommand command, DbCommandInterceptionContext<IDataReader> interceptionContext)
        {
            DateTime startTime = (DateTime)(interceptionContext.DataBag["startTime"]);
            Console.WriteLine("SQL用时(毫秒):" + DateTime.Now.Subtract(startTime).TotalMilliseconds);
            if (interceptionContext.Exception == null)
                Console.WriteLine("字段个数:"+interceptionContext.Result.FieldCount);
            Console.WriteLine("-----------------------");
        }

        /* 执行 DbCommand.ExecuteNonQuery() 时调用 */
        public void NonQueryExecuting(IDbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            DateTime startTime = DateTime.Now;
            interceptionContext.DataBag.Add("startTime", startTime);
            Console.WriteLine(startTime.ToString());
            Console.WriteLine("SQL内容:" + command.CommandText);
        }
        /* 执行 DbCommand.ExecuteNonQuery() 后调用 */
        public void NonQueryExecuted(IDbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            DateTime startTime = (DateTime)(interceptionContext.DataBag["startTime"]);
            Console.WriteLine("SQL用时(毫秒):" + DateTime.Now.Subtract(startTime).TotalMilliseconds);
            if (interceptionContext.Exception == null)
                Console.WriteLine("影响行数:" + interceptionContext.Result);
            Console.WriteLine("-----------------------");
        }

        /* 执行 DbCommand.ExecuteScalar() 时调用 */
        public void ScalarExecuting(IDbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            DateTime startTime = DateTime.Now;
            interceptionContext.DataBag.Add("startTime", startTime);
            Console.WriteLine(startTime.ToString());
            Console.WriteLine("SQL内容:" + command.CommandText);
        }
        /* 执行 DbCommand.ExecuteScalar() 后调用 */
        public void ScalarExecuted(IDbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            DateTime startTime = (DateTime)(interceptionContext.DataBag["startTime"]);
            Console.WriteLine("SQL用时(毫秒):" + DateTime.Now.Subtract(startTime).TotalMilliseconds);
            if (interceptionContext.Exception == null)
                Console.WriteLine("影响行数:" + interceptionContext.Result);
            Console.WriteLine("-----------------------");
        }
    }
}
