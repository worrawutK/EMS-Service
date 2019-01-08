using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rohm.Ems;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //EmsServiceClient c_EmsService = new EmsServiceClient("xxx","http://localhost:7777/EmsService");
            EmsServiceClient c_EmsService = new EmsServiceClient("http://webserv.thematrix.net:7777/EmsService");
            EmsMachineRegisterInfo emsMachineRegisterInfo = new EmsMachineRegisterInfo("TWE-00", "MP-TWE-00", "MP", "Y1E3120-4P", "", 0, 0, 0, 0, 0);
            c_EmsService.Register(emsMachineRegisterInfo);
        }
    }
}
