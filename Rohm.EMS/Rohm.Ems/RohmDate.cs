using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rohm.Ems
{
    public class RohmDate
    {
        private RohmDate() { }
        
        private DateTime m_Date;
        public DateTime Date
        {
            get { return m_Date; }
        }

        private DateTime m_StartDate;
        public DateTime StartDate
        {
            get { return m_StartDate; }
        }

        private DateTime m_EndDate;
        public DateTime EndDate
        {
            get { return m_EndDate; }
        }

        public void AddDays(int day)
        {
            m_Date = m_Date.AddDays(day);
            m_EndDate = m_EndDate.AddDays(day);
            m_StartDate = m_StartDate.AddDays(day);
        }

        public static RohmDate FromNow()
        {
            return FromDateTime(DateTime.Now);
        }

        public static RohmDate FromDateTime(DateTime dateTime)
        {
            RohmDate rd = new RohmDate();
            rd.m_Date = dateTime.AddHours(-8).Date;
            rd.m_StartDate = rd.m_Date.AddHours(8);
            rd.m_EndDate = rd.m_StartDate.AddHours(24);
            return rd;
        }

        public static RohmDate FromDate(DateTime date)
        {
            RohmDate rd = new RohmDate();
            rd.m_Date = date.Date;
            rd.m_StartDate = rd.m_Date.AddHours(8);
            rd.m_EndDate = rd.m_StartDate.AddHours(24);
            return rd;
        }
    }
}
