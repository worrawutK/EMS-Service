using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmsConsoleClient.ServiceReference1;
using System.Threading;

namespace EmsConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i <= 1000; i++)
            {
                Thread t = new Thread(ThreadWork);
                t.IsBackground = true;
                t.Start();
            }

            Console.Read();
            Console.WriteLine("Successed = " + m_SuccessCount.ToString());
            Console.Read();

        }

        private static object m_Locker = new object();
        private static int m_SuccessCount = 0;

        static void ThreadWork()
        {
            try
            {
                ReporterClient rc = new ReporterClient();
                rc.ReportActivity(0, 0, 0);
                Console.WriteLine("Report activity");
                lock (m_Locker)
                {
                    m_SuccessCount = m_SuccessCount + 1;
                }
            }
            catch(TimeoutException te)
            { 
                
            }
        }
    }
}
