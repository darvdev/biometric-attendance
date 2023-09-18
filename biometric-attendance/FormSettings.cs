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
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private static FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
        private SerialPort serialPort = formMain.serialPort;

        private void Settings_Load(object sender, EventArgs e)
        {

            comboBoxDeviceType.SelectedIndex = 0;

            var ports = SerialPort.GetPortNames();

            comboBoxDevicePort.Items.AddRange(ports);

            if (comboBoxDevicePort.Items.Count > 0)
            {
                comboBoxDevicePort.SelectedIndex = 0;
            }

            if (!serialPort.IsOpen)
            {
                comboBoxDevicePort.Enabled = true;
                buttonConnect.Text = "Connect";
                buttonConnect.Enabled = true;
            }
           
            
        }

        private void ComboBoxDevicePort_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonConnect.Enabled = serialPort.IsOpen  && buttonConnect.Text == "Disconnect" ? true : comboBoxDevicePort.SelectedIndex > -1;


            
            Console.WriteLine($"buttonConnect.Text: {buttonConnect.Text}");
            Console.WriteLine($"serialPort.IsOpen: {serialPort.IsOpen}");

            buttonConnect.Enabled = comboBoxDevicePort.SelectedIndex < 0 ? false : serialPort.IsOpen && buttonConnect.Text == "Disconnect";

        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                Console.WriteLine("Disconnect");
            }
            else
            {
                Console.WriteLine("Connect");

                try
                {
                    formMain.portName = comboBoxDevicePort.Items[comboBoxDevicePort.SelectedIndex].ToString();
                }
                catch (Exception err)
                {
                    Console.WriteLine($"Error: {err}");
                    return;
                }

                buttonConnect.Enabled = false;
                buttonConnect.Text = "Connecting";
                comboBoxDevicePort.Enabled = false;


                Task.Run(async () =>
                {
                    var result = await formMain.SerialConnect();

                    this.Invoke((MethodInvoker)delegate {
                        buttonConnect.Text = result ? "Disconnect" : "Connect";
                        buttonConnect.Enabled = true;
                        comboBoxDevicePort.Enabled = !result;
                    });

                });
            }


        }
        
    }
}
