using System;
using System.Collections.Generic;
using System.Text;

namespace WaterCloud.Code
{
    public class Utils
    {
        #region 自动生成编号
        /// <summary>
        /// 表示全局唯一标识符 (GUID)。
        /// </summary>
        /// <returns></returns>
        public static string GuId()
        {
            return IDGen.NextID().ToString();
        }
        /// <summary>
        /// 自动生成编号  201008251145409865
        /// </summary>
        /// <returns></returns>
        public static string CreateNo()
        {
            Random random = new Random();
            string strRandom = random.Next(1000, 10000).ToString(); //生成编号 
            string code = DateTime.Now.ToString("yyyyMMddHHmmss") + strRandom;//形如
            return code;
        }
        #endregion
        public static string GetGuid()
        {
            return IDGen.NextID().ToString().Replace("-", string.Empty).ToLower();
        }
    }
}
