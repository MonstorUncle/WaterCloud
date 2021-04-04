using System;
using Chloe.Annotations;

namespace WaterCloud.Domain.SystemOrganize
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-08-06 08:54
    /// 描 述：用户实体扩展类
    /// </summary>
    [TableAttribute("sys_user")]
    public class UserExtend : UserEntity
    {
        //使用导入错误信息
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public string F_Remark { get; set; }
        public string F_DutyName { get; set; }
        public string F_CompanyName { get; set; }
    }
}
