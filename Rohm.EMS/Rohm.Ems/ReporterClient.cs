using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Rohm.Ems
{
    public class ReporterClient:ClientBase<IReporter>, IReporter 
    {
        public ReporterClient(string serviceEndpointUri):base(new BasicHttpBinding(), new EndpointAddress(serviceEndpointUri))
        {          
        }

        #region IReporter Members

        public void ReportActivity(int machineID, int activityID, int processID)
        {
            base.Channel.ReportActivity(machineID, activityID, processID);
        }

        public void ReportLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID)
        {
            base.Channel.ReportLotEnd(machineID, lotNo, inputQty, goodOutput, ngOutput, processID);
        }

        public void ReportLotEnd2(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID, float standardRPM)
        {
            base.Channel.ReportLotEnd2(machineID, lotNo, inputQty, goodOutput, ngOutput, processID, standardRPM);
        }

        public EmsMachineOutputInfo RegisterMachine(EmsMachineRegisterInfo regInfo)
        {
            return base.Channel.RegisterMachine(regInfo);
        }

        public bool SaveOutputInfo(EmsMachineOutputInfo mc)
        {
            return base.Channel.SaveOutputInfo(mc);
        }

        public void ReportOutput(EmsOutputRecordBLL output)
        {
            base.Channel.ReportOutput(output);
        }

        public int SetCurrentActivity(string processName, string mcNo, string activityName, string activityCategoryName)
        {
            return base.Channel.SetCurrentActivity(processName, mcNo, activityName, activityCategoryName);
        }

        #endregion

    }
}
