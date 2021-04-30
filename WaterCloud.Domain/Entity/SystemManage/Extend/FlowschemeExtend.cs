using System.Collections.Generic;

namespace WaterCloud.Domain.SystemManage
{
    public class FlowschemeExtend : FlowschemeEntity
    {
        /// 用户显示
        /// </summary>
        public string F_WebId { get; set; }
        public string F_FrmContentData { get; set; }
        public string F_FrmContent { get; set; }
        /// <summary>
        /// 如果下个执行节点是运行时指定执行者。需要传指定的类型
        /// <para>取值为RUNTIME_SPECIAL_ROLE、RUNTIME_SPECIAL_USER</para>
        /// </summary>
        public string NextNodeDesignateType { get; set; }

        /// <summary>
        /// 如果下个执行节点是运行时指定执行者。该值表示具体的执行者
        /// <para>如果NodeDesignateType为RUNTIME_SPECIAL_ROLE，则该值为指定的角色</para>
        /// <para>如果NodeDesignateType为RUNTIME_SPECIAL_USER，则该值为指定的用户</para>
        /// </summary>
        public string[] NextNodeDesignates { get; set; }
        public string NextMakerName { get; set; }
    }
}
