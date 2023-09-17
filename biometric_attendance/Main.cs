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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private readonly SerialPort serialPort  = new SerialPort();

        private void Main_Load(object sender, EventArgs e)
        {
            comboBoxDeviceType.SelectedIndex = 0;

            var ports = SerialPort.GetPortNames();

            comboBoxDevicePort.Items.AddRange(SerialPort.GetPortNames());

            if (comboBoxDevicePort.Items.Count > 0)
            {
                comboBoxDevicePort.SelectedIndex = 0;
            }

            serialPort.DtrEnable = true;
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void ComboBoxDevicePort_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonConnect.Enabled = comboBoxDevicePort.SelectedIndex > -1;
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Connect");
            DeviceConnect();
        }


        private async void DeviceConnect()
        {
            Console.WriteLine("DeviceConnect");
            try 
            {
                buttonConnect.Enabled = false;
                buttonConnect.Text = "Connecting";
                serialPort.PortName = comboBoxDevicePort.Items[comboBoxDevicePort.SelectedIndex].ToString();
                serialPort.Open();
                Console.WriteLine("Opened");
                await Task.Delay(5000);
                Console.WriteLine("Send status");
                serialPort.WriteLine("status");
                await Task.Delay(1000);
                Console.WriteLine("Done");
                buttonConnect.Text = "Disconnect";
                buttonConnect.Enabled = true;
            } catch (Exception e)
            {
                Console.WriteLine("Error: $e" + e);
                buttonConnect.Text = "Connect";
                buttonConnect.Enabled = true;
            }


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

    }
}
