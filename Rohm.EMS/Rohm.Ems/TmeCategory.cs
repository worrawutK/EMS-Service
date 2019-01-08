using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rohm.Ems
{
    [DataContract()]
    public enum TmeCategory
    {
        [EnumMember()]
        PlanStopLoss = 1,
        [EnumMember()]
        StopLoss = 2,
        [EnumMember()]
        ChokotieLoss = 3,
        [EnumMember()]
        [Obsolete()]
        FreeRunningLoss = 4,
        [EnumMember()]
        [Obsolete()]
        RealOperation = 5,
        [EnumMember()]
        NetOperationTime = 6
        
    }
}
