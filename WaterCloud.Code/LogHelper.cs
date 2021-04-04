using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WaterCloud.Code
{
    public class LogHelper
    {
        private static object lockHelper = new object();

        #region 写文本日志
        /// <summary>
        /// 写日志 异步
        /// 默认路径：根目录/Log/yyyy-MM/
        /// 默认文件：yyyy-MM-dd.log
        /// </summary>
        /// <param name="logContent">日志内容 自动附加时间</param>
        public static void Write(string logContent)
        {
            string logPath = DateTime.Now.ToString("yyyy-MM");
            Write(logPath, logContent);
        }

        public static void WriteWithTime(string logContent)
        {
            string logPath = DateTime.Now.ToString("yyyy-MM");
            logContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + logContent;
            Write(logPath, logContent);
        }
        #endregion

        #region 写异常日志
        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Write(Exception ex)
        {
            string logContent = string.Empty;
            string logPath = DateTime.Now.ToString("yyyy-MM");
            logContent += GetExceptionMessage(ex);
            Write(logPath, logContent);
        }

        public static void WriteWithTime(Exception ex)
        {
            string logContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
            string logPath = DateTime.Now.ToString("yyyy-MM");
            logContent += GetExceptionMessage(ex);
            Write(logPath, logContent);
        }
        public static void WriteWithTime(ExceptionContext ex)
        {
            if (ex == null)
                return;
            string logContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
            logContent += "程序异常" + Environment.NewLine;
            string logPath = DateTime.Now.ToString("yyyy-MM");
            Exception Error = ex.Exception;
            LogMessage logMessage = new LogMessage();
            logMessage.OperationTime = DateTime.Now;
            logMessage.Url = ex.HttpContext.Request.GetDisplayUrl();
            if (ex.ActionDescriptor!=null)
            {
                logMessage.Class = ex.ActionDescriptor.DisplayName;
            }
            else
            {
                logMessage.Class = "服务器配置问题";
            }
            logMessage.Ip = WebHelper.Ip;
            logMessage.Host = ex.HttpContext.Request.Host.ToString();
            var current = OperatorProvider.Provider.GetCurrent();
            if (current != null)
            {
                logMessage.UserName = current.UserCode + "（" + current.UserName + "）";
            }
            var err= Error.GetOriginalException();
            logMessage.ExceptionInfo = err.Message;
            logMessage.ExceptionSource = err.Source;
            logMessage.ExceptionRemark = err.StackTrace;
            logContent+= ExceptionFormat(logMessage);
            Write(logPath, logContent);
        }
        private static string GetExceptionMessage(Exception ex)
        {
            string message = string.Empty;
            if (ex != null)
            {
                message += ex.Message;
                message += Environment.NewLine;
                Exception originalException = ex.GetOriginalException();
                if (originalException != null)
                {
                    if (originalException.Message != ex.Message)
                    {
                        message += originalException.Message;
                        message += Environment.NewLine;
                    }
                }
                message += ex.StackTrace;
                message += Environment.NewLine;
            }
            return message;
        }
        #endregion

        #region 写日志到指定路径
        /// <summary>
        /// 写日志 异步
        /// 默认文件：yyyy-MM-dd.log
        /// </summary>
        /// <param name="logPath">日志目录[默认程序根目录\Log\下添加，故使用相对路径，如：营销任务]</param>
        /// <param name="logContent">日志内容 自动附加时间</param>
        public static void Write(string logPath, string logContent)
        {
            string logFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            Write(logPath, logFileName, logContent);
        }

        /// <summary>
        /// 写日志 异步
        /// </summary>
        /// <param name="logPath">日志目录</param>
        /// <param name="logFileName">日志文件名</param>
        /// <param name="logContent">日志内容 自动附加时间</param>
        public static void Write(string logPath, string logFileName, string logContent)
        {
            if (string.IsNullOrWhiteSpace(logContent))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(logPath))
            {
                logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", DateTime.Now.ToString("yyyy-MM"));
            }
            else
            {
                logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", logPath.Trim('\\'));
            }
            if (string.IsNullOrWhiteSpace(logFileName))
            {
                logFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            }
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string fileName = Path.Combine(logPath, logFileName);
            Action taskAction = () =>
            {
                lock (lockHelper)
                {
                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(logContent + Environment.NewLine);
                        sw.Flush();
                        sw.Close();
                    }
                }
            };
            Task task = new Task(taskAction);
            task.Start();
        }
        /// <summary>
        /// 生成异常信息
        /// </summary>
        /// <param name="logMessage">对象</param>
        /// <returns></returns>
        public static string ExceptionFormat(LogMessage logMessage)
        {
            StringBuilder strInfo = new StringBuilder();
            strInfo.Append("1. 调试: >> 操作时间: " + logMessage.OperationTime + "   操作人: " + logMessage.UserName + " \r\n");
            strInfo.Append("2. 地址: " + logMessage.Url + "    \r\n");
            strInfo.Append("3. 类名: " + logMessage.Class + " \r\n");
            strInfo.Append("4. 主机: " + logMessage.Host + "   Ip  : " + logMessage.Ip + " \r\n");
            strInfo.Append("5. 异常: " + logMessage.ExceptionInfo + "\r\n");
            strInfo.Append("6. 来源: " + logMessage.ExceptionSource + "\r\n");
            strInfo.Append("7. 实例: " + logMessage.ExceptionRemark + "\r\n");
            strInfo.Append("-----------------------------------------------------------------------------------------------------------------------------\r\n");
            return strInfo.ToString();
        }
        /// <summary>
        /// 格式化异常信息
        /// </summary>
        /// <param name="logMessage">对象</param>
        /// <returns></returns>
        public static string ExMsgFormat(string message)
        {
            //数据库异常
            if (message.Contains("An exception occurred while executing DbCommand."))
            {
                if (message.Contains("Duplicate entry '")&& message.Contains("key"))
                {
                    message = "数据违反唯一约束，请检查";
                }
                else if (message.Contains("Data too long for column"))
                {

                    message = "数据长度过长，请检查";
                }
                else
                {
                    message = "数据操作异常，请联系管理员";
                }
            }
            //其他异常
            else
            {
                if (message.Contains("Object reference not set to an instance of an object."))
                {
                    message="操作对象为空，请联系管理员";
                }
                else if (message.Contains("Value cannot be null"))
                {
                    message = "值为空，请联系管理员";
                }
            }
            if (!IsHasCHZN(message))
            {
                message = "程序内部异常，请联系管理员";
            }
            return message;
        }
        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private static bool IsHasCHZN(string inputData)
        {

            Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }
        #endregion
    }
}
