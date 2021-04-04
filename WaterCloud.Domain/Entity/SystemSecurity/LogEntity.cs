/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using Chloe.Annotations;
using System;

namespace WaterCloud.Domain.SystemSecurity
{
    [TableAttribute("sys_log")]

    public class LogEntity : IEntity<LogEntity>, ICreationAudited
    {
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        public DateTime? F_Date { get; set; }
        public string F_Account { get; set; }
        public string F_NickName { get; set; }
        public string F_Type { get; set; }
        public string F_IPAddress { get; set; }
        public string F_IPAddressName { get; set; }
        public string F_ModuleId { get; set; }
        public string F_ModuleName { get; set; }
        public bool? F_Result { get; set; }
        public string F_Description { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public string F_KeyValue { get; set; }
        public string F_CompanyId { get; set; }
        public  LogEntity()
        {

        }
        //重载构造方法
        public LogEntity(string module,string moduleitem,string optiontype)
        {
            this.F_ModuleName = module+ moduleitem;
            this.F_Description = moduleitem+"操作,";
            this.F_Type = optiontype;
            this.F_Result = true;
        }
    }
}
