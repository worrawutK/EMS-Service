using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MockUpClient
{
    class Machine :INotifyPropertyChanged
    {

        private string m_MCNo;
        public string MCNo
        {
            get { return m_MCNo; }
            set
            {
                if (m_MCNo != value)
                {
                    m_MCNo = value;
                    ReportPropertyChanged("MCNo");
                }
            }
        }

        private string m_LotNo;
        public string LotNo
        {
            get { return m_LotNo; }
            set
            {
                if (m_LotNo != value)
                {
                    m_LotNo = value;
                    ReportPropertyChanged("LotNo");
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
                    ReportPropertyChanged("TotalGood");
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
                    ReportPropertyChanged("TotalNG");
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
                    ReportPropertyChanged("StandardRPM");
                }
            }
        }
			
        private void ReportPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
