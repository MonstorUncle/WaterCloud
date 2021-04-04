/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using Chloe.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace WaterCloud.Domain.SystemManage
{
    [TableAttribute("sys_module")]
    public class ModuleEntity : IEntity<ModuleEntity>, ICreationAudited, IModificationAudited, IDeleteAudited
    {
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        [Required(ErrorMessage = "上级不能为空")]
        public string F_ParentId { get; set; }
        public int? F_Layers { get; set; }
        [Required(ErrorMessage = "编号不能为空")]
        public string F_EnCode { get; set; }
        [Required(ErrorMessage = "名称不能为空")]
        public string F_FullName { get; set; }
        public string F_Icon { get; set; }
        public string F_UrlAddress { get; set; }
        [Required(ErrorMessage = "目标不能为空")]
        public string F_Target { get; set; }
        public bool? F_IsMenu { get; set; }
        public bool? F_IsExpand { get; set; }
        public bool? F_IsPublic { get; set; }
        public bool? F_IsFields { get; set; }
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
        public string F_Authorize { get; set; }
    }
}
