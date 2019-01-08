using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EmsMonitor.EmsServiceReference;
using System.ServiceModel;

namespace EmsMonitor
{
    public partial class Form1 : Form , IMonitorCallback 
    {
        private MonitorClient m_Client;

        public Form1()
        {
            InitializeComponent();

            InstanceContext ic = new InstanceContext(this);
            m_Client = new MonitorClient(ic);

            using (Testpublic t = new Testpublic())
            { 
            
            }

        }

        #region IMonitorCallback Members

        public void MachineActivityChanged(int machineID, int activityID, int processID)
        {
            textBox1.AppendText(string.Format("MachineId = {0}, ActivityID = {1}, ProcessID = {2}", machineID, activityID, processID));
        }

        public void MachineLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID)
        {
            textBox1.AppendText(string.Format("MachineId = {0}, LotNo = {1}, InputQty = {2}", machineID, lotNo, processID));
        }

        public void KeepAlive()
        {
            
        }

        #endregion

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            m_Client.Connect(1);
        }


        public class Testpublic : System.ServiceModel.ClientBase<EmsMonitor.EmsServiceReference.IReporter>, EmsMonitor.EmsServiceReference.IReporter 
        {

            #region IReporter Members

            public void ReportActivity(int machineID, int activityID, int processID)
            {
                throw new NotImplementedException();
            }

            public void ReportLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

    }
}
