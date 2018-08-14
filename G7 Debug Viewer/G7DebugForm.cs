using System;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Linq;

namespace G7_Debug_Viewer
{
    public partial class G7DebugForm : Form
    {

        public G7DebugForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Verify if a service exists
        /// </summary>
        /// <param name="ServiceName">Service name</param>
        /// <returns></returns>
        public bool ServiceExists(string ServiceName)
        {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(ServiceName));
        }

        /// <summary>
        /// Start a service by it's name
        /// </summary>
        /// <param name="ServiceName"></param>
        public void StartService(string ServiceName)
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = ServiceName;

            IndicatorStatusBox.Text=("The {0} service status is currently set to {1}" + ServiceName.ToString() + sc.Status.ToString());

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                // Start the service if the current status is stopped.
                IndicatorStatusBox.Text = ("Starting the {0} service ..." + ServiceName.ToString());
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);

                    // Display the current service status.
                    IndicatorStatusBox.Text = ("The {0} service status is now set to {1}." + ServiceName.ToString() + sc.Status.ToString());
                }
                catch (InvalidOperationException e)
                {
                    IndicatorStatusBox.Text = ("Could not start the {0} service." + ServiceName.ToString());
                    IndicatorStatusBox.Text = (e.Message);
                }
            }
            else
            {
                IndicatorStatusBox.Text = ("Service {0} already running." + ServiceName.ToString());
            }
        }

        /// <summary>
        /// Stop a service that is active
        /// </summary>
        /// <param name="ServiceName"></param>
        public void StopService(string ServiceName)
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = ServiceName;

            IndicatorStatusBox.Text=("The {0} service status is currently set to {1}" + ServiceName + sc.Status.ToString());

            if (sc.Status == ServiceControllerStatus.Running)
            {
                // Start the service if the current status is stopped.
                IndicatorStatusBox.Text = ("Stopping the {0} service ..." + ServiceName);
                try
                {
                    // Stop the service, and wait until its status is "Stopped".
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);

                    // Display the current service status.
                    IndicatorStatusBox.Text = ("The {0} service status is now set to {1}." + ServiceName + sc.Status.ToString());
                }
                catch (InvalidOperationException e)
                {
                    IndicatorStatusBox.Text = ("Could not stop the {0} service." + ServiceName);
                    IndicatorStatusBox.Text = (e.Message);
                }
            }
            else
            {
                Console.WriteLine("Cannot stop service {0} because it's already inactive.", ServiceName);
            }
        }

        /// <summary>
        ///  Verify if a service is running.
        /// </summary>
        /// <param name="ServiceName"></param>
        public bool ServiceIsRunning(string ServiceName)
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = ServiceName;

            if (sc.Status == ServiceControllerStatus.Running)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reboots a service
        /// </summary>
        /// <param name="ServiceName"></param>
        public void RebootService(string ServiceName)
        {
            if (ServiceExists(ServiceName))
            {
                if (ServiceIsRunning(ServiceName))
                {
                    StopService(ServiceName);
                }
                else
                {
                    StartService(ServiceName);
                }
            }
            else
            {
                Console.WriteLine("The given service {0} doesn't exists", ServiceName);
            }
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
            G7DebugAbout a = new G7DebugAbout();
            a.ShowDialog();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
           
            
            if (ServiceExists("Preciamolen_Indicator_Agent"))
            {
                StartService("Preciamolen_Indicator_Agent");
            }
            else
            {
                IndicatorStatusBox.ForeColor = System.Drawing.Color.Red;
                IndicatorStatusBox.Text = "Service Not Installed";
            }
            
        }

    private void Button7_Click(object sender, EventArgs e)
        {
            if (ServiceExists("Preciamolen_Indicator_Agent"))
            {
                StopService("Preciamolen_Indicator_Agent");
            }
            else
            {
                IndicatorStatusBox.ForeColor = System.Drawing.Color.Red;
                IndicatorStatusBox.Text = "Service Not Installed";
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (ServiceExists("Preciamolen_Indicator_Agent"))
            {
                RebootService("Preciamolen_Indicator_Agent");
            }
            else
            {
                IndicatorStatusBox.ForeColor = System.Drawing.Color.Red;
                IndicatorStatusBox.Text = "Service Not Installed";
            }
        }

    }
}
