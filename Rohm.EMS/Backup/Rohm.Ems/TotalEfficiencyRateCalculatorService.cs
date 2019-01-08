using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rohm.Ems
{
    public class TotalEfficiencyRateCalculatorService
    {
        private System.Timers.Timer m_Timer;
        private DBxDatabase m_DB;
        private bool m_IsCalculating;
        private bool m_IsStarted;

        public event ErrorCatchedEventHandler ErrorCatched;

        public TotalEfficiencyRateCalculatorService(string conString)
        {
            m_Timer = new System.Timers.Timer();
            m_Timer.Elapsed += new System.Timers.ElapsedEventHandler(m_Timer_Elapsed);
            m_Timer.Interval = 1000;

            m_DB = new DBxDatabase();
            m_DB.ConnectionString = conString;
            //m_DB.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=LocalDBx;Data Source=Client-003";

            m_IsCalculating = false;
            m_IsStarted = false;
        }

        private void RaiseErrorCatchedEvent(string errorCode, Exception ex, string detail)
        {
            if (ErrorCatched != null)
            {
                ErrorCatched(this, new ErrorCatchedEventArgs(errorCode, ex, detail));
            }
        }

        void m_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime tmp = DateTime.Now;
            //start at 8:30 of every day
            if (tmp.Hour == 8 && tmp.Minute == 30 && tmp.Second == 0)
            {
                m_IsCalculating = true;
                m_Timer.Stop();
                DoCalculateJob();
                if (m_IsStarted)
                {
                    m_Timer.Start();
                }
                m_IsCalculating = false;
            }
        }

        public void Start()
        {
            m_IsStarted = true;
            if (!m_IsCalculating)
            {
                m_Timer.Start();
            }
        }

        public void Stop()
        {
            m_IsStarted = false;
            m_Timer.Stop();
        }

        private void DoCalculateJob() {
            RohmDate yesterDay = RohmDate.FromNow();
            yesterDay.AddDays(-1);
            //RohmDate yesterDay = RohmDate.FromDateTime(new DateTime(2013, 9, 27, 8,0,0));
            Calculate(yesterDay.Date);
        }

        public void Calculate(DateTime rohmDate)
        {
            EmsMachineRow[] machines = m_DB.GetAllMachine();

            EmsActivityRecordRow[] activityRecords;
            EmsOutputRecordRow[] outputRecords;
            TotalEfficiencyRateRow row;
            //int alarmCount = 0;

            foreach (EmsMachineRow mc in machines)
            {
                try
                {
                    activityRecords = m_DB.LoadActivityRecord(mc.ProcessName, mc.MCNo, rohmDate);
                    outputRecords = m_DB.LoadOutputRecord(mc.ProcessName, mc.MCNo, rohmDate);
                }
                catch (Exception ex)
                {
                    string data = mc.ProcessName + "," + mc.MCNo + "," + rohmDate.ToString("yyyy/MM/dd");
                    RaiseErrorCatchedEvent("Calculate-001", ex, data);
                    continue;
                }

                if (activityRecords.Length != 0)
                {
                    try
                    {
                        row = Calculate(mc.ProcessName, mc.MCNo, rohmDate, outputRecords, activityRecords, mc.AlarmCount);
                    }
                    catch (Exception ex)
                    {
                        string data = mc.ProcessName + "," + mc.MCNo + "," + rohmDate.ToString("yyyy/MM/dd");
                        RaiseErrorCatchedEvent("Calculate-002", ex, data);
                        continue;
                    }

                    try
                    {
                        //save to database 
                        m_DB.InsertTotalMachineEfficiency(row);
                    }
                    catch (Exception ex)
                    {
                        string data = mc.ProcessName + "," + mc.MCNo + "," + rohmDate.ToString("yyyy/MM/dd");
                        RaiseErrorCatchedEvent("Calculate-003", ex, data);
                        continue;
                    }
                }
            }
        }

        public TotalEfficiencyRateRow Calculate(string processName, string mcNo, DateTime rohmDate, 
            EmsOutputRecordRow[] outputRecords,EmsActivityRecordRow[] activityRecords, int alarmCount)
        {
            TotalEfficiencyRateRow ter = new TotalEfficiencyRateRow();
            ter.ProcessName = processName;
            ter.MCNo = mcNo;
            ter.RohmDate = rohmDate;

            RohmDate r = RohmDate.FromDate(rohmDate);
            //int alarmCount = 0;// m_DB.CountChokotieLoss(processName, mcNo, r);
            int breakdownCount = 0;// m_DB.CountBreakdown(processName, mcNo, r);
            foreach (EmsActivityRecordRow a in activityRecords)
            {
                switch (a.ActivityCategoryName)
                { 
                    case "PlanStopLoss":
                        ter.PlanStopLoss += a.Duration;
                        break;
                    case "StopLoss":
                        ter.StopLoss += a.Duration;
                        if (a.ActivityName == "Breakdown")
                        {
                            ter.BreakdownTime += a.Duration;
                            breakdownCount += 1;
                        }
                        break;
                    case "ChokotieLoss":
                        ter.ChokotieLoss += a.Duration;
                        break;
                    case "FreeRunningLoss": //I had made a mistake on client side
                    case "RealOperation": //I had made a mistake on client side
                    case "NetOperationTime":
                        ter.NetOperationTime += a.Duration;
                        break;
                    default:
                        break;
                }
            }// foreach (EmsActivityRecordBLL a in m_ActivityRecords)

            ter.AlarmCount = alarmCount;
            ter.BreakdownCount = breakdownCount;            

            ter.OperationTime = ter.NetOperationTime + ter.ChokotieLoss;
            ter.LoadTime = ter.OperationTime + ter.StopLoss;
            ter.TotalWorkingTime = ter.LoadTime + ter.PlanStopLoss;

            double sumOutput = 0; //ห้ามใช้ int
            double sumNG = 0; //ห้ามใช้ int
            double sumGood = 0; //ห้ามใช้ int
            double lotCount = 0; //ห้ามใช้ int
            double sumRealOperationTime = 0;

            //เนื่องจากตอนคำนวณ ผลลัพจะถูกปัดให้เป็น int ทำให้จาก 0.94 กลายเป็น 0
            foreach (EmsOutputRecordRow o in outputRecords)
            {
                if (o.StandardRPM > 0) //some time selfcon sent 0 
                {
                    sumRealOperationTime += ((double)(o.TotalGood + o.TotalNG)) / (o.StandardRPM);
                }
                
                sumNG += o.TotalNG;
                sumGood += o.TotalGood;
                lotCount += 1;
            }

            ter.RealOperationTime = sumRealOperationTime;
            ter.FreeRunningLoss = ter.NetOperationTime - ter.RealOperationTime;
            
            ter.TotalGood = (int)sumGood;
            ter.TotalNG = (int)sumNG;
            
            sumOutput = sumGood + sumNG;      
            if (sumOutput != 0)
            {
                //calculate Value Operation Time
                ter.ValueOperationTime = (sumGood / sumOutput) * ter.RealOperationTime;
                //calculate NG Loss
                ter.NGLoss = (sumNG / sumOutput) * ter.RealOperationTime;
            }

            //calculate performance rate by average
            if (lotCount != 0 && ter.OperationTime != 0)
            {
                ter.PerformanceRate = (ter.RealOperationTime / ter.OperationTime) * 100;
            }
            //calculate good rate            
            if (sumGood != 0)
            {
                ter.GoodRate = ((sumGood - sumNG) / sumGood) * 100;
            }
            //calculate Time Operation Rate
            if (ter.LoadTime != 0)
            {
                ter.TimeOperationRate = (ter.OperationTime / ter.LoadTime) * 100;
            }
            //calculate PD MTTR 
            if (alarmCount != 0)
            {
                ter.PDMTTR = ter.ChokotieLoss / alarmCount;
            }
            //calculate PM MTTR
            if (breakdownCount != 0)
            {
                ter.PMMTTR = ter.BreakdownTime / breakdownCount;
            }
            //calculate  MTBF 
            ter.MTBF = ter.NetOperationTime / (alarmCount + 1);
            //calculate TME rate
            //reference to PM [2016-10-19]
            //  Operation rate = 93 %
            //  Performance rate = 50.6 %
            //  Good Rate = 99.15 %
            //  *** TME = 46 % [from 93 * 50.6 * 99.15 / 10000]
            ter.TMERate = ter.TimeOperationRate * ter.PerformanceRate * ter.GoodRate / 10000;

            return ter;

        }

    }
}
