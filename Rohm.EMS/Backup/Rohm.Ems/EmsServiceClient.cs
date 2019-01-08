using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.ServiceModel;
using System.IO;

namespace Rohm.Ems
{
    public class EmsServiceClient
    {
        private ILogger m_Log;

        private System.Timers.Timer m_Timer;
        private System.Timers.Timer m_UpdateOutputTimer;

        private ReporterClient m_ReporterClient;
        private Dictionary<string, EmsMachineOutputInfo> m_MachineDictionary;
        private Dictionary<string, EmsMachineRegisterInfo> m_MachineRegisterInfoDictionary;

        
        public double UpdateOutputInterval
        {
            get {
                return m_UpdateOutputTimer.Interval;
            }
            set {
                m_UpdateOutputTimer.Interval = value;
            }
        }

        private object m_Locker = new object();

        public EmsServiceClient(string serviceEndpointUrl)
        {
            m_MachineDictionary = new Dictionary<string, EmsMachineOutputInfo>();
            m_MachineRegisterInfoDictionary = new Dictionary<string, EmsMachineRegisterInfo>();
            m_ReporterClient = new ReporterClient(serviceEndpointUrl);
            m_Timer = new System.Timers.Timer();
            m_Timer.Interval = 1000;
            m_Timer.Elapsed += new System.Timers.ElapsedEventHandler(m_Timer_Elapsed);
            m_UpdateOutputTimer = new System.Timers.Timer();
            m_UpdateOutputTimer.Interval = 600000; // 10 minutes
            m_UpdateOutputTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_UpdateOutputTimer_Elapsed);

            m_Log = new TextFileLogger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmsClient.log"));
        }

        void m_UpdateOutputTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_UpdateOutputTimer.Stop();

            lock (m_Locker)
            {
                try
                {
                    foreach (EmsMachineOutputInfo mc in m_MachineDictionary.Values.ToArray())
                    {
                        m_ReporterClient.SaveOutputInfo(mc);
                    }
                }
                catch(Exception ex) {
                    m_Log.Log("m_UpdateOutputTimer_Elapsed", "SaveOutputInfo", ex.Message, "");
                }
            }

