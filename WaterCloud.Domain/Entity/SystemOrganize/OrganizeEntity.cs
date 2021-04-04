/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using Chloe.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace WaterCloud.Domain.SystemOrganize
{
    [TableAttribute("sys_organize")]
    public class OrganizeEntity : IEntity<OrganizeEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        [Required(ErrorMessage = "父级不能为空")]
        public string F_ParentId { get; set; }
        public int? F_Layers { get; set; }
        [Required(ErrorMessage = "编号不能为空")]
        public string F_EnCode { get; set; }
        [Required(ErrorMessage = "名称不能为空")]
        public string F_FullName { get; set; }
        public string F_ShortName { get; set; }
        [Required(ErrorMessage = "类型不能为空")]
        public string F_CategoryId { get; set; }
        public string F_ManagerId { get; set; }
        public string F_TelePhone { get; set; }
        public string F_MobilePhone { get; set; }
        public string F_WeChat { get; set; }
        public string F_Fax { get; set; }
        public string F_Email { get; set; }
        public string F_AreaId { get; set; }
        public string F_Address { get; set; }
        public bool? F_AllowEdit { get; set; }
        public bool? F_AllowDelete { get; set; }
        [Required(ErrorMessage = "排序不能为空")]
        [Range(0, 99999999, ErrorMessage = "排序大小必须介于1~99999999之间")]
        public int? F_SortCode { get; set; }
        public bool? F_DeleteMark { get; set; }
        public bool? F_EnabledMark { get; set; }
        public string F_Description { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_DeleteUserId { get; set; }
        [NotMapped]
        //tablecheck字段
        public bool LAY_CHECKED { get; set; }
    }
}
