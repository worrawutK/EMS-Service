using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rohm.Ems
{
    [DataContract()]
    public class EmsMachineOutputInfo
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

        private string m_CurrentLotNo;
        [DataMember()]
        public string CurrentLotNo
        {
            get { return m_CurrentLotNo; }
            set
            {
                if (m_CurrentLotNo != value)
                {
                    m_CurrentLotNo = value;
                }
            }
        }

        private int m_CurrentTotalGood;
        [DataMember()]
        public int CurrentTotalGood
        {
            get { return m_CurrentTotalGood; }
            set
            {
                if (m_CurrentTotalGood != value)
                {
                    m_CurrentTotalGood = value;
                }
            }
        }

        private int m_CurrentTotalNG;
        [DataMember()]
        public int CurrentTotalNG
        {
            get { return m_CurrentTotalNG; }
            set
            {
                if (m_CurrentTotalNG != value)
                {
                    m_CurrentTotalNG = value;
                }
            }
        }

        private double m_CurrentStandardRPM;
        [DataMember()]
        public double CurrentStandardRPM
        {
            get { return m_CurrentStandardRPM; }
            set
            {
                if (m_CurrentStandardRPM != value)
                {
                    m_CurrentStandardRPM = value;
                }
            }
        }

        private int m_CutTotalGood;
        [DataMember()]
        public int CutTotalGood
        {
            get { return m_CutTotalGood; }
            set
            {
                if (m_CutTotalGood != value)
                {
                    m_CutTotalGood = value;
                }
            }
        }

        private int m_CutTotalNG;
        [DataMember()]
        public int CutTotalNG
        {
            get { return m_CutTotalNG; }
            set
            {
                if (m_CutTotalNG != value)
                {
                    m_CutTotalNG = value;
                }
            }
        }

        public EmsMachineOutputInfo(EmsMachineRow data)
        {
            m_ID = data.ID;
            m_MCNo = data.MCNo;
            m_ProcessName = data.ProcessName;
            m_CurrentLotNo = data.CurrentLotNo;
            m_CurrentTotalGood = data.CurrentTotalGood;
            m_CurrentTotalNG = data.CurrentTotalNG;
            m_CurrentStandardRPM = data.CurrentStandardRPM;
            m_CutTotalGood = data.CutTotalGood;
            m_CutTotalNG = data.CutTotalNG;
        }
    }
}
