using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rohm.Ems
{
    public class TotalEfficiencyRateRow
    {        
		private string m_ProcessName;
		public string ProcessName { 
			get { return m_ProcessName; }
			set {
				if(m_ProcessName != value)
				{
					m_ProcessName = value;
				}
			}
		}			
       
		private string m_MCNo;
		public string MCNo { 
			get { return m_MCNo; }
			set {
				if(m_MCNo != value)
				{
					m_MCNo = value;
				}
			}
		}			
       
		private DateTime m_RohmDate;
		public DateTime RohmDate { 
			get { return m_RohmDate; }
			set {
				if(m_RohmDate != value)
				{
					m_RohmDate = value;
				}
			}
		}
		
       
		private double m_PlanStopLoss;
		public double PlanStopLoss { 
			get { return m_PlanStopLoss; }
			set {
				if(m_PlanStopLoss != value)
				{
					m_PlanStopLoss = value;
				}
			}
		}
		           
		private double m_StopLoss;
		public double StopLoss { 
			get { return m_StopLoss; }
			set {
				if(m_StopLoss != value)
				{
					m_StopLoss = value;
				}
			}
		}
		
		private double m_ChokotieLoss;
		public double ChokotieLoss { 
			get { return m_ChokotieLoss; }
			set {
				if(m_ChokotieLoss != value)
				{
					m_ChokotieLoss = value;
				}
			}
		}
		
		private double m_FreeRunningLoss;
		public double FreeRunningLoss { 
			get { return m_FreeRunningLoss; }
			set {
				if(m_FreeRunningLoss != value)
				{
					m_FreeRunningLoss = value;
				}
			}
		}
		
		private double m_RealOperationTime;
		public double RealOperationTime { 
			get { return m_RealOperationTime; }
			set {
				if(m_RealOperationTime != value)
				{
					m_RealOperationTime = value;
				}
			}
		}

        private double m_ValueOperationTime;
        public double ValueOperationTime
        {
            get { return m_ValueOperationTime; }
            set
            {
                if (m_ValueOperationTime != value)
                {
                    m_ValueOperationTime = value;
                }
            }
        }

        private double m_NGLoss;
        public double NGLoss
        {
            get { return m_NGLoss; }
            set
            {
                if (m_NGLoss != value)
                {
                    m_NGLoss = value;
                }
            }
        }			
		
		private double m_MTBF;
		public double MTBF { 
			get { return m_MTBF; }
			set {
				if(m_MTBF != value)
				{
					m_MTBF = value;
				}
			}
		}			
       
		private double m_PMMTTR;
		public double PMMTTR { 
			get { return m_PMMTTR; }
			set {
				if(m_PMMTTR != value)
				{
					m_PMMTTR = value;
				}
			}
		}			
       
		private double m_PDMTTR;
		public double PDMTTR { 
			get { return m_PDMTTR; }
			set {
				if(m_PDMTTR != value)
				{
					m_PDMTTR = value;
				}
			}
		}			
       
		private double m_PerformanceRate;
		public double PerformanceRate { 
			get { return m_PerformanceRate; }
			set {
				if(m_PerformanceRate != value)
				{
					m_PerformanceRate = value;
				}
			}
		}
		
		private double m_GoodRate;
		public double GoodRate { 
			get { return m_GoodRate; }
			set {
				if(m_GoodRate != value)
				{
					m_GoodRate = value;
				}
			}
		}
		
		private double m_TimeOperationRate;
		public double TimeOperationRate { 
			get { return m_TimeOperationRate; }
			set {
				if(m_TimeOperationRate != value)
				{
					m_TimeOperationRate = value;
				}
			}
		}

        private double m_TMERate;
        public double TMERate
        {
            get { return m_TMERate; }
            set
            {
                if (m_TMERate != value)
                {
                    m_TMERate = value;
                }
            }
        }

       
		private double m_TotalWorkingTime;
		public double TotalWorkingTime { 
			get { return m_TotalWorkingTime; }
			set {
				if(m_TotalWorkingTime != value)
				{
					m_TotalWorkingTime = value;
				}
			}
		}
			
	
		private double m_LoadTime;
		public double LoadTime { 
			get { return m_LoadTime; }
			set {
				if(m_LoadTime != value)
				{
					m_LoadTime = value;
				}
			}
		}
			
	
		private double m_OperationTime;
		public double OperationTime { 
			get { return m_OperationTime; }
			set {
				if(m_OperationTime != value)
				{
					m_OperationTime = value;
				}
			}
		}


        private double m_NetOperationTime;
        public double NetOperationTime
        {
            get { return m_NetOperationTime; }
            set
            {
                if (m_NetOperationTime != value)
                {
                    m_NetOperationTime = value;
                }
            }
        }

        private double m_BreakdownTime;
        public double BreakdownTime
        {
            get { return m_BreakdownTime; }
            set
            {
                if (m_BreakdownTime != value)
                {
                    m_BreakdownTime = value;
                }
            }
        }

        private int m_TotalGood;
        public int TotalGood
        {
            get { return m_TotalGood; }
            set
            {
                if (m_TotalGood != value)
                {
                    m_TotalGood = value;
                }
            }
        }

        private int m_TotalNG;
        public int TotalNG
        {
            get { return m_TotalNG; }
            set
            {
                if (m_TotalNG != value)
                {
                    m_TotalNG = value;
                }
            }
        }

        private int m_AlarmCount;
        public int AlarmCount
        {
            get { return m_AlarmCount; }
            set
            {
                if (m_AlarmCount != value)
                {
                    m_AlarmCount = value;
                }
            }
        }

        private int m_BreakdownCount;
        public int BreakdownCount
        {
            get { return m_BreakdownCount; }
            set
            {
                if (m_BreakdownCount != value)
                {
                    m_BreakdownCount = value;
                }
            }
        }
				
        public TotalEfficiencyRateRow() { 
        }

        public TotalEfficiencyRateRow(DataRow row)
        { 
              m_ProcessName = (string)row["ProcessName"];
              m_MCNo = (string)row["MCNo"];
              m_RohmDate = (DateTime)row["RohmDate"];
              m_PlanStopLoss = (double)row["PlanStopLoss"];
              m_StopLoss = (double)row["StopLoss"];
              m_ChokotieLoss = (double)row["ChokotieLoss"];
              m_FreeRunningLoss = (double)row["FreeRunningLoss"];
              m_RealOperationTime = (double)row["RealOperationTime"];
              m_ValueOperationTime = (double)row["ValueOperationTime"];
              m_NGLoss = (double)row["NGLoss"];
              m_MTBF = (double)row["MTBF"];
              m_PMMTTR = (double)row["PMMTTR"];
              m_PDMTTR = (double)row["PDMTTR"];
              m_PerformanceRate = (double)row["PerformanceRate"];
              m_GoodRate = (double)row["GoodRate"];
              m_TimeOperationRate = (double)row["TimeOperationRate"];
              m_TMERate = (double)row["TMERate"];
              m_TotalWorkingTime = (double)row["TotalWorkingTime"];
              m_LoadTime = (double)row["LoadTime"];
              m_OperationTime = (double)row["OperationTime"];
              m_NetOperationTime = (double)row["NetOperationTime"];
              m_TotalGood = (int)row["TotalGood"];
              m_TotalNG = (int)row["TotalNG"];
              m_AlarmCount = (int)row["AlarmCount"];
              m_BreakdownCount = (int)row["BreakdownCount"];
              m_BreakdownTime = (double)row["BreakdownTime"];
        }
			
    }
}
