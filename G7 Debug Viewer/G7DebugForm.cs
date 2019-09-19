using System;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimS.Telnet;
using System.Text.RegularExpressions;

namespace G7_Debug_Viewer
{
    public partial class G7DebugForm : Form
    {
        string datafromconfig;
        string IndicatorIP;
        string IndicatorPORT;
        string IndicatorConnected;
        string GSMIP;
        string GSMPORT;
        string GSMConnected;
        string IMPEXPIP;
        string IMPEXPPORT;
        string IMPEXPConnected;
        string SMTPIP;
        string SMTPPORT;
        string SMTPConnected;
        int TimeoutMs;


        public G7DebugForm()
        {
            InitializeComponent();
         //   IndicatorPORT = ReadXML(@"c:\molen G7\services\Preciamolen.Indicator.Service.config","Preciamolen.Indicator.Service","Debug","Port");
         //   IndicatorPorttxt.Text = IndicatorPORT;
         //   IndicatorIP = ReadXML(@"c:\molen G7\services\Preciamolen.Indicator.Service.config", "Preciamolen.Indicator.Service", "Debug","Address");
         //   IndicatorIPtxt.Text = IndicatorIP;
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
            ServiceController sc = new ServiceController
            {
                ServiceName = ServiceName
            };

            IndicatorStatusBox.Text="The {0} service status is currently set to {1}" + ServiceName.ToString() + sc.Status.ToString();

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                // Start the service if the current status is stopped.
                IndicatorStatusBox.Text = "Starting the {0} service ..." + ServiceName.ToString();
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);

                    // Display the current service status.
                    IndicatorStatusBox.Text = "The {0} service status is now set to {1}." + ServiceName.ToString() + sc.Status.ToString();
                }
                catch (InvalidOperationException e)
                {
                    IndicatorStatusBox.Text = "Could not start the {0} service." + ServiceName.ToString();
                    IndicatorStatusBox.Text = e.Message;
                }
            }
            else
            {
                IndicatorStatusBox.Text = "Service {0} already running." + ServiceName.ToString();
            }
        }

        /// <summary>
        /// Stop a service that is active
        /// </summary>
        /// <param name="ServiceName"></param>
        public void StopService(string ServiceName)
        {
            ServiceController sc = new ServiceController
            {
                ServiceName = ServiceName
            };

            IndicatorStatusBox.Text="The {0} service status is currently set to {1}" + ServiceName + sc.Status.ToString();

            if (sc.Status == ServiceControllerStatus.Running)
            {
                // Start the service if the current status is stopped.
                IndicatorStatusBox.Text = "Stopping the {0} service ..." + ServiceName;
                try
                {
                    // Stop the service, and wait until its status is "Stopped".
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);

                    // Display the current service status.
                    IndicatorStatusBox.Text = "The {0} service status is now set to {1}." + ServiceName + sc.Status.ToString();
                }
                catch (InvalidOperationException e)
                {
                    IndicatorStatusBox.Text = "Could not stop the {0} service." + ServiceName;
                    IndicatorStatusBox.Text = e.Message;
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
            ServiceController sc = new ServiceController
            {
                ServiceName = ServiceName
            };

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

        /// <summary>
        ///  Read XML Config file for service and search for parameter.
        /// </summary>
        /// <param name="FilePath (of XML Config file for service) as String, Element (to find) as string, returns parameter as String (should be value of element searched for)"></param>

        public string ReadXML(string FilePath, string root, string Element,string info)
        {
            datafromconfig = null;

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentException("Filpath is blank", nameof(FilePath));
            }
            else
            {
                 XDocument doc = XDocument.Load(FilePath);
                // datafromconfig = doc.Descendants(root).Descendants(Element).Elements(info)ToString();
                // string FilterRoot = doc.Descendants(root).ToString();
                // MessageBox.Show(FilterRoot);
                // string FilterElement = doc.Descendants(Element).ToString();
                // MessageBox.Show(FilterElement);
                // string FilterInfo = doc.Descendants(info).ToString();
                // MessageBox.Show(FilterInfo);

                IEnumerable<XElement> rows = doc.Descendants().Where(d => d.Name == "Preciamolen.Indicator.Service"
                               && d.Descendants().Any(e => e.Name == "Debug"
                               && d.Descendants().Any(f => f.Name == "Port")));
                                 // && e.Value == "Port"));



                XElement configfile = XElement.Load(FilePath);
                XElement retrieved = configfile.Element(info);
                datafromconfig = retrieved.Value;

            }


            if (string.IsNullOrWhiteSpace(datafromconfig))
            {
                throw new ArgumentException("XML Element not found", nameof(info));
            }
            else
            {
            return datafromconfig;
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

        private void Button1_Click(object sender, EventArgs e)
        {
            //Connect to telnet server using details in indicatoriptxt indicatorporttxt
            //Chuck anythng that's received into the text box and make sure that you can expand the size of the text box


        }

        public async Task ReadmeExample()
        {

            Client client = new Client(IndicatorIP, Convert.ToInt32( IndicatorPORT), new System.Threading.CancellationToken());

            {
                TimeoutMs = 1000;
                client.IsConnected.Should().Be(true);
                (await client.TryLoginAsync("username", "password", TimeoutMs)).Should().Be(true);
                client.WriteLine("show statistic wan2");
                string s = await client.TerminatedReadAsync(">", TimeSpan.FromMilliseconds(TimeoutMs));
                s.Should().Contain(">");
                s.Should().Contain("WAN2");
                Regex regEx = new Regex("(?!WAN2 total TX: )([0-9.]*)(?! GB ,RX: )([0-9.]*)(?= GB)");
                 decimal tx = decimal.Parse(matches[0].Value);
                decimal rx = decimal.Parse(matches[1].Value);
                (tx + rx).Should().BeLessThan(50);
            }
            }
        }
    }

