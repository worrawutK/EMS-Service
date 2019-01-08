using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Rohm.Ems
{
    [ServiceContract(CallbackContract=typeof(IMonitorCallback))]
    public interface IMonitor
    {
        [OperationContract()]
        void Connect(int areaID);
    }
}
