using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Rohm.Ems
{
    [ServiceContract()]
    public interface IMonitorCallback
    {
        [OperationContract()]
        void MachineActivityChanged(int machineID, int activityID, int processID);

        [OperationContract()]
        void MachineLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID, float standardRPM);

        ///asdskadl;askd;laskdl;ask
        //    add LotStart Method

        [OperationContract()]
        void KeepAlive();

        int AreaID { get; set; }
    }
}
