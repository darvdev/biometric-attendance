using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BiometricAttendance
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

            var startup = formMain.ini.Read("Startup");
            startupCheckBox.Checked = startup == "1";

            var connect = formMain.ini.Read("Connect");
            connectCheckBox.Checked = connect == "1";

            var start = formMain.ini.Read("Start");
            startCheckBox.Checked = start == "1";

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

        private async void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (formMain.serial.IsOpen)
            {
                buttonConnect.Enabled = false;
                buttonConnect.Text = "Disconnecting...";

                var result = await formMain.SerialDisconnect();
                buttonConnect.Text = result ? "Connect" : "Disconnect";
                buttonConnect.Enabled = true;
                comboBoxDevicePort.Enabled = result;
            }
            else
            {
                try
                {
                    formMain.port = comboBoxDevicePort.Items[comboBoxDevicePort.SelectedIndex].ToString();

                    buttonConnect.Enabled = false;
                    buttonConnect.Text = "Connecting";
                    comboBoxDevicePort.Enabled = false;

                    var result = await formMain.SerialConnect();
                    if (result) formMain.SendStatus();

                    buttonConnect.Text = result ? "Disconnect" : "Connect";
                    buttonConnect.Enabled = true;
                    comboBoxDevicePort.Enabled = !result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ConnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var enable = connectCheckBox.Checked;
            
            formMain.ini.Write("Connect", enable ? "1" : "0");

            startCheckBox.Enabled = enable;

            if (!enable)
            {
                startCheckBox.Checked = false;
                formMain.ini.Write("Start", "0");
            }
        }

        private void StartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            formMain.ini.Write("Startup", startupCheckBox.Checked ? "1" : "0");
        }

        private void StartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            formMain.ini.Write("Start", startCheckBox.Checked ? "1" : "0");
        }
    }
}
