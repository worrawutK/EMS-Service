using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Rohm.Ems
{
    [ServiceContract]
    public interface IReporter
    {
        [OperationContract]
        void ReportActivity(int machineID, int activityID, int processID);

        [OperationContract]
        void ReportLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID);

        [OperationContract()]
        void ReportLotEnd2(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID, float standardRPM);

        //Client ต้องเรียกใช้ตอนเปิดโปรแกรม
        [OperationContract()]
        EmsMachineOutputInfo RegisterMachine(EmsMachineRegisterInfo mcInfo);

        //Client ต้องเรียกใช้ ตอนปิดโปรแกรม
        [OperationContract()]
        bool SaveOutputInfo(EmsMachineOutputInfo mc);

        //Client ต้องเรียกใช้ตอนจบ Lot หรือ ตอนจบวัน เท่านั้น
        [OperationContract()]
        void ReportOutput(EmsOutputRecordBLL output);

        //Client ต้องเรียกใช้ตอนเปลี่ยน Activity 
        [OperationContract()]
        int SetCurrentActivity(string processName, string mcNo, string activityName, string activityCategoryName);

    }

}
