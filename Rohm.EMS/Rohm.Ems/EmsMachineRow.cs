using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rohm.Ems
{
    [Serializable()]
    public class EmsMachineRow
    {
        private int m_ID;
        public int ID
        {
            get { return m_ID; }
            set
            {
                m_ID = value;
            }
        }

        private string m_MCNo;
        public string MCNo
        {
            get { return m_MCNo; }
            set
            {
                m_MCNo = value;
            }
        }


        private DateTime m_RegisteredDate;
        public DateTime RegisteredDate
        {
            get { return m_RegisteredDate; }
            set
            {
                m_RegisteredDate = value;
            }
        }

        private string m_CurrentActivityName;
        public string CurrentActivityName
        {
            get { return m_CurrentActivityName; }
            set
            {
                m_CurrentActivityName = value;
            }
        }

        private string m_CurrentActivityCategoryName;
        public string CurrentActivityCategoryName
        {
            get { return m_CurrentActivityCategoryName; }
            set
            {
                m_CurrentActivityCategoryName = value;
            }
        }

        private string m_AreaName;
        public string AreaName
        {
            get { return m_AreaName; }
            set
            {
                m_AreaName = value;
            }
        }

        private string m_ProcessName;
        public string ProcessName
        {
            get { return m_ProcessName; }
            set
            {
                m_ProcessName = value;
            }
        }

        private string m_MachineTypeName;
        public string MachineTypeName
        {
            get { return m_MachineTypeName; }
            set
            {
                m_MachineTypeName = value;
            }
        }

        private string m_CurrentLotNo;
        public string CurrentLotNo
        {
            get { return m_CurrentLotNo; }
            set
            {
                m_CurrentLotNo = value;
            }
        }

        private int m_CurrentTotalGood;
        public int CurrentTotalGood
        {
            get { return m_CurrentTotalGood; }
            set
            {
                m_CurrentTotalGood = value;
            }
        }

        private int m_CurrentTotalNG;
        public int CurrentTotalNG
        {
            get { return m_CurrentTotalNG; }
            set
            {
                m_CurrentTotalNG = value;
            }
        }

        private double m_CurrentStandardRPM;
        public double CurrentStandardRPM
        {
            get { return m_CurrentStandardRPM; }
            set
            {
                m_CurrentStandardRPM = value;
            }
        }

        private DateTime m_LastUpdateDate;
        public DateTime LastUpdateDate
        {
            get { return m_LastUpdateDate; }
            set
            {
                m_LastUpdateDate = value;
            }
        }

        private int m_CutTotalGood;
        public int CutTotalGood
        {
            get { return m_CutTotalGood; }
            set
            {
                m_CutTotalGood = value;
            }
        }

        private int m_CutTotalNG;
        public int CutTotalNG
        {
            get { return m_CutTotalNG; }
            set
            {
                m_CutTotalNG = value;
            }
        }

        private DateTime m_ActivityChangeTime;
        public DateTime ActivityChangeTime
        {
            get { return m_ActivityChangeTime; }
            set
            {
                m_ActivityChangeTime = value;
            }
        }

        private int m_AlarmCount;
        public int AlarmCount
        {
            get { return m_AlarmCount; }
            set {
                m_AlarmCount = value;
            }
        }

        private int m_BMCount;
        public int BMCount
        {
            get { return m_BMCount; }
            set {
                m_BMCount = value;
            }
        }
	
        public EmsMachineRow(DataRow source)
        {
            m_ID = (int)source["ID"];
            m_MCNo = (string)source["MCNo"];
            m_RegisteredDate = (DateTime)source["RegisteredDate"];
            m_CurrentActivityName = (string)source["CurrentActivityName"];
            m_CurrentActivityCategoryName = (string)source["CurrentActivityCategoryName"];
            m_AreaName = (string)source["AreaName"];
            m_ProcessName = (string)source["ProcessName"];
            m_MachineTypeName = (string)source["MachineTypeName"];
            m_CurrentLotNo = (string)source["CurrentLotNo"];
            m_CurrentTotalGood = (int)source["CurrentTotalGood"];
            m_CurrentTotalNG = (int)source["CurrentTotalNG"];
            m_CurrentStandardRPM = (double)source["CurrentStandardRPM"];
            m_LastUpdateDate = (DateTime)source["LastUpdateDate"];
            m_CutTotalGood = (int)source["CutTotalGood"];
            m_CutTotalNG = (int)source["CutTotalNG"];
            m_ActivityChangeTime = (DateTime)source["ActivityChangeTime"];
            m_AlarmCount = (int)source["AlarmCount"];
            m_BMCount = (int)source["BMCount"];
        }

        public EmsMachineRow()
        {
        }
			
    }
}
