using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rohm.Ems
{    
    public class EmsOutputRecordRow
    {       
		private int m_ID;
		public int ID { 
			get { return m_ID; }
			set {
				if(m_ID != value)
				{
					m_ID = value;
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
  
		private string m_LotNo;
		public string LotNo { 
			get { return m_LotNo; }
			set {
				if(m_LotNo != value)
				{
					m_LotNo = value;
				}
			}
		}
		
  
		private int m_TotalGood;
		public int TotalGood { 
			get { return m_TotalGood; }
			set {
				if(m_TotalGood != value)
				{
					m_TotalGood = value;
				}
			}
		}
		
  
		private int m_TotalNG;
		public int TotalNG { 
			get { return m_TotalNG; }
			set {
				if(m_TotalNG != value)
				{
					m_TotalNG = value;
				}
			}
		}


        private double m_StandardRPM;
        public double StandardRPM
        {
            get { return m_StandardRPM; }
            set
            {
                if (m_StandardRPM != value)
                {
                    m_StandardRPM = value;
                }
            }
        }

        public EmsOutputRecordRow()
        { 
            
        }

        public EmsOutputRecordRow(DataRow row)
        {
            m_ID = (int)row["ID"];
            m_RohmDate = (DateTime)row["RohmDate"];
            m_ProcessName = (string)row["ProcessName"];
            m_MCNo = (string)row["MCNo"];
            m_LotNo = (string)row["LotNo"];
            m_TotalGood = (int)row["TotalGood"];
            m_TotalNG = (int)row["TotalNG"];
            m_StandardRPM = (double)row["StandardRPM"];
        }

        public EmsOutputRecordRow(EmsOutputRecordBLL output) {
            m_ID = output.ID;
            m_RohmDate = output.RohmDate;
            m_ProcessName = output.ProcessName;
            m_MCNo = output.MCNo;
            m_LotNo = output.LotNo;
            m_TotalGood = output.TotalGood;
            m_TotalNG = output.TotalNG;
            m_StandardRPM = output.StandardRPM;
        }
			
    }
}
