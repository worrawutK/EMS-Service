using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rohm.Ems
{
    public class TextFileLogger
        :ILogger 
    {

        public string FileName { get; set; }

        private object m_Locker;


        public TextFileLogger(string fileName)
        {
            this.FileName = fileName;
            m_Locker = new object();
        }

        #region ILogger Members

        public void Log(string function, string position, string errMessage, string comment)
        {
            lock (m_Locker)
            {
                using (StreamWriter sw = new StreamWriter(this.FileName, true))
                {
                    sw.WriteLine(
                        string.Format("DateTime:{0}, Function:{1}, Position:{2}, ErrorMessage:{3}, Comment:{4}",
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), function, position, errMessage, comment)
                        );
                }
            }
        }

        #endregion
    }
}
