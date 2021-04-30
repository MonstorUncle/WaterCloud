using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.FlowManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-14 09:18
    /// 描 述：我的流程实体类
    /// </summary>
    [TableAttribute("oms_flowinstance")]
    public class FlowinstanceEntity : IEntity<FlowinstanceEntity>, ICreationAudited
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        /// <returns></returns>
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 流程实例模板Id
        /// </summary>
        /// <returns></returns>
        public string F_InstanceSchemeId { get; set; }
        /// <summary>
        /// 实例编号
        /// </summary>
        /// <returns></returns>
        [Required(ErrorMessage = "编号不能为空")]
        public string F_Code { get; set; }
        /// <summary>
        /// 自定义名称
        /// </summary>
        /// <returns></returns>
        public string F_CustomName { get; set; }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        /// <returns></returns>
        public string F_ActivityId { get; set; }
        /// <summary>
        /// 当前节点类型（0会签节点）
        /// </summary>
        /// <returns></returns>
        public int? F_ActivityType { get; set; }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        /// <returns></returns>
        public string F_ActivityName { get; set; }
        /// <summary>
        /// 前一个ID
        /// </summary>
        /// <returns></returns>
        public string F_PreviousId { get; set; }
        /// <summary>
        /// 流程模板内容
        /// </summary>
        /// <returns></returns>
        public string F_SchemeContent { get; set; }
        /// <summary>
        /// 流程模板ID
        /// </summary>
        /// <returns></returns>
        [Required(ErrorMessage = "模板不能为空")]
        public string F_SchemeId { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        /// <returns></returns>
        public string F_DbName { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        /// <returns></returns>
        public string F_FrmData { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        /// <returns></returns>
        public int? F_FrmType { get; set; }
        /// <summary>
        /// 表单中的字段
        /// </summary>
        /// <returns></returns>
        public string F_FrmContentData { get; set; }
        /// <summary>
        /// 表单字段（冗余)
        /// </summary>
        /// <returns></returns>
        public string F_FrmContentParse { get; set; }
        /// <summary>
        /// 表单参数
        /// </summary>
        /// <returns></returns>
        public string F_FrmContent { get; set; }
        /// <summary>
        /// 表单ID
        /// </summary>
        /// <returns></returns>
        public string F_FrmId { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        /// <returns></returns>
        public string F_SchemeType { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        /// <returns></returns>
        public int? F_FlowLevel { get; set; }
        /// <summary>
        /// 实例备注
        /// </summary>
        /// <returns></returns>
        public string F_Description { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        /// <returns></returns>
        public int F_IsFinish { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        /// <returns></returns>
        public string F_MakerList { get; set; }
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
        /// <summary>
        /// 如果下个执行节点是运行时指定执行者。需要传指定的类型
        /// <para>取值为RUNTIME_SPECIAL_ROLE、RUNTIME_SPECIAL_USER</para>
        /// </summary>
        [NotMapped]
        public string NextNodeDesignateType { get; set; }

        /// <summary>
        /// 如果下个执行节点是运行时指定执行者。该值表示具体的执行者
        /// <para>如果NodeDesignateType为RUNTIME_SPECIAL_ROLE，则该值为指定的角色</para>
        /// <para>如果NodeDesignateType为RUNTIME_SPECIAL_USER，则该值为指定的用户</para>
        /// </summary>
        [NotMapped]
        public string[] NextNodeDesignates { get; set; }
        [NotMapped]
        public string NextMakerName { get; set; }
        [NotMapped]
        public string CurrentMakerName { get; set; }
        [NotMapped]
        public string CurrentNodeDesignateType { get; set; }
    }
}
