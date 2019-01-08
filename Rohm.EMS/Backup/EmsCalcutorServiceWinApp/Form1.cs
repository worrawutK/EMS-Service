using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rohm.Ems;

namespace EmsCalcutorServiceWinApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private TotalEfficiencyRateCalculatorService m_Service;

        private void Form1_Load(object sender, EventArgs e)
        {
            m_Service = new TotalEfficiencyRateCalculatorService(EmsCalcutorServiceWinApp.Properties.Settings.Default.DBxConnectionString);
            //m_Service.Start();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            label1.Text = "Calculation start at :=" + DateTime.Now.ToString();
            backgroundWorker1.RunWorkerAsync(dateTimePicker1.Value.Date);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            m_Service.Calculate((DateTime)e.Argument);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "TME CALCULATOR SERVICE";
        }
        
    }
}
