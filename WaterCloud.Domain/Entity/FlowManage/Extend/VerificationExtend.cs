using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterCloud.Domain.FlowManage
{
    public class VerificationExtend:NodeDesignateEntity
    {
        public string F_FlowInstanceId { get; set; }
        /// <summary>
        /// 1:同意；2：不同意；3：驳回
        /// </summary>
        public string F_VerificationFinally { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string F_VerificationOpinion { get; set; }

        /// <summary>
        /// 驳回的步骤，即驳回到的节点ID
        /// </summary>
        public string NodeRejectStep { get; set; }

        /// <summary>
        /// 驳回类型。null:使用节点配置的驳回类型/0:前一步/1:第一步/2：指定节点，使用NodeRejectStep
        /// </summary>
        public string NodeRejectType { get; set; }
    }
}
