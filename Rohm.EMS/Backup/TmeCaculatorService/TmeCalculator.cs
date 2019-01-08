using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Rohm.Ems;

namespace TmeCalculatorService
{
    public partial class TmeCalculator : ServiceBase
    {
        private EventLog m_EL;
        private TmeCalculatorService.Properties.Settings m_Setting;

        public TmeCalculator()
        {
            InitializeComponent();
            
            //m_Service = new TotalEfficiencyRateCalculatorService("Data Source=172.16.0.102;Initial Catalog=DBx;User ID=dbxuser;Password='';Persist Security Info=True;");
            m_Setting = new TmeCalculatorService.Properties.Settings();

            m_Service = new TotalEfficiencyRateCalculatorService(m_Setting.DBxConnectionString);
            m_Service.ErrorCatched += new ErrorCatchedEventHandler(m_Service_ErrorCatched);

            m_EL = new EventLog();
            m_EL.Source = "TmeCalculatorService";
            
        }

        void m_Service_ErrorCatched(object sender, ErrorCatchedEventArgs e)
        {
            m_EL.WriteEntry(e.ErrorCode + "-->" + e.Ex.Message + Environment.NewLine + e.Detail, EventLogEntryType.Error);
        }

        private TotalEfficiencyRateCalculatorService m_Service;

        protected override void OnStart(string[] args)
        {
            m_EL.WriteEntry("Start with connection String :=" + m_Setting.DBxConnectionString);
            m_Service.Start();
        }

        protected override void OnStop()
        {
            m_Service.Stop();
        }
    }
}