            m_UpdateOutputTimer.Start();
        }

        void m_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_Timer.Stop();
            lock (m_Locker)
            {
                DateTime tmp = DateTime.Now;
                if (tmp.Hour == 8 && tmp.Minute == 0 && tmp.Second == 0)
                {
                    EmsOutputRecordBLL co;
                    foreach (EmsMachineOutputInfo mc in m_MachineDictionary.Values)
                    {
                        co = CaptureOutput(mc);

                        mc.CutTotalGood = mc.CurrentTotalGood;
                        mc.CutTotalNG = mc.CurrentTotalNG;

                        try
                        {
                            //save to db
                            m_ReporterClient.ReportOutput(co);
                        }
                        catch  (Exception ex)
                        {
                            m_Log.Log("m_Timer_Elapsed", "ReportOutput", ex.Message, "");
                        }

                        try
                        {
                            m_ReporterClient.SaveOutputInfo(mc);
                        }
                        catch (Exception ex)
                        {
                            m_Log.Log("m_Timer_Elapsed", "SaveOutputInfo", ex.Message, "");
                        }
                    }
                }
            }
            m_Timer.Start();
        }

        public void Start(){
            m_Timer.Start();
            m_UpdateOutputTimer.Start();
        }

        public void Stop(){

            foreach (EmsMachineOutputInfo mc in m_MachineDictionary.Values.ToArray())
            {
                m_ReporterClient.SaveOutputInfo(mc);                
            }
            m_UpdateOutputTimer.Stop();
            m_Timer.Stop();
        }

        ~EmsServiceClient()
        {               
            m_MachineDictionary.Clear();
            m_MachineDictionary = null;
            m_MachineRegisterInfoDictionary.Clear();
            m_MachineRegisterInfoDictionary = null;
            m_ReporterClient = null;
            m_Timer.Stop();
            m_Timer.Elapsed +=new System.Timers.ElapsedEventHandler(m_Timer_Elapsed);
            m_Timer = null;
        }

        public void Register(EmsMachineRegisterInfo regInfo)
        {
            string key = GetMachineUniqueKey(regInfo.ProcessName, regInfo.MCNo);
            //1.) keep registere  info
            //remove old register data if exists
            if (m_MachineRegisterInfoDictionary.ContainsKey(key))
            {
                m_MachineRegisterInfoDictionary.Remove(key);
            }
            //add new register data
            m_MachineRegisterInfoDictionary.Add(key, regInfo);

            PrivateRegister(key, regInfo);
        }        

        public void Remove(string processName, string mcNo)
        {
            EmsMachineOutputInfo oi = GetEmsMachine(processName, mcNo);
            if( oi != null )
            {
                m_ReporterClient.SaveOutputInfo(oi);
                m_MachineDictionary.Remove(mcNo);
            }
        }

        public void SetActivity(string processName, string mcNo, string activityName, TmeCategory category)
        {
            if (string.IsNullOrEmpty(activityName))
            {
                return;
            }

            try
            {
                int errorCode = m_ReporterClient.SetCurrentActivity(processName, mcNo, activityName, category.ToString());
                if (errorCode == 1) //machine not found
                {
                    ReRegister(processName, mcNo);
                    m_ReporterClient.SetCurrentActivity(processName, mcNo, activityName, category.ToString());
                }
            }
            catch(Exception ex)
            {
                m_Log.Log("SetActivity", "SetCurrentActivity", ex.Message, "");
            }
        }

        public void SetCurrentLot(string processName, string mcNo, string lotNo, double standardRpm )
        {
            EmsMachineOutputInfo oi = TryGetEmsMachine(processName, mcNo);

            if (oi != null)
            {
                oi.CurrentLotNo = lotNo;
                oi.CurrentStandardRPM = standardRpm;
                oi.CurrentTotalGood = 0;
                oi.CurrentTotalNG = 0;
                oi.CutTotalGood = 0;
                oi.CutTotalNG = 0;

                m_ReporterClient.SaveOutputInfo(oi);
            }
        }

        public void SetOutput(string processName, string mcNo, int totalGood, int totalNG)
        {
            EmsMachineOutputInfo oi = TryGetEmsMachine(processName , mcNo);
            if (oi != null)
            {
                oi.CurrentTotalGood = totalGood;
                oi.CurrentTotalNG = totalNG;
            }
        }

        public void SetLotEnd(string processName, string mcNo)
        {
            EmsMachineOutputInfo mc = TryGetEmsMachine(processName, mcNo);
            if (mc == null)
            {
                return;
            }

            EmsOutputRecordBLL co = CaptureOutput(mc);

            mc.CurrentLotNo = ""; //clear Lot
            mc.CurrentStandardRPM = 0;
            mc.CurrentTotalGood = 0;
            mc.CurrentTotalNG = 0;
            mc.CutTotalGood = 0;
            mc.CutTotalNG = 0;

            try
            {
                m_ReporterClient.ReportOutput(co);
            }
            catch (Exception ex)
            {
                m_Log.Log("SetLotEnd", "ReportOutput", ex.Message, "");
            }

            try
            {
                m_ReporterClient.SaveOutputInfo(mc);    
            }
            catch (Exception ex)
            {
                m_Log.Log("SetLotEnd", "SaveOutputInfo", ex.Message, "");
            }
        }

        private EmsMachineOutputInfo TryGetEmsMachine(string processName,string mcNo) 
        {
            EmsMachineOutputInfo oi = null;
            string key = GetMachineUniqueKey(processName, mcNo);

            if (m_MachineDictionary.ContainsKey(key))
            {
                oi = m_MachineDictionary[key];
            }
            else 
            {
                ReRegister(processName, mcNo);
                //check again after re-register
                if (m_MachineDictionary.ContainsKey(key))
                {
                    oi = m_MachineDictionary[key];
                }

            }
            return oi;
        }

        private void ReRegister(string processName, string mcNo)
        {
            string key = GetMachineUniqueKey(processName, mcNo);

            //1.) keep registere  info
            if (!m_MachineRegisterInfoDictionary.ContainsKey(key))
            {
                return;
            }
            //add new register data
            EmsMachineRegisterInfo regInfo = m_MachineRegisterInfoDictionary[key];
            PrivateRegister(key, regInfo);
            
        }

        private void PrivateRegister(string key, EmsMachineRegisterInfo regInfo)
        {
            try
            {
                //2.) register
                EmsMachineOutputInfo registeredMC = m_ReporterClient.RegisterMachine(regInfo);
                //3.) check exists 
                if (m_MachineDictionary.ContainsKey(key))
                {
                    m_MachineDictionary.Remove(key);
                }
                m_MachineDictionary.Add(key, registeredMC);
            }
            catch (Exception ex)
            {
                m_Log.Log("PrivateRegister", "RegisterMachine", ex.Message, "");
            }
        }

        private EmsMachineOutputInfo GetEmsMachine(string processName, string mcNo) 
        {
            EmsMachineOutputInfo oi = null;
            string key = GetMachineUniqueKey(processName, mcNo);
            if (m_MachineDictionary.ContainsKey(key))
            {
                oi = m_MachineDictionary[key];
            }
            return oi;
        }

        private EmsOutputRecordBLL CaptureOutput(EmsMachineOutputInfo mc)
        {
            if (string.IsNullOrEmpty(mc.CurrentLotNo))
            {
                return null;
            }

            EmsOutputRecordBLL ret = new EmsOutputRecordBLL();
            ret.LotNo = mc.CurrentLotNo;
            ret.MCNo = mc.MCNo;
            ret.ProcessName = mc.ProcessName;
            ret.RohmDate = DateTime.Now.AddHours(-8).Date;
            ret.StandardRPM = mc.CurrentStandardRPM;
            ret.TotalGood = mc.CurrentTotalGood - mc.CutTotalGood;
            ret.TotalNG = mc.CurrentTotalNG - mc.CutTotalNG;      
      
            return ret;
        }

        private string GetMachineUniqueKey(string processName, string mcNo)
        {
            string key = processName + mcNo;
            return key;
        }
    		
    }
}
