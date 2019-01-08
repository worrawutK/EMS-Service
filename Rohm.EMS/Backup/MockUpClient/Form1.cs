using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rohm.Ems;

namespace MockUpClient
{
    public partial class Form1 : Form
    {
        EmsServiceClient m_ServiceClient;

        public Form1()
        {
            InitializeComponent();
            m_ServiceClient = new EmsServiceClient("http://127.0.0.1:7777/EmsService/");
            m_MC = new Machine();
        }

        private Machine m_MC;
        

        private void Form1_Load(object sender, EventArgs e)
        {
            ComboBoxActivityCategory.Items.Add(TmeCategory.ChokotieLoss);
            ComboBoxActivityCategory.Items.Add(TmeCategory.NetOperationTime);
            ComboBoxActivityCategory.Items.Add(TmeCategory.PlanStopLoss);
            ComboBoxActivityCategory.Items.Add(TmeCategory.StopLoss );

            m_ServiceClient.Start();
        }


        private void ButtonRegister_Click(object sender, EventArgs e)
        {
            EmsMachineRegisterInfo emri = new EmsMachineRegisterInfo(TextBoxMCNo.Text, 
                TextBoxArea.Text,
                TextBoxProcessName.Text, 
                TextBoxMachineType.Text,
                "", 0, 0, 0, 0, 0);
            m_ServiceClient.Register(emri);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_ServiceClient.SetActivity(TextBoxProcessName.Text, TextBoxMCNo.Text, TextBoxActivityName.Text, (TmeCategory)ComboBoxActivityCategory.SelectedItem);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_ServiceClient.SetCurrentLot(TextBoxProcessName.Text, TextBoxMCNo.Text, "1234A5678V", 25);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_ServiceClient.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DBxDatabase db = new DBxDatabase();
            db.ConnectionString = "Data Source=10.28.33.11;Initial Catalog=DBxTemp;User ID=sa;Password='p@$$w0rd';Persist Security Info=True;";
            EmsActivityRecordRow earRow = new EmsActivityRecordRow();
            earRow.ID = 916149;
            earRow.ProcessName = "WB";
            earRow.MCNo = "C-019";
            earRow.ActivityName = "Load Code Waiting";
            earRow.ActivityCategoryName = "ChokotieLoss";
            earRow.RohmDate = RohmDate.FromDate(new DateTime(2013, 09, 13)).Date;
            earRow.Duration = 55.55;
            int aff = db.SaveEmsActivityRecord(earRow);

        }


        
    }
}

