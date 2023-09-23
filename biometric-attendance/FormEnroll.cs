using BiometricAttendance;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace biometric_attendance
{
    public partial class FormEnroll : Form
    {
        public FormEnroll()
        {
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (formMain.enrolling)
            {
                formMain.employeeIndex = -1;
                Console.WriteLine("send: standby");
                formMain.serial.WriteLine("standby");
            }
        }

        private void FormEnroll_Load(object sender, EventArgs e)
        {

            for (int i = 1; i < 140; i++)
            {

                var ee = Array.Find(formMain.employeeList, (e) => e.biometric_id == i);
        
                dynamic item = i;
                
                if (ee != null)
                {
                    item = $"{i} - {ee.name}";
                }


                comboBoxBiometricId.Items.Add(item);
            }

            foreach (ModelEmployee ee in formMain.employeeList)
            {
                int? id = ee.biometric_id;
                comboBoxEmployeeList.Items.Add($"{ee.employee_id} - {ee.name} - " + (id == null ? "NOT REG" :  $"REG [{id}]"));
            }

        }

        private void comboBoxEmployeeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            formMain.employeeIndex = comboBoxEmployeeList.SelectedIndex;
            comboBoxBiometricId.Enabled = comboBoxEmployeeList.SelectedIndex > -1;
        }

        private void comboBoxBiometricId_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonEnroll.Enabled = comboBoxEmployeeList.SelectedIndex > -1 && comboBoxBiometricId.SelectedIndex > -1; 
        }

        private void Enroll(object sender, EventArgs e)
        {
            comboBoxEmployeeList.Enabled = false;
            comboBoxBiometricId.Enabled = false;
            buttonEnroll.Enabled = false;
            buttonEnroll.Text = "Enrolling...";
            
            string id = comboBoxBiometricId.Text;

            Task.Run(async () => {
                try
                {
                    await Task.Delay(500);
                    formMain.serial.WriteLine("enroll");
                    await Task.Delay(100);
                    formMain.serial.WriteLine($"id={id}");
                    await Task.Delay(500);
                    Console.WriteLine("Enroll: {0}", formMain.enrolling);


                    this.Invoke((MethodInvoker)delegate {
                        buttonEnroll.Enabled = true;
                        if (formMain.enrolling)
                        {
                            buttonEnroll.Text = "Cancel";
                        }
                        else
                        {
                            comboBoxEmployeeList.Enabled = true;
                            comboBoxBiometricId.Enabled = true;
                            buttonEnroll.Text = "Enroll";
                        }

                    });
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    this.Invoke((MethodInvoker) delegate {
                        comboBoxEmployeeList.Enabled = true;
                        comboBoxBiometricId.Enabled = true;
                        buttonEnroll.Enabled = true;
                        buttonEnroll.Text = "Enroll";
                    });
                }
            });

            this.Focus();

        }


        public void EnrollStatus(string status)
        {
            labelEnrollStatus.Text = status;
        }
    }
}
