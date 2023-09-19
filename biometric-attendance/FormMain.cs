using BiometricAttendance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
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

        //private const string connectionString = "Data Source=.\\database.db;Version=3";
        //private const string providerName = "System.Data.SqlClient";

        public string connectionString = "";

        private FormSettings formSettings;
        private FormEnroll formEnroll;
        private FormEmployee formEmployee;
        private FormEmployeeList formEmployeeList;
        private FormAttendance formAttendance;

        public readonly SerialPort serialPort = new SerialPort();
        public string portName = "";

        public ModelEmployee[] employees = Array.Empty<ModelEmployee>();

        private void Main_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            serialPort.DtrEnable = true;
            serialPort.DataReceived += SerialPort_DataReceived;

            Task.Run(() =>
            {
                employees = Helper.GetEmployeeList(connectionString);

                this.Invoke((MethodInvoker)delegate {
                    labelTotalEmployees.Text = $"Total: {employees.Length} employees";
                });

            });

        }


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string rawData = serialPort.ReadTo(serialPort.NewLine);

                string data = rawData.Remove(rawData.Length-1);

                Console.WriteLine("received: {0}", data);

                if (data[0].ToString() == "$")
                {
                    Console.WriteLine(data);
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
                        }

                    }
                    else
                    {
                    
                    }
                    
                }

                
            }
            catch (Exception err)
            {
                Console.WriteLine("Error serial event: " + err);
            }
        }

        public async Task<bool> SerialConnect()
        {
            Console.WriteLine("Connecting...");
            try
            {
                Console.WriteLine("Port Name: " + portName);
                serialPort.PortName = portName;
                serialPort.Open();
                Console.WriteLine("Connected!");
                await Task.Delay(3000);
                Console.WriteLine("Send=status");
                serialPort.WriteLine("status");
                await Task.Delay(1000);
                return true;
                
            }
            catch (Exception e)
            {
                await Task.Delay(1000);
                Console.WriteLine("Error: $e" + e);
            }

            return false;
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


    }
}
