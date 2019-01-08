using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using Rohm.Ems;
using System.IO;
using System.Xml.Serialization;

namespace EmsServer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private EmsService m_Service;
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            m_Service = new EmsService("Data Source=10.28.33.11;Initial Catalog=DBx;User ID=emsuser;Password='em$u$er';Persist Security Info=True;");            
            m_Service.ErrorCatched += new ErrorCatchedEventHandler(m_Service_ErrorCatched);
            startServiceWorker.RunWorkerAsync();
        }

        void m_Service_ErrorCatched(object sender, ErrorCatchedEventArgs e)
        {
            MessageBox.Show(e.ErrorCode + "-->" + e.Ex.Message + Environment.NewLine +
                e.Ex.StackTrace + Environment.NewLine +
                e.Detail);
        }

        #region "Background Starting Service"

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                m_Service.Start();
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void startServiceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                textBox1.Text = "service is started !!" + Environment.NewLine;
            }
            else
            {
                textBox1.Text = e.Result.ToString();
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            m_Service.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            startServiceWorker.RunWorkerAsync();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_Service.Stop();
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //m_Service.Test();
        }
        
    }
}
