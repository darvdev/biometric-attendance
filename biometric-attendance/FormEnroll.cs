﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormEnroll : Form
    {
        public FormEnroll(int index = -1)
        {
            InitializeComponent();
            LoadCombobox(index);
            formMain.EnrollStatusEvent += EnrollStatusEvent;
            formMain.EnrollEmployeeEvent += EnrollEmployeeEvent;
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
        private int employeeIndex = -1;
        private int biometricIndex = -1;
        private string status;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            formMain.serial.WriteLine("standby");
            formMain.EnrollStatusEvent -= EnrollStatusEvent;
            formMain.EnrollEmployeeEvent -= EnrollEmployeeEvent;
            base.OnFormClosing(e);
        }

        private void Invoke(Action action) => this.Invoke((MethodInvoker)delegate { action(); });

        private void LoadCombobox(int index = -1) {

            comboBoxEmployeeList.Items.Clear();
            comboBoxBiometricId.Items.Clear();

            for (int i = 1; i < 140; i++)
            {
                var ee = Array.Find(formMain.employeeList, (e) => e.biometric_id == i);
                var item = $"{i} - " + (ee == null ? "Available" : ee.name);
                comboBoxBiometricId.Items.Add(item);
            }

            foreach (ModelEmployee ee in formMain.employeeList)
            {
                int? id = ee.biometric_id;
                comboBoxEmployeeList.Items.Add($"{ee.employee_id} - {ee.name} - " + (id == null ? "NOT REG" : $"REG [{id}]"));
            }
        }

        private void ComboBoxEmployeeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            employeeIndex = comboBoxEmployeeList.SelectedIndex;
            comboBoxBiometricId.Enabled = employeeIndex >= 0;         

            if (employeeIndex >= 0)
            {
                labelStatus.Text = biometricIndex >=0 ? "Click enroll button to start" : "Select biometric id from dropdown";
                pictureBoxEmployee.Image = formMain.employeeList[employeeIndex].image;
            }
            else
            {
                pictureBoxEmployee.Image = null;
                labelStatus.Text = "Select employee from dropdown";
            }
        }

        private void ComboBoxBiometricId_SelectedIndexChanged(object sender, EventArgs e)
        {
            biometricIndex = comboBoxBiometricId.SelectedIndex;
            buttonEnroll.Enabled = employeeIndex >= 0 && biometricIndex >= 0;
            labelStatus.Text = biometricIndex >= 0 ? "Click enroll button to start" : "Select biometric id from dropdown";
        }

        private async void ButtonEnroll_Click(object sender, EventArgs e)
        {
            buttonEnroll.Enabled = false;

            if (buttonEnroll.Text == "Cancel")
            {
                formMain.serial.WriteLine("standby");
                buttonEnroll.Text = "Canceling...";
                await Task.Delay(1000);
                if (status == "000")
                {
                    status = "";
                    this.MinimumSize = new System.Drawing.Size(440, 200);
                    this.MaximumSize = new System.Drawing.Size(440, 200);
                    comboBoxEmployeeList.Enabled = true;
                    comboBoxBiometricId.Enabled = true;
                    buttonEnroll.Text = "Enroll";
                    buttonEnroll.Enabled = true;
                    labelStatus.Text = "Enroll canceled";
                }
                
                return;
            }

            comboBoxEmployeeList.Enabled = false;
            comboBoxBiometricId.Enabled = false;
            buttonEnroll.Text = "Enrolling...";
            
            try
            {
                await Task.Delay(500);
                formMain.serial.WriteLine("enroll");
                await Task.Delay(100);
                formMain.serial.WriteLine($"id={biometricIndex + 1}");
                await Task.Delay(500);
                     
                buttonEnroll.Enabled = true;
                if (status == "100")
                {
                    this.MinimumSize = new System.Drawing.Size(440, 430);
                    this.MaximumSize = new System.Drawing.Size(440, 430);
                    buttonEnroll.Text = "Cancel";
                }
                else
                {
                    comboBoxEmployeeList.Enabled = true;
                    comboBoxBiometricId.Enabled = true;
                    buttonEnroll.Text = "Enroll";
                }

            }
            catch (Exception ex)
            {
                comboBoxEmployeeList.Enabled = true;
                comboBoxBiometricId.Enabled = true;
                buttonEnroll.Enabled = true;
                buttonEnroll.Text = "Enroll";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Focus();

        }

        private void EnrollEmployeeEvent(object sender, CustomEventArgs e) //Called from FormMain SerialPort_DataReceived
        {
            Invoke(async () => {
                try
                {
                    pictureBoxEnroll.Image = Properties.Resources.ok;
                    int id = int.Parse(e.value);
                    var ee = formMain.employeeList[employeeIndex];
                    buttonEnroll.Enabled = false;

                    labelStatus.Text = $"Fingerprint OK. Updating {ee.name} details...";

                    //Updating employee databse
                    bool result = await Helper.EnrollEmployee(ee.id, id);

                    if (result)
                    {
                        //update successs
                        var dialog = MessageBox.Show($"{ee.name} fingerprint registered.\nEnroll another employee?", "Enrolled", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dialog == DialogResult.Yes)
                        {
                            formMain.GetEmployeeList();
                            LoadCombobox();
                            pictureBoxEmployee.Image = null;
                            comboBoxEmployeeList.SelectedIndex = -1;
                            comboBoxBiometricId.SelectedIndex = -1;
                            pictureBoxEnroll.Image = Properties.Resources.fingerprint;

                            employeeIndex = -1;
                            biometricIndex = -1;
                            labelStatus.Text = "Select employee from dropdown";
                        }
                        else
                        {
                            this.Close();
                            return;
                        }

                    }
                    else
                    {
                        //Update error
                        labelStatus.Text = $"Error enrolling {ee.name}";
                    }

                    buttonEnroll.Text = "Enroll";
                    buttonEnroll.Enabled = true;
                    comboBoxEmployeeList.Enabled = true;
                    comboBoxBiometricId.Enabled = true;
                    this.MinimumSize = new System.Drawing.Size(440, 200);
                    this.MaximumSize = new System.Drawing.Size(440, 200);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void EnrollStatusEvent(object sender, CustomEventArgs e)
        {
            //000 = Cancel enrollment
            //100 = Place finger in the sensor
            //101 = Enroll step 1 success (Remove finger in the Sensor)
            //102 = Place same finger in the Sensor

            status = e.value;
            Invoke(() => {
                switch (e.value)
                {
                    case "000":
                        break;
                    case "100":
                        labelStatus.Text = "Place your finger in the sensor";
                        pictureBoxEnroll.Image = Properties.Resources.fingerprint;
                        break;
                    case "101":
                        labelStatus.Text = "Fingerprint OK. Remove finger in the Sensor";
                        pictureBoxEnroll.Image = Properties.Resources.ok;
                        break;
                    case "102":
                        labelStatus.Text = "Place the same finger in the Sensor";
                        pictureBoxEnroll.Image = Properties.Resources.fingerprint;
                        break;
                    default:
                        labelStatus.Text = "Fingerprint error: " + status;
                        pictureBoxEnroll.Image = Properties.Resources.error;
                        break;
                }
            });
        }
    
    }

}
