using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rohm.Ems
{
    public class TmeCalculator
    {
        private DBxDatabase m_DB;

        public TmeCalculator(string connectionString)
        {
            m_DB = new DBxDatabase();
            m_DB.ConnectionString = connectionString;
        }

        public TotalEfficiencyRateRow GetTotalEfficiencyRateRow(string processName, string mcNo, DateTime rohmDate)
        {
            EmsOutputRecordRow[] outputRecords = m_DB.LoadOutputRecord(processName, mcNo, rohmDate);
            Dictionary<string, EmsActivityRecordRow> activityRecordDic = new Dictionary<string, EmsActivityRecordRow>();

            EmsActivityHistoryRow preHist = null;
            EmsActivityHistoryRow curHist = null;
            EmsActivityRecordRow activityRecord = null;
            EmsActivityRecordRow prevActivityRecord = null;

            RohmDate targetRohmDate = RohmDate.FromDateTime(rohmDate);
            RohmDate nowRohmDate = RohmDate.FromNow();

            RohmDate tmp;

            using (DataTable tbl = m_DB.GetActivityHistory(processName, mcNo, rohmDate))
            {
                
                foreach (DataRow row in tbl.Rows)
                {
                    curHist = new EmsActivityHistoryRow(row);
                    
                    if (activityRecordDic.ContainsKey(curHist.ActivityName))
                    {
                        activityRecord = activityRecordDic[curHist.ActivityName];
                    }
                    else
                    {
                        activityRecord = new EmsActivityRecordRow();
                        activityRecord.ActivityName = curHist.ActivityName;
                        activityRecord.ActivityCategoryName = curHist.ActivityCategoryName;
                        activityRecord.Duration = 0;
                        activityRecord.ProcessName = processName;
                        activityRecord.MCNo = mcNo;
                        activityRecord.RohmDate = rohmDate;
                        activityRecordDic.Add(activityRecord.ActivityName, activityRecord);
                    }

                    if (preHist != null)
                    {
                        prevActivityRecord = activityRecordDic[preHist.ActivityName];

                        tmp = RohmDate.FromDateTime(preHist.RecordTime);
                        if (tmp.Date != targetRohmDate.Date)
                        {
                            prevActivityRecord.Duration += curHist.RecordTime.Subtract(targetRohmDate.StartDate).Duration().TotalMinutes;
                        }
                        else
                        {
                            prevActivityRecord.Duration += curHist.RecordTime.Subtract(preHist.RecordTime).Duration().TotalMinutes;                            
                        }
                    }

                    preHist = curHist;
                }
            }

            if (preHist != null)
            {
                prevActivityRecord = activityRecordDic[preHist.ActivityName];
                if (nowRohmDate.Date == targetRohmDate.Date)
                {
                    prevActivityRecord.Duration = preHist.RecordTime.Subtract(DateTime.Now).Duration().TotalMinutes;
                }
                else
                {
                    prevActivityRecord.Duration = preHist.RecordTime.Subtract(targetRohmDate.EndDate).Duration().TotalMinutes;
                }
            }

            TotalEfficiencyRateRow ter = new TotalEfficiencyRateRow();
            ter.ProcessName = processName;
            ter.MCNo = mcNo;
            ter.RohmDate = rohmDate;

            RohmDate r = RohmDate.FromDate(rohmDate);
            int alarmCount = 0;// m_DB.CountChokotieLoss(processName, mcNo, r);
            int breakdownCount = 0;// m_DB.CountBreakdown(processName, mcNo, r);
            foreach (EmsActivityRecordRow a in activityRecordDic.Values.ToArray())
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
                        }
                        break;
                    case "ChokotieLoss":
                        ter.ChokotieLoss += a.Duration;
                        alarmCount += 1;
                        break;
                    case "FreeRunningLoss": //I had made a mistake on client side
                    case "RealOperation": //I had made a mistake on client side
                    case "NetOperationTime":
                        ter.NetOperationTime += a.Duration;
                        break;
                    case "Breakdown":
                        breakdownCount += 1;
                        break;
                    default:
                        break;
                }
            }// foreach (EmsActivityRecordBLL a in m_ActivityRecords)

            ter.AlarmCount = alarmCount;
            ter.BreakdownCount = breakdownCount;

            ter.OperationTime = ter.NetOperationTime + ter.ChokotieLoss;
            ter.LoadTime = ter.OperationTime + ter.StopLoss;
            //ter.LoadTime + ter.PlanStopLoss;
            ter.TotalWorkingTime = targetRohmDate.StartDate.Subtract(targetRohmDate.EndDate).Duration().TotalMinutes;

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
