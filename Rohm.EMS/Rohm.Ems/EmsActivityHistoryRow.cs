using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rohm.Ems
{
    public class EmsActivityHistoryRow
    {
        public int ID { get; set; }
        public string ProcessName { get; set; }
        public string MCNo { get; set; }
        public string ActivityName { get; set; }
        public string ActivityCategoryName { get; set; }
        public DateTime RecordTime { get; set; }

        public EmsActivityHistoryRow(DataRow row)
        { 
            //ID	    ProcessName	MCNo	ActivityName	    ActivityCategoryName	RecordTime
            //29344505	PE	        PC-01	xxx	                ChokotieLoss	        2017-04-25 15:35:36.537
            //29344506	PE	        PC-01	Stop Button Pressed	StopLoss	            2017-04-26 10:34:18.690
            this.ID = (int)row["ID"];
            this.ProcessName = (string)row["ProcessName"];
            this.MCNo = (string)row["MCNo"];
            this.ActivityName = (string)row["ActivityName"];
            this.ActivityCategoryName = (string)row["ActivityCategoryName"];
            this.RecordTime = (DateTime)row["RecordTime"];

        }
    }
}
