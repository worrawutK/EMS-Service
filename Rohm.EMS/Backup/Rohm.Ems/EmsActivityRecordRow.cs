using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rohm.Ems
{
    [Serializable()]
    public class EmsActivityRecordRow 
    {        
		private int m_ID;
		public int ID { 
			get { return m_ID; }
			set {
				m_ID = value;
			}
		}		
  
		private string m_ProcessName;
		public string ProcessName { 
			get { return m_ProcessName; }
			set {
				m_ProcessName = value;
			}
		}		
  
		private string m_MCNo;
		public string MCNo { 
			get { return m_MCNo; }
			set {
				m_MCNo = value;
			}
		}		
  
		private string m_ActivityName;
		public string ActivityName { 
			get { return m_ActivityName; }
			set {
				m_ActivityName = value;
			}
		}
		
  
		private string m_ActivityCategoryName;
		public string ActivityCategoryName { 
			get { return m_ActivityCategoryName; }
			set {
				m_ActivityCategoryName = value;
			}
		}
		
  
		private DateTime m_RohmDate;
		public DateTime RohmDate { 
			get { return m_RohmDate; }
			set {
				m_RohmDate = value;
			}
		}
    
        private double m_Duration;
        public double Duration
        {
            get { return m_Duration; }
            set
            {
                m_Duration = value;
            }
        }


        public EmsActivityRecordRow() {
        }

        public EmsActivityRecordRow(DataRow row)
        { 
            m_ID = (int)row["ID"];
            m_ProcessName = (string)row["ProcessName"];
            m_MCNo = (string)row["MCNo"];
            m_ActivityName = (string)row["ActivityName"];
            m_ActivityCategoryName = (string)row["ActivityCategoryName"];
            m_RohmDate = (DateTime)row["RohmDate"];
            m_Duration = (double)row["Duration"];
        }
			
    }
}
