using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rohm.Ems
{
    public class EmsReportEventArgs
    {
        public string Message { get; set; }
        public string FunctionName { get; set; }
    }

    public delegate void EmsReportEventHandler(object sender, EmsReportEventArgs e);
}
