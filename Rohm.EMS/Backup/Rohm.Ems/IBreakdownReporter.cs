using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Rohm.Ems
{
    [ServiceContract()]
    public interface IBreakdownReporter
    {
        [OperationContract()]
        void ReportBreakDown(string processName, string mcNo, string breakDownReason);
    }
}
