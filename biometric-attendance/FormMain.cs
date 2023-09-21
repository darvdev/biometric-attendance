using BiometricAttendance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace biometric_attendance
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private FormSettings formSettings;
        private FormEnroll formEnroll;
        private FormEmployee formEmployee;
        private FormEmployeeList formEmployeeList;
        private FormAttendance formAttendance;

        public readonly SerialPort serial = new SerialPort();
        public IniFile settings = new IniFile();
        
        public string[] ports = Helper.GetPorts();
        public ModelEmployee[] employees = Array.Empty<ModelEmployee>();

        public bool connected = false;
        public bool enrolling = false;

        public string port = "";
        public string connectionString = "";

        private void Main_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            serial.DtrEnable = true;
            serial.DataReceived += SerialPort_DataReceived;

            //portName = settings.Read("Port");


            //if (String.IsNullOrEmpty(portName))
            //{
            //    settings.Write("Port", "COM1");

            //    portName = settings.Read("Port");

            //    Console.WriteLine("Port Name is Empty: {0}", portName);
            //}

            Task.Run(() =>
            {
                employees = Helper.GetEmployeeList(connectionString);

                this.Invoke((MethodInvoker)delegate {
                    labelTotal.Text = employees.Length.ToString();
                });
            });

            Task.Run(async () => {

                port = settings.Read("Port");

                if (!string.IsNullOrEmpty(port) && ports.Length > 0)
                {
                    int index = Array.IndexOf(ports, port);
                    if (index > -1) {
                        await SerialConnect();
                    }
                    
                }

            });

        }

        //Open Forms
        private void OpenFormSettings(object sender, EventArgs e)
        {
            formSettings = new FormSettings();
            formSettings.ShowDialog();
        }

        private void OpenFormEmployee(object sender, EventArgs e)
        {
            formEmployee = new FormEmployee();
            formEmployee.ShowDialog();

        }

        private void OpemFormEmployeeList(object sender, EventArgs e)
        {
            formEmployeeList = new FormEmployeeList();
            formEmployeeList.ShowDialog();
        }

        private void OpenFormEnroll(object sender, EventArgs e)
        {
            formEnroll = new FormEnroll();
            formEnroll.ShowDialog();
        }


        //Methods
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string rawData = serial.ReadTo(serial.NewLine);
                string data = rawData.Remove(rawData.Length - 1);
                Console.WriteLine("received: {0}", data);

                if (data[0].ToString() == "$")
                {
                    Console.WriteLine("Implement this command: {0}", data);
                }
                else
                {
                    if (data.Contains("="))
                    {
                        int index = data.IndexOf("=");
                        string command = data.Substring(0, index);
                        string value = data.Replace(command + "=", "");
                        value.Remove(value.Length - 1);

                        if (command == "status")
                        {
                            if (value == "0")
                            {
                                this.Invoke((MethodInvoker)delegate {
                                    enrollToolStripMenuItem.Enabled = true;
                                });
                            }

                            this.Invoke((MethodInvoker)delegate {
                                labelStatus.Text = value;
                            });

                        }

                    }
                    else
                    {

                        switch (data)
                        {
                            case "ok":
                                connected = true;
                                serial.WriteLine("status");
                                settings.Write("Port", port);
                                
                                this.Invoke((MethodInvoker)delegate {
                                    labelConnection.Text = "Connected";
                                });

                                break;
                            case "enroll":
                                enrolling = true;
                                break;
                            default:
                                enrolling = false;
                                break;
                        }

                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("SerialPort_DataReceived error: " + ex.Message);
            }
        }

        public async Task<bool> SerialConnect()
        {
            Console.WriteLine("Connecting...");
            try
            {
                serial.PortName = port;
                serial.Open();
                await Task.Delay(2000);
                Console.WriteLine("send: connect");
                serial.WriteLine("connect");
                await Task.Delay(1000);

                if (!connected)
                {
                    this.Invoke((MethodInvoker)delegate {
                        labelConnection.Text = "Connected - No response";
                    });
                }

                return true;

            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
                Console.WriteLine("SerialConnect error:" + ex.Message);
            }

            return false;
        }

        public async Task<bool> SerialDisconnect()
        {
            Console.WriteLine("Disconnecting...");
            try
            {
                Console.WriteLine("send: standby");
                serial.WriteLine("standby");
                await Task.Delay(1000);
                serial.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerialDisconnect error:" + ex.Message);
                await Task.Delay(1000);
            }
            return false;
        }
    }
}
