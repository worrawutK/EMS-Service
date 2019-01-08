using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Rohm.Ems;
using System.ServiceModel;

namespace EmsWinService
{
    public partial class EmsWinService : ServiceBase
    {
        
        private EventLog m_EL;
        private EmsService m_EmsService;

        public EmsWinService()
        {
            InitializeComponent();

            Properties.Settings setting = new global::EmsWinService.Properties.Settings();

            m_EmsService = new EmsService(setting.DBxConnectionString);
            m_EmsService.ErrorCatched += new ErrorCatchedEventHandler(m_EmsService_ErrorCatched);
            m_EmsService.Reported += new EmsReportEventHandler(m_EmsService_Reported);
            m_EL = new EventLog();
            m_EL.Source = "EmsWinService";
            m_EL.WriteEntry("Service started :=" + setting.DBxConnectionString , EventLogEntryType.Information);
        }

        void m_EmsService_Reported(object sender, EmsReportEventArgs e)
        {
            m_EL.WriteEntry(e.FunctionName + Environment.NewLine +
                e.Message, EventLogEntryType.Warning);
        }

        void m_EmsService_ErrorCatched(object sender, ErrorCatchedEventArgs e)
        {
            m_EL.WriteEntry(e.ErrorCode + "-->" + e.Ex.Message + Environment.NewLine + 
                e.Ex.StackTrace + Environment.NewLine + 
                e.Detail, EventLogEntryType.Error);
        }        
       
        protected override void OnStart(string[] args)
        {
            try
            {
                m_EmsService.Start();
            }
            catch (Exception ex)
            {
                m_EL.WriteEntry(ex.Message + Environment.NewLine +  ex.StackTrace, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            try
            {
                m_EmsService.Stop();
            }
            catch (Exception ex)
            {
                m_EL.WriteEntry(ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
            }

        }
    }
}
