using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rohm.Ems
{
    public class ErrorCatchedEventArgs
    {

        private string m_ErrorCode;
        public string ErrorCode
        {
            get { return m_ErrorCode; }
        }			

        private Exception m_Ex;
        public Exception Ex
        {
            get { return m_Ex; }
        }

        private string m_Detail;
        public string Detail
        {
            get { return m_Detail; }
        }			

        public ErrorCatchedEventArgs(string errorCode, Exception ex, string detail)
        {
            m_ErrorCode = errorCode;
            m_Ex = ex;
            m_Detail = detail;
        }
			
    }

    public delegate void ErrorCatchedEventHandler(object sender, ErrorCatchedEventArgs e);
}
