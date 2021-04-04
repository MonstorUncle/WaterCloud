using System;
using Chloe.Annotations;

namespace WaterCloud.Domain.FileManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-22 12:04
    /// 描 述：文件管理实体类
    /// </summary>
    [TableAttribute("oms_uploadfile")]
    public class UploadfileEntity : IEntity<UploadfileEntity>,ICreationAudited
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        /// <returns></returns>
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        /// <returns></returns>
        public string F_FilePath { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        /// <returns></returns>
        public string F_FileName { get; set; }
        /// <summary>
        /// 文件类型(0文件、1图片)
        /// </summary>
        /// <returns></returns>
        public int? F_FileType { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        /// <returns></returns>
        public int? F_FileSize { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        /// <returns></returns>
        public string F_FileExtension { get; set; }
        /// <summary>
        /// 文件所属
        /// </summary>
        /// <returns></returns>
        public string F_FileBy { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public string F_Description { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        /// <returns></returns>
        public string F_OrganizeId { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        /// <returns></returns>
        public bool? F_EnabledMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserName { get; set; }
        [NotMapped]
        public string F_OrganizeName { get; set; }
    }
}
