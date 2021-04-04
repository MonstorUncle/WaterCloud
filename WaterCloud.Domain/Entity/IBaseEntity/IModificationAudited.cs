/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using System;

namespace WaterCloud.Domain
{
    public interface IModificationAudited
    {
        string F_Id { get; set; }
        string F_LastModifyUserId { get; set; }
        DateTime? F_LastModifyTime { get; set; }
    }
}