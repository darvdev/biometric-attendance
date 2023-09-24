using BiometricAttendance;
using System;
using System.IO.Ports;
using System.Linq;
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
        private FormStart formStart;
        private FormAttendanceList formAttendanceList;

        public readonly SerialPort serial = new SerialPort();
        public IniFile ini = new IniFile();

        public string[] ports = Helper.GetPorts();
        public ModelEmployee[] employeeList = Array.Empty<ModelEmployee>();
        public ModelAttendance[] attendaceList = Array.Empty<ModelAttendance>();

        public bool connected = false;
   
        public string port = "";
        public string status = "-1";
        
        private Timer timer = new Timer();


        private void Invoke(Action method) => this.Invoke((MethodInvoker)delegate { method(); });

        private void Main_Load(object sender, EventArgs e)
        {
            serial.DtrEnable = true;
            serial.DataReceived += SerialPort_DataReceived;
            timer.Interval = 1000;
            timer.Tick += TimerTick;
            timer.Start();

            GetEmployeeList();
            GetAttendanceList();

            var connect = ini.Read("Connect");
            if (connect == "1") 
            {
                Task.Run(async () => {
                    port = ini.Read("Port");
                    if (!string.IsNullOrEmpty(port) && ports.Length > 0)
                    {
                        int index = Array.IndexOf(ports, port);
                        if (index > -1)
                        {
                            await SerialConnect();
                            SendStatus();

                            var start = ini.Read("Start");

                            if (start == "1") 
                            {
                                await Task.Delay(500);
                                if (status == "0")
                                {
                                    Invoke(() => OpenFormStart(null, null));
                                }
                            }
                            
                        }

                    }

                });
            }

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
            var result = formEmployee.ShowDialog();
            if (result == DialogResult.OK)
            {
                GetEmployeeList();
            }
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

        private void OpenFormStart(object sender, EventArgs e)
        {
            formStart = new FormStart();
            this.Hide();
            formStart.Show();
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

                            if (query == "id") //Fingerprint saved into sensor database.
                            {
                                Invoke(() => formEnroll?.EnrollEmployee(queryValue));
                            }
                            else
                            {
                                //query == code (Error enrolling fingerprint)
                                Invoke(() => formEnroll?.EnrollEmployee(queryValue));
                            }
                        }
                        else
                        {
                            if (value == "ok") //Enroll step 1 success - delay 2000, go to step 2
                            {
                                Invoke(() => formEnroll?.UpdateEnrollStatus("101"));
                            }
                            else if (value == "next") //Enroll Step 2 success - go to step 3
                            {
                                Invoke(() => formEnroll?.UpdateEnrollStatus("102"));
                            }
                            else
                            {
                                //Error enrolling fingerprint
                                Invoke(() => formEnroll?.UpdateEnrollStatus(value));
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
                                    Invoke(() => formStart?.UpdateBiometric(queryValue, null));
                                }
                                else
                                {
                                    Invoke(() => formStart?.UpdateBiometric(queryValue, null));
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
                            status = value;
                            Invoke(()=>{
                                labelStatus.Text = value;
                                enrollToolStripMenuItem.Enabled = value == "0" || value == "301";
                                startToolStripMenuItem.Enabled = value == "0";
                            });
                        }

                    }
                    else
                    {

                        switch (data)
                        {
                            case "ok":
                                connected = true;
                                break;

                            case "enroll":
                                Invoke(() => formEnroll?.UpdateEnrollStatus("100")); //Place finger in the sensor
                                break;
                            case "standby":
                                Invoke(() => formEnroll?.UpdateEnrollStatus("000")); //Cancel fingerprint enrollment
                                break;
                            default:
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

        private async void AddAttendance(string biometricId)
        {
            Console.WriteLine("AddAttendance");
            int index = int.Parse(biometricId);
            var attendance = await Helper.AddAttendance(index);
            Invoke(() => formStart?.UpdateBiometric("100", attendance));
            if (attendance != null)
            {
                GetAttendanceList();
            }
            
        }

        private void TimerTick(object sender, EventArgs e)
        {

            var today = DateTime.Now;

            var date = today.ToShortDateString();
            var time = today.ToLongTimeString();

            formStart?.UpdateDateTime(date + "\n" + time);

        }

        //Public Methods
        public async Task<bool> SerialConnect()
        {
            Console.WriteLine("SerialConnect");
            try
            {
                serial.PortName = port;
                serial.Open();
                await Task.Delay(2000);
                serial.WriteLine("connect");
                await Task.Delay(1000);
                if (connected) ini.Write("Port", port);
                Invoke(() => labelConnection.Text = connected ? "Connected" : "Connected - No response");

                return true;

            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
                Console.WriteLine("FormMain.SerialConnect error:" + ex.Message);
            }

            return false;
        }

        public async Task<bool> SerialDisconnect()
        {
            Console.WriteLine("SerialDisconnect");
            try
            {
                serial.WriteLine("standby");
                await Task.Delay(1000);
                serial.Close();
                status = "-1";
                Invoke(() => {
                    enrollToolStripMenuItem.Enabled = false;
                    startToolStripMenuItem.Enabled = false;
                    labelConnection.Text = "Disconnected";
                    labelStatus.Text = status;
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormMain.SerialDisconnect error:" + ex.Message);
                await Task.Delay(1000);
            }
            return false;
        }

        public void SendStatus()
        {
            Console.WriteLine("SendStatus");
            try 
            {
                serial.WriteLine("status");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "FormMain.SendStatus Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void GetEmployeeList()
        {
            employeeList = await Helper.GetEmployeeList();
            labelTotal.Text = employeeList.Length.ToString();
            Console.WriteLine("GetEmployeeList Count: {0}", employeeList.Count());
        }

        public async void GetAttendanceList()
        {
            attendaceList = await Helper.GetAttendaceList();
            formStart?.DisplayAttendance();
            Console.WriteLine("GetAttendanceList Count: {0}", attendaceList.Count());
        }

    }

}
