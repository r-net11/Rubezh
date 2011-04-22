using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Container
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string DriverId { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            activeX.DriverId = "1E045AD6-66F9-4F0B-901C-68C46C89E8DA";
            activeX.SetDriverId(DriverId);
            activeX.Width = this.Width -100;
            activeX.Height = this.Height -100;
        }
    }
}
