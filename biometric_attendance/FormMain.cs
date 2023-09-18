using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private const string connectionString = "Data Source=.\\database.db;Version=3";
        private const string providerName = "System.Data.SqlClient";

        private FormSettings formSettings;
        private FormEnroll formEnroll;
        private FormEmployee formEmployee;
        private FormAttendance formAttendance;

        
        public readonly SerialPort serialPort = new SerialPort();
        public string portName = "";

        public string test = "";
        
        private void Main_Load(object sender, EventArgs e)
        {

            Task.Run(() => {
                serialPort.DtrEnable = true;
                serialPort.DataReceived += SerialPort_DataReceived;
            });
            

        }

        private void OpenFormSettings(object sender, EventArgs e)
        {
            formSettings = new FormSettings();
            formSettings.ShowDialog();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string serialData = serialPort.ReadTo(serialPort.NewLine);
                Console.WriteLine("Data: " + serialData);
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

    }
}
