/*******************************************************************************
 * Copyright © 2016 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/

namespace WaterCloud.Code
{
    /// <summary>
    /// 常用返回结果
    /// </summary>
    public class AlwaysResult
    {
        /// <summary>
        /// 操作结果类型
        /// </summary>
        public object state { get; set; }
        /// <summary>
        /// 获取 消息内容
        /// </summary>
        public string message { get; set; }

    }
    /// <summary>
    /// 常用返回结果
    /// </summary>
    public class AlwaysResult<T>: AlwaysResult
    {
        /// <summary>
        /// 列表的记录数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 获取 返回数据
        /// </summary>
        public T data { get; set; }
    }
    /// <summary>
    /// DTree返回结果
    /// </summary>
    public class DTreeResult
    {
        /// <summary>
        /// 操作结果类型
        /// </summary>
        public StatusInfo status { get; set; }
        /// 此页要显示的记录列表
        /// </summary>
        public object data { get; set; }
    }
    public class StatusInfo
    {
        /// <summary>
        /// 操作结果类型
        /// </summary>
        public object code { get; set; }
        /// 此页要显示的记录列表
        /// </summary>
        public object message { get; set; }
    }
    /// <summary>
    /// 表示 ajax 操作结果类型的枚举
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 消息结果类型
        /// </summary>
        info,
        /// <summary>
        /// 成功结果类型
        /// </summary>
        success,
        /// <summary>
        /// 警告结果类型
        /// </summary>
        warning,
        /// <summary>
        /// 异常结果类型
        /// </summary>
        error
    }
}
