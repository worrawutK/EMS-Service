using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;

namespace Rohm.Ems
{
    public class DBxDatabase
    {
        public DBxDatabase()
        {
        }

        private string m_ConnectionString;
        public string ConnectionString
        {
            get { return m_ConnectionString; }
            set
            {
                m_ConnectionString = value;
            }
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(m_ConnectionString);
        }

        public DataTable ExecuteDataTable(string sqlCommand)
        {
            SqlCommand command = new SqlCommand(sqlCommand);
            return ExecuteDataTable(command);
        }

        public DataTable ExecuteDataTable(SqlCommand command)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = CreateConnection())
            {
                connection.Open();
                using (command)
                {
                    command.Connection = connection;
                    table.Load(command.ExecuteReader());
                }
            }
            return table;
        }
                
        public int ExecuteNonQuery(SqlCommand command)
        {
            int affectedRow = 0;
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                affectedRow = command.ExecuteNonQuery();
            }
            return affectedRow;
        }

        public object ExecuteScalar(SqlCommand command)
        {
            object value = 0;
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                value = command.ExecuteScalar();
            }
            return value;
        }

        public EmsOutputRecordRow[] LoadOutputRecord(string processName, string mcNo, DateTime rohmDate)
        {
            List<EmsOutputRecordRow> outputRows = new List<EmsOutputRecordRow>();

            string sqlCmd2 = "SELECT * FROM EMS.OutputRecord WHERE RohmDate = '" +
                        rohmDate.ToString("yyyy/MM/dd") + "' AND ProcessName = '" +
                        processName + "' AND MCNo = '" + mcNo + "'";

            using (DataTable dt2 = ExecuteDataTable(sqlCmd2))
            {
                foreach (DataRow row in dt2.Rows)
                {
                    EmsOutputRecordRow eor = new EmsOutputRecordRow(row);
                    outputRows.Add(eor);
                }
            }
            return outputRows.ToArray();
        }

        public EmsActivityRecordRow[] LoadActivityRecord(string processName, string mcNo, DateTime rohmDate)
        {
            List<EmsActivityRecordRow> ret = new List<EmsActivityRecordRow>();
            string sqlCmd3 = "SELECT * FROM EMS.ActivityRecord WHERE RohmDate = '" +
                        rohmDate.ToString("yyyy/MM/dd") + "' AND ProcessName = '" +
                        processName + "' AND MCNo = '" + mcNo + "'";

            using (DataTable dt3 = ExecuteDataTable(sqlCmd3))
            {
                foreach (DataRow row in dt3.Rows)
                {
                    EmsActivityRecordRow ear = new EmsActivityRecordRow(row);
                    ret.Add(ear);
                }
            }
            return ret.ToArray();
        }

        public int CountActivityHistory(DateTime rohmDate, string processName, string mcNo, string activityCategoryName)
        {
            int ret = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EMS.ActivityHistory " +
                    "WHERE (RecordTime BETWEEN @Date1 AND @Date2) AND (MCNo = @MCNo) " +
                    "AND (ProcessName = @ProcessName) AND (ActivityCategoryName = @ActivityCategoryName)";
                cmd.Parameters.Add("@Date1", SqlDbType.DateTime).Value = rohmDate.AddHours(8); //2017-07-07 --> 2017-07-07 8:00
                cmd.Parameters.Add("@Date2", SqlDbType.DateTime).Value = rohmDate.AddHours(8).AddDays(1); //2017-07-07 --> 2017-07-07 8:00 --> 2017-07-08 8:00
                cmd.Parameters.Add("@MCNo", SqlDbType.VarChar, 15).Value = mcNo;
                cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10).Value = processName;
                cmd.Parameters.Add("@ActivityCategoryName", SqlDbType.VarChar, 50).Value = activityCategoryName;

                ret = (int)ExecuteScalar(cmd);
            }
            return ret;
        }

        public EmsMachineRow[] LoadAllEmsMachine()
        {
            List<EmsMachineRow> ret = new List<EmsMachineRow>();

            string sqlCmd1 = "SELECT [ID],[MCNo],[RegisteredDate],[CurrentActivityName],[CurrentActivityCategoryName]" + 
                ",[AreaName],[ProcessName],[MachineTypeName],[CurrentLotNo],[CurrentTotalGood],[CurrentTotalNG]" + 
                ",[CurrentStandardRPM],[LastUpdateDate],[CutTotalGood],[CutTotalNG],[ActivityChangeTime], [AlarmCount], [BMCount] " +
                "FROM EMS.Machine";
            using (DataTable dt1 = ExecuteDataTable(sqlCmd1))
            {
                foreach(DataRow row in dt1.Rows)
                {
                    ret.Add(new EmsMachineRow(row));
                }
            }
            return ret.ToArray();
        }

        public int SaveEmsMachineRow(EmsMachineRow data)
        {
            int aff = 0;            

            data.LastUpdateDate = DateTime.Now;

            bool updateIDRequire = (data.ID == -1);
                        
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.SAVE_EMS_MACHINE";

            cmd.Parameters.Add("@ID", SqlDbType.Int);
            cmd.Parameters["@ID"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters["@ID"].Value = data.ID;
            
	        cmd.Parameters.Add("@MCNo", SqlDbType.VarChar,15).Value = data.MCNo;
	        cmd.Parameters.Add("@RegisteredDate", SqlDbType.DateTime).Value = data.RegisteredDate;
	        cmd.Parameters.Add("@CurrentActivityName", SqlDbType.VarChar, 50).Value = data.CurrentActivityName;
	        cmd.Parameters.Add("@CurrentActivityCategoryName", SqlDbType.VarChar, 50).Value = data.CurrentActivityCategoryName;
	        cmd.Parameters.Add("@AreaName", SqlDbType.VarChar, 50).Value = data.AreaName;
	        cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10).Value = data.ProcessName;
	        cmd.Parameters.Add("@MachineTypeName", SqlDbType.VarChar, 20).Value = data.MachineTypeName;
	        cmd.Parameters.Add("@CurrentLotNo", SqlDbType.VarChar, 10).Value = (data.CurrentLotNo==null?"":data.CurrentLotNo);
	        cmd.Parameters.Add("@CurrentTotalGood", SqlDbType.Int).Value = data.CurrentTotalGood;
	        cmd.Parameters.Add("@CurrentTotalNG", SqlDbType.Int).Value = data.CurrentTotalNG;
	        cmd.Parameters.Add("@CurrentStandardRPM", SqlDbType.Float).Value = data.CurrentStandardRPM;
	        cmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime).Value = data.LastUpdateDate;
	        cmd.Parameters.Add("@CutTotalGood", SqlDbType.Int).Value = data.CutTotalGood;
	        cmd.Parameters.Add("@CutTotalNG", SqlDbType.Int).Value = data.CutTotalNG;
	        cmd.Parameters.Add("@ActivityChangeTime", SqlDbType.DateTime).Value = data.ActivityChangeTime;
	        cmd.Parameters.Add("@AlarmCount", SqlDbType.Int).Value = data.AlarmCount;
	        cmd.Parameters.Add("@BMCount", SqlDbType.Int).Value = data.BMCount;

            aff = ExecuteNonQuery(cmd);

            if (aff == 1 && updateIDRequire)
            {
                data.ID = (int)cmd.Parameters["@ID"].Value;
            }

            return aff;
        }

        //insert only
        public int InsertEmsOutput(EmsOutputRecordRow o)
        {
            int aff = 0;

            string sqlCmd = "INSERT INTO [EMS].[OutputRecord]" +
                "([RohmDate],[ProcessName],[MCNo],[LotNo],[TotalGood],[TotalNG],[StandardRPM])" +
                " VALUES (@RohmDate,@ProcessName,@MCNo,@LotNo,@TotalGood,@TotalNG,@StandardRPM)";
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.Parameters.Add("@RohmDate", SqlDbType.Date);
                cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10);
                cmd.Parameters.Add("@MCNo", SqlDbType.VarChar, 15);
                cmd.Parameters.Add("@LotNo", SqlDbType.VarChar, 10);
                cmd.Parameters.Add("@TotalGood", SqlDbType.Int);
                cmd.Parameters.Add("@TotalNG", SqlDbType.Int);
                cmd.Parameters.Add("@StandardRPM", SqlDbType.Float);

                cmd.Parameters[0].Value = o.RohmDate;
                cmd.Parameters[1].Value = o.ProcessName;
                cmd.Parameters[2].Value = o.MCNo;
                cmd.Parameters[3].Value = o.LotNo;
                cmd.Parameters[4].Value = o.TotalGood;
                cmd.Parameters[5].Value = o.TotalNG;
                cmd.Parameters[6].Value = o.StandardRPM;

                aff = ExecuteNonQuery(cmd);
            }

            return aff;

        }

        public int SaveEmsActivityRecord(EmsActivityRecordRow a)
        {
            int aff = 0;

            string sqlCmd = "IF EXISTS (SELECT * FROM [EMS].[ActivityRecord] " +
                "WHERE ([ProcessName] = @ProcessName) AND ([MCNo] = @MCNo) " +
                "AND ([ActivityName] = @ActivityName) AND ([RohmDate] = @RohmDate)) " +
                "BEGIN UPDATE [EMS].[ActivityRecord] SET [Duration] = @Duration " +
                "WHERE ([ProcessName] = @ProcessName) AND ([MCNo] = @MCNo) " +
                "AND ([ActivityName] = @ActivityName) AND ([RohmDate] = @RohmDate) END " +
                "ELSE BEGIN INSERT INTO [EMS].[ActivityRecord] ([ProcessName],[MCNo],[ActivityName]," +
                "[ActivityCategoryName],[RohmDate],[Duration]) VALUES (@ProcessName,@MCNo,@ActivityName," +
                "@ActivityCategoryName,@RohmDate,@Duration); SET @ID = @@IDENTITY END";

            using (SqlCommand cmd1 = new SqlCommand())                  
            {
                cmd1.CommandText = sqlCmd;
                cmd1.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10);
                cmd1.Parameters.Add("@MCNo", SqlDbType.VarChar, 15);
                cmd1.Parameters.Add("@ActivityName", SqlDbType.VarChar, 50);
                cmd1.Parameters.Add("@ActivityCategoryName", SqlDbType.VarChar, 50);
                cmd1.Parameters.Add("@RohmDate", SqlDbType.Date);
                cmd1.Parameters.Add("@Duration", SqlDbType.Float);
                cmd1.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.InputOutput;

                //LBL001:                
                cmd1.Parameters[0].Value = a.ProcessName;
                cmd1.Parameters[1].Value = a.MCNo;
                cmd1.Parameters[2].Value = a.ActivityName;
                cmd1.Parameters[3].Value = a.ActivityCategoryName;
                cmd1.Parameters[4].Value = a.RohmDate;
                cmd1.Parameters[5].Value = a.Duration;
                cmd1.Parameters[6].Value = a.ID;

                aff = ExecuteNonQuery(cmd1);
                a.ID = (int)cmd1.Parameters[6].Value;
                
            }

            return aff;
        }        

        public EmsMachineRow[] GetAllMachine() {
            string strSQL = "SELECT * FROM EMS.Machine";
            List<EmsMachineRow> mcList = new List<EmsMachineRow>();
            using (DataTable dt = ExecuteDataTable(strSQL))
            {
                foreach (DataRow row in dt.Rows)
                {
                    mcList.Add(new EmsMachineRow(row));
                }
            }
            return mcList.ToArray();
        }

        public EmsMachineRow GetMachine(string processName, string mcNo)
        {
            string strSQL = "SELECT [ID],[MCNo],[RegisteredDate],[CurrentActivityName]" + 
                ",[CurrentActivityCategoryName],[AreaName],[ProcessName],[MachineTypeName]" +
                ",[CurrentLotNo],[CurrentTotalGood],[CurrentTotalNG],[CurrentStandardRPM]" +
                ",[LastUpdateDate],[CutTotalGood],[CutTotalNG],[ActivityChangeTime],[AlarmCount],[BMCount] " +
                "FROM [EMS].[Machine] WHERE [MCNo] = @MCNo AND [ProcessName] = @ProcessName";
            EmsMachineRow mc = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = strSQL;
                cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10).Value = processName;
                cmd.Parameters.Add("@MCNo", SqlDbType.VarChar, 15).Value = mcNo;
                using (DataTable dt = ExecuteDataTable(cmd))
                {

                    if (dt.Rows.Count == 1)
                    {
                        mc = new EmsMachineRow(dt.Rows[0]);
                    }
                }
            }
            return mc;
        }

        public void InsertTotalMachineEfficiency(TotalEfficiencyRateRow d)
        {
            string strSql = "INSERT INTO [EMS].[TotalMachinEfficiencyRate] ([ProcessName],[MCNo],[RohmDate],[PlanStopLoss]" +
           ",[StopLoss],[ChokotieLoss],[FreeRunningLoss],[RealOperationTime],[ValueOperationTime],[NGLoss],[MTBF],[PMMTTR]" +
           ",[PDMTTR],[PerformanceRate],[GoodRate],[TimeOperationRate],[TMERate],[TotalWorkingTime],[LoadTime],[OperationTime]" +
           ",[NetOperationTime],[TotalGood],[TotalNG],[AlarmCount],[BreakdownCount],[BreakdownTime]) VALUES (@ProcessName,@MCNo" + 
           ",@RohmDate,@PlanStopLoss,@StopLoss,@ChokotieLoss,@FreeRunningLoss,@RealOperationTime,@ValueOperationTime,@NGLoss," + 
           "@MTBF,@PMMTTR,@PDMTTR,@PerformanceRate,@GoodRate,@TimeOperationRate,@TMERate,@TotalWorkingTime,@LoadTime," +
           "@OperationTime,@NetOperationTime,@TotalGood,@TotalNG,@AlarmCount,@BreakdownCount,@BreakdownTime)";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = strSql;
                cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10);
                cmd.Parameters.Add("@MCNo", SqlDbType.VarChar, 15);
                cmd.Parameters.Add("@RohmDate", SqlDbType.Date);
                cmd.Parameters.Add("@PlanStopLoss", SqlDbType.Float);
                cmd.Parameters.Add("@StopLoss", SqlDbType.Float);
                cmd.Parameters.Add("@ChokotieLoss", SqlDbType.Float);
                cmd.Parameters.Add("@FreeRunningLoss", SqlDbType.Float);
                cmd.Parameters.Add("@RealOperationTime", SqlDbType.Float);
                cmd.Parameters.Add("@ValueOperationTime", SqlDbType.Float);
                cmd.Parameters.Add("@NGLoss", SqlDbType.Float);
                cmd.Parameters.Add("@MTBF", SqlDbType.Float);
                cmd.Parameters.Add("@PMMTTR", SqlDbType.Float);
                cmd.Parameters.Add("@PDMTTR", SqlDbType.Float);
                cmd.Parameters.Add("@PerformanceRate", SqlDbType.Float);
                cmd.Parameters.Add("@GoodRate", SqlDbType.Float);
                cmd.Parameters.Add("@TimeOperationRate", SqlDbType.Float);
                cmd.Parameters.Add("@TMERate", SqlDbType.Float);
                cmd.Parameters.Add("@TotalWorkingTime", SqlDbType.Float);
                cmd.Parameters.Add("@LoadTime", SqlDbType.Float);
                cmd.Parameters.Add("@OperationTime", SqlDbType.Float);
                cmd.Parameters.Add("@NetOperationTime", SqlDbType.Float);
                cmd.Parameters.Add("@TotalGood", SqlDbType.Int);
                cmd.Parameters.Add("@TotalNG", SqlDbType.Int);
                cmd.Parameters.Add("@AlarmCount", SqlDbType.Int);
                cmd.Parameters.Add("@BreakdownCount", SqlDbType.Int);
                cmd.Parameters.Add("@BreakdownTime", SqlDbType.Float);
                
                cmd.Parameters[0].Value = d.ProcessName;
                cmd.Parameters[1].Value = d.MCNo;
                cmd.Parameters[2].Value = d.RohmDate;
                cmd.Parameters[3].Value = d.PlanStopLoss;
                cmd.Parameters[4].Value = d.StopLoss;
                cmd.Parameters[5].Value = d.ChokotieLoss;
                cmd.Parameters[6].Value = d.FreeRunningLoss;
                cmd.Parameters[7].Value = d.RealOperationTime;
                cmd.Parameters[8].Value = d.ValueOperationTime;
                cmd.Parameters[9].Value = d.NGLoss;
                cmd.Parameters[10].Value = d.MTBF;
                cmd.Parameters[11].Value = d.PMMTTR;
                cmd.Parameters[12].Value = d.PDMTTR;
                cmd.Parameters[13].Value = d.PerformanceRate;
                cmd.Parameters[14].Value = d.GoodRate;
                cmd.Parameters[15].Value = d.TimeOperationRate;
                cmd.Parameters[16].Value = d.TMERate;
                cmd.Parameters[17].Value = d.TotalWorkingTime;
                cmd.Parameters[18].Value = d.LoadTime;
                cmd.Parameters[19].Value = d.OperationTime;
                cmd.Parameters[20].Value = d.NetOperationTime;
                cmd.Parameters[21].Value = d.TotalGood;
                cmd.Parameters[22].Value = d.TotalNG;
                cmd.Parameters[23].Value = d.AlarmCount;
                cmd.Parameters[24].Value = d.BreakdownCount;
                cmd.Parameters[25].Value = d.BreakdownTime;
                
                ExecuteNonQuery(cmd);
            }
        }

        public void InsertActivityHistory(string processName, string mcNo, string activityName, string activityCategoryName, DateTime recordTime)
        { 
            string strSQL = "INSERT INTO [EMS].[ActivityHistory] ([ProcessName],[MCNo],[ActivityName],[ActivityCategoryName]" +
                ",[RecordTime]) VALUES (@ProcessName,@MCNo,@ActivityName,@ActivityCategoryName,@RecordTime)";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = strSQL;
                cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar, 10);
                cmd.Parameters.Add("@MCNo", SqlDbType.VarChar, 15);
                cmd.Parameters.Add("@ActivityName", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@ActivityCategoryName", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@RecordTime", SqlDbType.DateTime);
                cmd.Parameters[0].Value = processName;
                cmd.Parameters[1].Value = mcNo;
                cmd.Parameters[2].Value = activityName;
                cmd.Parameters[3].Value = activityCategoryName;
                cmd.Parameters[4].Value = recordTime;
                int aff = ExecuteNonQuery(cmd);
            }
        }

        public EmsActivityRecordRow[] LoadEmsActivityRecordOfCurrentDate()
        {
            List<EmsActivityRecordRow> ret = new List<EmsActivityRecordRow>();
            RohmDate r = RohmDate.FromNow();
            string strSQL = "SELECT * FROM EMS.ActivityRecord WHERE RohmDate = @RohmDate";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = strSQL;
                cmd.Parameters.Add("@RohmDate", SqlDbType.DateTime).Value = r.Date;
                using (DataTable dt = ExecuteDataTable(cmd))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ret.Add(new EmsActivityRecordRow(row));
                    }
                }
            }
            return ret.ToArray();
        }

        public DataTable GetActivityHistory(string processName, string mcNo, DateTime rohmDate)
        { 
            DataTable table = new DataTable();

            using (SqlConnection con = CreateConnection())
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "[dbo].[GET_ACTIVITY_HISTORY]";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Process", SqlDbType.VarChar, 10).Value = processName;
                cmd.Parameters.Add("@MCNo", SqlDbType.VarChar, 15).Value = mcNo;
                cmd.Parameters.Add("@RohmDate", SqlDbType.Date).Value = rohmDate;

                table.Load(cmd.ExecuteReader());
            }

            return table;
        }
    }
}
