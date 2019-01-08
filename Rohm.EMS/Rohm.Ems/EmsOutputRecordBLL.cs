using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rohm.Ems
{
    [Serializable()]
    [DataContract()]
    public class EmsOutputRecordBLL
    {
        private int m_ID;
        [DataMember()]
        public int ID
        {
            get { return m_ID; }
            set
            {
                if (m_ID != value)
                {
                    m_ID = value;
                }
            }
        }
			
        private string m_MCNo;
        [DataMember()]
        public string MCNo
        {
            get { return m_MCNo; }
            set
            {
                if (m_MCNo != value)
                {
                    m_MCNo = value;
                }
            }
        }
        
        private string m_ProcessName;
        [DataMember()]
        public string ProcessName
        {
            get { return m_ProcessName; }
            set
            {
                if (m_ProcessName != value)
                {
                    m_ProcessName = value;
                }
            }
        }

        private DateTime m_RohmDate;
        [DataMember()]
        public DateTime RohmDate
        {
            get { return m_RohmDate; }
            set
            {
                if (m_RohmDate != value)
                {
                    m_RohmDate = value;
                }
            }
        }			
			
        private string m_LotNo;
        [DataMember()]
        public string LotNo
        {
            get { return m_LotNo; }
            set
            {
                if (m_LotNo != value)
                {
                    m_LotNo = value;
                }
            }
        }
        
        private int m_TotalGood;
        [DataMember()]
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
        [DataMember()]
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

        private double m_StandardRPM;
        [DataMember()]
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

        public EmsOutputRecordBLL()
        { 
        
        }

        public EmsOutputRecordBLL(EmsOutputRecordRow data)
        {
            m_ID = data.ID;
            m_RohmDate = data.RohmDate;
            m_ProcessName = data.ProcessName;
            m_MCNo = data.MCNo;
            m_LotNo = data.LotNo;
            m_TotalGood = data.TotalGood;
            m_TotalNG = data.TotalNG;
            m_StandardRPM = data.StandardRPM;
        }
			
    }
}
