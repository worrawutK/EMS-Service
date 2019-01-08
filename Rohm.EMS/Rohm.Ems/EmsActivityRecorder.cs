using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rohm.Ems
{
    public class EmsActivityRecorder
    {
        private Dictionary<string, EmsActivityRecordRow> m_RecordList;
        
        public EmsActivityRecorder() {
            m_RecordList = new Dictionary<string, EmsActivityRecordRow>();
        }

        public string GetActivityRecordUniqueKey(string processNo, string mcNo, string activityName, DateTime rohmDate)
        {
            string key = processNo + mcNo + activityName + rohmDate.ToString("yyyyMMdd");
            return key;
        }

        public void RecordActivityBeforeChange(EmsMachineRow mc, DateTime timeStamp)
        {
            if (string.IsNullOrEmpty(mc.CurrentActivityName))
            {
                return;
            }
            //วันเวลาที่เคยบันทึกก่อนหน้า
            DateTime lastActivityChangedTimeStamp = mc.ActivityChangeTime;
            //Rohm date ของเวลาที่มีการบันทึก
            RohmDate timeStampRohmDate = RohmDate.FromDateTime(timeStamp);
            //Rohm date ของเวลาที่เคยบันทึกก่อนหน้า
            RohmDate lastActivityStampRohmDate = RohmDate.FromDateTime(lastActivityChangedTimeStamp);
            //ถ้าเป็นคนละวันกัน ให้ทำการบันทึกแยก
            if (timeStampRohmDate.Date != lastActivityStampRohmDate.Date)
            {
                //ทำการหา Duraition เพื่อปิดท้ายให้กับวันนั้น
                TimeSpan d1 = mc.ActivityChangeTime.Subtract(lastActivityStampRohmDate.EndDate).Duration();
                PrivateRecordActivity(mc.ProcessName, mc.MCNo, mc.CurrentActivityName, 
                    mc.CurrentActivityCategoryName, lastActivityStampRohmDate, d1);
                //กรณีเป็นคนละวันกันให้เปลี่ยน เวลาที่เคยบันทึกล่าสุดเป็น เวลาเริ่มต้นของวัน
                lastActivityChangedTimeStamp = timeStampRohmDate.StartDate; 
            }

            TimeSpan d2 = lastActivityChangedTimeStamp.Subtract(timeStamp).Duration();
            PrivateRecordActivity(mc.ProcessName, mc.MCNo, mc.CurrentActivityName, 
                mc.CurrentActivityCategoryName, timeStampRohmDate, d2);
            
        }

        private void PrivateRecordActivity(string processName, string mcNo, string activityName, string activityCategoryName, RohmDate rohmDate, TimeSpan duration)
        {
            string key = GetActivityRecordUniqueKey(processName, mcNo, activityName, rohmDate.Date);

            EmsActivityRecordRow act = null;

            if (m_RecordList.ContainsKey(key))
            {
                act = m_RecordList[key];
            }
            else
            {
                act = new EmsActivityRecordRow();
                act.ProcessName = processName;
                act.MCNo = mcNo;
                act.ActivityName = activityName;
                act.ActivityCategoryName = activityCategoryName;
                act.Duration = 0;
                act.RohmDate = rohmDate.Date;
                m_RecordList.Add(key, act);
            }

            act.Duration += duration.TotalMinutes;
        }

        public void LoadActivity(EmsActivityRecordRow[] rows)
        {
            foreach (EmsActivityRecordRow act in rows)
            {
                string key = GetActivityRecordUniqueKey(act.ProcessName, act.MCNo, act.ActivityName, act.RohmDate);
                if (!m_RecordList.ContainsKey(key))
                {
                    m_RecordList.Add(key, act);
                }
                else
                {
                    m_RecordList[key] = act;
                }
            }
        }

        public EmsActivityRecordRow[] GetActivityRecordRows()
        {
            return m_RecordList.Values.ToArray();
        }

        public void Clear()
        {
            m_RecordList.Clear();
        }

        public void ClearDataBefore(DateTime rohmDate)
        {
            List<string> clearList = new List<string>();
            foreach (EmsActivityRecordRow act in m_RecordList.Values.ToArray())
            {
                if (act.RohmDate < rohmDate)
                {
                    clearList.Add(GetActivityRecordUniqueKey(act.ProcessName, act.MCNo, act.ActivityName, act.RohmDate));
                }
            }
            foreach (string key in clearList)
            {
                m_RecordList.Remove(key);
            }
            clearList.Clear();
        }

    }
}
