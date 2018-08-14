using System;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Linq;

namespace G7_Debug_Viewer
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really Quit?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                Application.Exit();

            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.ShowDialog();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
           
            
            if (ServiceExists("mysql"))
            {
                Indicator.Start();
            }
            else
            {
                IndicatorStatusBox.ForeColor = System.Drawing.Color.Red;
                IndicatorStatusBox.Text = "Service Not Installed";
            }
            
        }

        private bool ServiceExists(string ServiceName)
        {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(ServiceName));
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            Indicator.Stop();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            Indicator.Stop();
            Indicator.Start();
        }


        
    }
}
