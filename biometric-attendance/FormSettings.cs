using BiometricAttendance;
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

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
       
        private void Settings_Load(object sender, EventArgs e)
        {

            comboBoxDeviceType.SelectedIndex = 0;

            comboBoxDevicePort.Items.AddRange(formMain.ports);

            if (comboBoxDevicePort.Items.Count > 0)
            {
                if (formMain.serial.IsOpen)
                {
                    int index = Array.IndexOf(comboBoxDevicePort.Items.Cast<string>().ToArray<string>(), formMain.serial.PortName);
                    comboBoxDevicePort.SelectedIndex = index;
                }
                else 
                {
                    comboBoxDevicePort.SelectedIndex = 0;
                }
            }

            if (!formMain.serial.IsOpen)
            {
                comboBoxDevicePort.Enabled = true;
                buttonConnect.Text = "Connect";
                buttonConnect.Enabled = comboBoxDevicePort.SelectedIndex > -1;
            }
           
            
        }

        private void ComboBoxDevicePort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDevicePort.SelectedIndex > -1) 
            {
                buttonConnect.Enabled = true;
                buttonConnect.Text = formMain.serial.IsOpen ? "Disconnect" : "Connect";
            }
            else
            {
                buttonConnect.Enabled = false;
            }
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (formMain.serial.IsOpen)
            {
                buttonConnect.Enabled = false;
                buttonConnect.Text = "Disconnecting...";
                
                Task.Run(async () =>
                {
                    var result = await formMain.SerialDisconnect();
                    this.Invoke((MethodInvoker)delegate {
                        buttonConnect.Text = result ? "Connect" : "Disconnect";
                        buttonConnect.Enabled = true;
                        comboBoxDevicePort.Enabled = result;
                    });

                });

            }
            else
            {
                try
                {
                    formMain.port = comboBoxDevicePort.Items[comboBoxDevicePort.SelectedIndex].ToString();
                }
                catch (Exception err)
                {
                    Console.WriteLine("FormSettings.ButtonConnect_Click error: {0}", err.Message);
                    return;
                }

                buttonConnect.Enabled = false;
                buttonConnect.Text = "Connecting";
                comboBoxDevicePort.Enabled = false;


                Task.Run(async () =>
                {
                    var result = await formMain.SerialConnect();
                    if (result) formMain.SendStatus();

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
