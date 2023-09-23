using BiometricAttendance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
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
        private FormAttendanceList formAttendanceList;

        public readonly SerialPort serial = new SerialPort();
        public IniFile settings = new IniFile();

        public string[] ports = Helper.GetPorts();
        public ModelEmployee[] employeeList = Array.Empty<ModelEmployee>();
        public ModelAttendance[] attendaceList = Array.Empty<ModelAttendance>();

        public bool connected = false;
        public bool enrolling = false;

        public string port = "";
        public string connectionString = "";
        public int employeeIndex = -1;

        private Timer timer = new Timer();


        private void Invoke(Action method)
        {
            this.Invoke((MethodInvoker)delegate { method(); });
        }

        private void Main_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            serial.DtrEnable = true;
            serial.DataReceived += SerialPort_DataReceived;
            timer.Interval = 1000;
            timer.Tick += TimerTick;
            timer.Start();


            GetEmployeeList();
            GetAttendanceList();
            
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
            GetEmployeeList();

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
            GetEmployeeList();
        }

        private void OpenFormAttendance(object sender, EventArgs e)
        {
            formAttendance = new FormAttendance();
            this.Hide();
            formAttendance.Show();
        }

        private void OpenFormAttendaceList(object sender, EventArgs e)
        {
            formAttendanceList = new FormAttendanceList();
            formAttendanceList.ShowDialog();
        }

        //Private Methods
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine("------------------------------------------------------------");
                string rawData = serial.ReadTo(serial.NewLine);
                string data = rawData.Remove(rawData.Length - 1);
                Console.WriteLine("received: {0}", data);

                if (data[0].ToString() == "$")
                {
                    int index = data.IndexOf("|");
                    string command = data.Substring(0, index);
                    string value = data.Replace(command + "|", "");

                    if (command == "$enroll")
                    {
                        if (value.Contains("="))
                        {
                            int valueIndex = value.IndexOf("=");
                            string query = value.Substring(0, valueIndex);
                            string queryValue = value.Replace(query + "=", "");

                            Console.WriteLine("Query: {0}, Value: {1}", query, queryValue);

                            if (query == "id")
                            {
                                EnrollEmployee(queryValue);
                            }

                        }
                        else
                        {
                            if (value == "ok")
                            {
                                Invoke(() =>
                                {
                                    formEnroll?.EnrollStatus("Remove finger in the Sensor");
                                });
                            }
                            else if (value == "next")
                            {
                                Invoke(() =>
                                {
                                    formEnroll?.EnrollStatus("Place same finger in the Sensor");
                                });

                            }
                            else
                            {
                                Invoke(() =>
                                {
                                    formEnroll?.EnrollStatus(value);
                                });

                            }
                        }

                    }
                    else if (command == "$start")
                    {
                        if (value.Contains("="))
                        {

                            if (value.Contains(','))
                            {
                                //$start|id=1,confidence=118
                                Console.WriteLine("Implement multiple value");
                            }
                            else
                            {
                                //$start|code=23
                                int valueIndex = value.IndexOf("=");
                                string query = value.Substring(0, valueIndex);
                                string queryValue = value.Replace(query + "=", "");

                                if (query == "id")
                                {
                                    AddAttendance(queryValue);
                                }
                                else if (query == "code")
                                {
                                    Invoke(() =>
                                    {
                                        formAttendance?.UpdateBiometric($"Error: {queryValue}");
                                    });
                                }
                                else
                                {
                                    Invoke(() =>
                                    {
                                        formAttendance?.UpdateBiometric($"Error: {queryValue}");
                                    });
                                }
                            }


                        }
                        else
                        {
                            Console.WriteLine("Implement: start without =");
                        }
                    }
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

                            if (value == "0" || value == "301")
                            {
                                Invoke(() => {
                                    enrollToolStripMenuItem.Enabled = true;
                                    if (value == "0")
                                    {
                                        OpenFormAttendance(null, null);
                                    }
                                });
                            }

                            Invoke(() => {
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

                                this.Invoke((MethodInvoker)delegate {
                                    formEnroll?.EnrollStatus("Place finger in the Sensor");
                                });

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

        private void EnrollEmployee(string id)
        {
            try
            {
                Invoke(() => {
                    formEnroll?.EnrollStatus($"Enrolling employee in biometric id: {id}");
                });

                int index = int.Parse(id);
                var ee = employeeList.ElementAt(employeeIndex);

                Console.WriteLine("Employee: {0}, ID: {1}", ee.name, ee.id);
                bool result = Helper.EnrollEmployee(connectionString, ee.id, index, ee.employee_id);

                if (result)
                {

                    Invoke(() => {
                        formEnroll?.EnrollStatus($"Success enrollment: {id}");
                    });
                }
                else
                {
                    Invoke(() => {
                        formEnroll?.EnrollStatus($"Employee cannot enroll biometric: {id}");
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("EnrollEmployee error: {0}", ex.Message);
                Invoke(() => {
                    formEnroll?.EnrollStatus($"Error registering employee: {ex.Message}");
                });
            }
        }

        private void AddAttendance(string id)
        {
            Invoke(() => {
                formAttendance?.UpdateBiometric($"Add attendance id of {id}");
            });
            int index = int.Parse(id);
            var result = Helper.AddAttendance(connectionString, index);

            if (result)
            {
                GetAttendanceList();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {

            var today = DateTime.Now;

            var date = today.ToShortDateString();
            var time = today.ToLongTimeString();

            formAttendance?.UpdateDateTime(date + "\n" + time);

        }

        //Public Methods
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

        public void GetEmployeeList()
        {
            Task.Run(() =>
            {
                employeeList = Helper.GetEmployeeList(connectionString);
                Invoke(() => {
                    labelTotal.Text = employeeList.Length.ToString();
                });
            });
        }

        public void GetAttendanceList()
        {
            Task.Run(() =>
            {
                attendaceList = Helper.GetAttendaceList(connectionString);
                formAttendance?.DisplayAttendance();
            });
        }
        
    }

}
