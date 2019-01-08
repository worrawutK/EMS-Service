using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rohm.Ems
{
    public interface ILogger
    {
        void Log(string function, string position, string errMessage, string comment);
    }
}
