using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace Rohm.Ems
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class EmsService : IReporter, IMonitor, IBreakdownReporter
    {
        private ServiceHost m_ServiceHost;
        private DBxDatabase m_DB;
        private List<IMonitorCallback> m_Callbacks;
        private System.Timers.Timer m_KeepMonitoCallbackAliveTimer;
        private EmsActivityRecorder m_ActivityRecorder;
        public Dictionary<string, EmsMachineRow> m_MachineList;
        private System.Timers.Timer m_DailySaveTimer;

        private DateTime m_UpdateTimeStamp;

        public event ErrorCatchedEventHandler ErrorCatched;
        public event EmsReportEventHandler Reported;
        
        public EmsService(string connectionString)
        {
            m_Callbacks = new List<IMonitorCallback>();
            m_MachineList = new Dictionary<string, EmsMachineRow>();
            m_ActivityRecorder = new EmsActivityRecorder();

            m_DB = new DBxDatabase();
            m_DB.ConnectionString = connectionString; 
            //m_DB.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=LocalDBx;Data Source=Client-003";

            m_ServiceHost = new ServiceHost(this);

            m_KeepMonitoCallbackAliveTimer = new System.Timers.Timer();
            m_KeepMonitoCallbackAliveTimer.Interval = 30000;
            m_KeepMonitoCallbackAliveTimer.Elapsed += new ElapsedEventHandler(m_KeepMonitoCallbackAliveTimer_Elapsed);

            m_DailySaveTimer = new System.Timers.Timer();
            m_DailySaveTimer.Interval = 5000;
            m_DailySaveTimer.Elapsed += new ElapsedEventHandler(m_DailySaveTimer_Elapsed);            
            
        }

        void m_DailySaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_DailySaveTimer.Stop();

            //rohm date have to -8 hour
            DateTime tmp = DateTime.Now.AddHours(-8);

            if (m_UpdateTimeStamp.Date != tmp.Date)
            {   
                DateTime captureDateTime = new DateTime(tmp.Year, tmp.Month, tmp.Day, 8, 0, 0);    
                //update 
                UpdateActivityRecord(captureDateTime);
                //save to database       
                SaveEmsMachine();
                SaveActivityRecord();
                //clear alarm count
                //https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
                foreach (EmsMachineRow mc in m_MachineList.Values.ToArray())
                {
                    mc.AlarmCount = 0;
                    mc.BMCount = 0;
                }
                //clear ActivityHistory
                RohmDate currentRohmDate = RohmDate.FromDateTime(captureDateTime);
                m_ActivityRecorder.ClearDataBefore(currentRohmDate.Date);

                m_UpdateTimeStamp = captureDateTime;

            }
            m_DailySaveTimer.Start();
        }

        private void UpdateActivityRecord(DateTime tmp)
        {
            //https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
            foreach (EmsMachineRow mcRow in m_MachineList.Values.ToArray())
            {
                m_ActivityRecorder.RecordActivityBeforeChange(mcRow, tmp);
                mcRow.ActivityChangeTime = tmp;
            }
        }

        private void RaiseErrorCatchedEvent(string errorCode, Exception ex, string detail)
        {
            if (ErrorCatched != null)
            {
                ErrorCatched(this, new ErrorCatchedEventArgs(errorCode, ex, detail));
            }
        }

        private void RaiseReportedEvent(string fnName, string msg)
        {
            if (Reported != null)
            {
                Reported(this, new EmsReportEventArgs() { FunctionName = fnName, Message = msg });
            }
        }
        
        public void Start()
        {
            if (m_ServiceHost.State == CommunicationState.Closed)
            {
                m_ServiceHost = new ServiceHost(this);
            }

            if (m_ServiceHost.State == CommunicationState.Created)
            {
                m_ServiceHost.Open();
            }

            //clear machine 
            m_MachineList.Clear();

            //โหลด Machine ทั้งหมด
            EmsMachineRow[] mcs = m_DB.LoadAllEmsMachine();
            string key = "";
            foreach (EmsMachineRow mc in mcs)
            {
                key = GetMachineUniqueKey(mc.ProcessName, mc.MCNo);
                if (m_MachineList.ContainsKey(key))
                {
                    m_MachineList.Remove(key);
                    RaiseReportedEvent("Start", "Duplicated key [MCNo :=" + mc.MCNo + ", ProcessName :=" + mc.ProcessName + "]");
                }
                m_MachineList.Add(key, mc);
            }
            //โหลด Activity ทั้งหมด
            EmsActivityRecordRow[] acts = m_DB.LoadEmsActivityRecordOfCurrentDate();
            m_ActivityRecorder.LoadActivity(acts);

            m_UpdateTimeStamp = DateTime.Now.AddHours(-8);

            m_KeepMonitoCallbackAliveTimer.Start();
            m_DailySaveTimer.Start();
        }

        public void Stop()
        {
            m_KeepMonitoCallbackAliveTimer.Stop();
            m_DailySaveTimer.Stop();

            SaveActivityRecord();
            m_ActivityRecorder.Clear();

            SaveEmsMachine();
            m_MachineList.Clear();

            m_ServiceHost.Close();
        }

        private void SaveEmsMachine()
        {
            DateTime tmp = DateTime.Now;
            //https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
            foreach (EmsMachineRow mc in m_MachineList.Values.ToArray())
            {
                try
                {
                    mc.LastUpdateDate = tmp;
                    m_DB.SaveEmsMachineRow(mc);
                }
                catch (Exception ex)
                {
                    RaiseErrorCatchedEvent("SaveEmsMachine-001", ex, "");
                }
            }
        }

        private void SaveActivityRecord()
        {
            EmsActivityRecordRow[] records = m_ActivityRecorder.GetActivityRecordRows();
            if (records != null && records.Length > 0)
            {
                foreach (EmsActivityRecordRow a in records)
                {
                    try
                    {
                        m_DB.SaveEmsActivityRecord(a);
                    }
                    catch (Exception ex)
                    {
                        RaiseErrorCatchedEvent("SaveActivityRecord-001", ex, "");
                    }
                }
            }
        }

        private void KeepAlived(IMonitorCallback imc)
        {            
            imc.KeepAlive();
        }

        private delegate void EachCallbackChannelHandler(IMonitorCallback m);

        void ForEachCallbackChannel(EachCallbackChannelHandler func)
        {            
            foreach (IMonitorCallback imc in m_Callbacks.ToArray())
            {

                ICommunicationObject co = ((ICommunicationObject)imc);
                CommunicationState state = co.State;
                switch (state)
                {
                    case CommunicationState.Opened:
                        func(imc);
                        break;
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                        break;
                    case CommunicationState.Faulted:
                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                    default:
                        m_Callbacks.Remove(imc);
                        break;
                }
            }

            if (m_Callbacks.Count == 0)
            {
                m_KeepMonitoCallbackAliveTimer.Stop();
            }
            
        }

        void m_KeepMonitoCallbackAliveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ForEachCallbackChannel(KeepAlived);
        }

        #region IReporter Members

        //obsolated : not use 
        public void ReportActivity(int machineID, int activityID, int processID)
        {

        }

        //obsolated : not use 
        public void ReportLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID)
        {

        }

        //obsolated : not use 
        public void ReportLotEnd2(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID, float standardRPM)
        {

        }

        //ใช้ในการสอบถามข้อมูล OutputInfo ล่าสุดที่ทำการบันทึกไว้ในฐานข้อมูล
        public EmsMachineOutputInfo RegisterMachine(EmsMachineRegisterInfo regInfo)
        {
            EmsMachineOutputInfo ret = null;
            EmsMachineRow mc = null;
            string key = GetMachineUniqueKey(regInfo.ProcessName, regInfo.MCNo);
          
            mc = GetMachine(regInfo.ProcessName, regInfo.MCNo);

            if (mc != null)
            {
                mc.AreaName = regInfo.AreaName;

                if (mc.CurrentLotNo != regInfo.CurrentLotNo)
                {
                    mc.CurrentLotNo = regInfo.CurrentLotNo;                   
                    mc.CurrentStandardRPM = regInfo.CurrentStandardRPM;
                    mc.CutTotalGood = regInfo.CutTotalGood; //always 0
                    mc.CutTotalNG = regInfo.CutTotalNG; //always 0
                }
                else
                {
                    mc.CurrentTotalGood = regInfo.CurrentTotalGood;
                    mc.CurrentTotalNG = regInfo.CurrentTotalNG;
                }
            }
            else
            {
                try
                {
                    mc = new EmsMachineRow();
                    //add to data base
                    mc.ID = -1;
                    mc.MCNo = regInfo.MCNo;
                    mc.RegisteredDate = DateTime.Now;
                    mc.CurrentActivityName = "";
                    mc.CurrentActivityCategoryName = "";
                    mc.AreaName = regInfo.AreaName;
                    mc.ProcessName = regInfo.ProcessName;
                    mc.MachineTypeName = regInfo.MachineTypeName;
                    mc.CurrentLotNo = regInfo.CurrentLotNo;
                    mc.CurrentTotalGood = regInfo.CurrentTotalGood;
                    mc.CurrentTotalNG = regInfo.CurrentTotalNG;
                    mc.CurrentStandardRPM = regInfo.CurrentStandardRPM;
                    mc.LastUpdateDate = DateTime.Now;
                    mc.CutTotalGood = regInfo.CutTotalGood;
                    mc.CutTotalNG = regInfo.CutTotalNG;
                    mc.ActivityChangeTime = DateTime.Now;
                    mc.AlarmCount = 0;
                    mc.BMCount = 0;
                    m_MachineList.Add(key, mc);
                }
                catch (Exception ex)
                {
                    RaiseErrorCatchedEvent("RegisterMachine-001", ex, "");
                }
            }

            int aff = m_DB.SaveEmsMachineRow(mc);

            ret = new EmsMachineOutputInfo(mc);

            return ret;
        }

        public bool SaveOutputInfo(EmsMachineOutputInfo oInfo)
        {
            bool ret = false;

            EmsMachineRow mcRow = GetMachine(oInfo.ProcessName, oInfo.MCNo);
           
            if (mcRow != null)
            {
                //กรณีหากถ้าเคยได้รับการ Register แล้วเก็บไว้ใน Memory แล้ว
                mcRow.CurrentLotNo = oInfo.CurrentLotNo;
                mcRow.CurrentTotalGood = oInfo.CurrentTotalGood;
                mcRow.CurrentTotalNG = oInfo.CurrentTotalNG;
                mcRow.CurrentStandardRPM = oInfo.CurrentStandardRPM;
                mcRow.CutTotalGood = oInfo.CutTotalGood;
                mcRow.CutTotalNG = oInfo.CutTotalNG;               
               
                //save to db
                try
                {
                    m_DB.SaveEmsMachineRow(mcRow);
                    ret = true;
                }
                catch (Exception ex)
                {
                    RaiseErrorCatchedEvent("SaveOutputInfo", ex, "SaveEmsMachineRow");
                }
            }
            
             
            
            return ret;
        }

        public void ReportOutput(EmsOutputRecordBLL output)
        {
            if (output == null)
            {
                return;
            }

            try
            {
                EmsOutputRecordRow row = new EmsOutputRecordRow(output);
                m_DB.InsertEmsOutput(row);
            }
            catch(Exception ex)
            {                
                //raise error
                RaiseErrorCatchedEvent("ReportOutput-001", ex, ex.StackTrace);
            }
        }

        public int SetCurrentActivity(string processName, string mcNo, string activityName, string activityCategoryName)
        {
            int errorCode = 0;
            //ค้นหาจากใน Collection ด้วย ProcessName และ MCNo
            EmsMachineRow mc = GetMachine(processName, mcNo);
            
            if (mc != null)
            {                
                DateTime timeStamp = DateTime.Now;
                //record
                m_ActivityRecorder.RecordActivityBeforeChange(mc, timeStamp);
                //change current activity
                mc.CurrentActivityName = activityName;
                mc.CurrentActivityCategoryName = activityCategoryName;
                mc.ActivityChangeTime = timeStamp;

                if (activityCategoryName == "ChokotieLoss")
                {
                    mc.AlarmCount += 1;
                }

                try
                {
                    m_DB.InsertActivityHistory(processName, mcNo, activityName, activityCategoryName, timeStamp);
                }
                catch (Exception ex) {
                    //raise error
                    RaiseErrorCatchedEvent("SetCurrentActivity-001", ex, "");
                }

                try
                {
                    int aff = m_DB.SaveEmsMachineRow(mc);
                }
                catch (Exception ex)
                {
                    RaiseErrorCatchedEvent("SetCurrentActivity-002", ex, "");
                }

            }
            else
            {
                //machine is not registered
                errorCode = 1;
            }

            return errorCode;
           
        }

        #endregion

        #region IMonitor Members

        public void Connect(int areaID)
        {
            OperationContext pc = OperationContext.Current;
            IMonitorCallback callbackChannel = pc.GetCallbackChannel<IMonitorCallback>();

            if (!m_KeepMonitoCallbackAliveTimer.Enabled)
            {
                m_KeepMonitoCallbackAliveTimer.Start();
            }
            
            if (!m_Callbacks.Contains(callbackChannel))
            {
                m_Callbacks.Add(callbackChannel);
            }
        }        

        #endregion

        private EmsMachineRow GetMachine(string processName, string mcNo)
        {
            string mcKey = GetMachineUniqueKey(processName, mcNo);
            
            EmsMachineRow mc = null;

            if (m_MachineList.ContainsKey(mcKey))
            {
                mc = m_MachineList[mcKey];
            }
            else
            {
                mc = m_DB.GetMachine(processName, mcNo);

                if (mc != null)
                {
                    m_MachineList.Add(mcKey, mc);
                }
            }

            return mc;
        }

        private string GetMachineUniqueKey(string processName, string mcNo)
        {
            string key = processName + mcNo;
            return key;
        }

        #region IBreakdownReporter Members

        public void ReportBreakDown(string processName, string mcNo, string breakDownReason)
        {
            SetCurrentActivity(processName, mcNo, "Breakdown", "StopLoss");
        }

        #endregion
    }
}
