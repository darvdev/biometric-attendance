using biometric_attendance;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormEmployeeList : Form
    {
        public FormEmployeeList()
        {
            InitializeComponent();
        }

        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    const int WM_NCLBUTTONDOWN = 0x00A1;

        //    if (m.Msg == WM_NCLBUTTONDOWN)
        //    {
        //        employeeDataGridView.ClearSelection();
        //        ee = null;
        //        DisplayEmployee(null);
        //    }
        //}

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
        private bool ready = false;
        private int index = -1;
        private ModelEmployee ee = null;

        private void Invoke(Action method) => this.Invoke((MethodInvoker)delegate { method(); });

        private void FormEmployeeList_Load(object sender, EventArgs e)
        {
            RefreshEmployeeList(true);
        }
        private void EmployeeDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (ready) 
            {
                if (employeeDataGridView.SelectedRows.Count > 0)
                {
                    index = employeeDataGridView.SelectedRows[0].Index;
                    ee = formMain.employeeList[index];
                }
                else
                {
                    index = -1;
                    employeeDataGridView.ClearSelection();
                    ee = null;
                }
                DisableComponents();
                DisplayEmployee(ee);
            }
        }

        private void DisplayEmployee(ModelEmployee ee) 
        {
            if (ee == null)
            {
                index = -1;
                pictureBox.Image = null;
                textBoxEmployeeId.Text = "";
                textBoxFirstName.Text = "";
                textBoxMiddleName.Text = "";
                textBoxLastName.Text = "";
                textBoxBiometricId.Text = "";

                buttonDelete.Enabled = false;
                buttonEnroll.Enabled = false;
                buttonEdit.Enabled = false;
            }
            else
            {
                pictureBox.Image = ee.image;
                textBoxEmployeeId.Text = ee.employee_id;
                textBoxFirstName.Text = ee.first_name;
                textBoxMiddleName.Text = ee.middle_name;
                textBoxLastName.Text = ee.last_name;
                textBoxBiometricId.Text = ee.biometric_id?.ToString();

                buttonDelete.Enabled = true;
                buttonEnroll.Enabled = true;
                buttonEdit.Enabled = true;
            }
        }

        private void EmployeeDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                var ht = employeeDataGridView.HitTest(e.X, e.Y);
                if (ht.Type == DataGridViewHitTestType.None)
                {
                    employeeDataGridView.ClearSelection();
                    ee = null;
                    DisableComponents();
                }
                DisplayEmployee(ee);
            }
        }

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            if (buttonNew.Text == "Cancel") 
            {
                DisableComponents();
                return;
            }  

            FormEmployee formEmployee = new FormEmployee();
            var result = formEmployee.ShowDialog();
            if (result == DialogResult.OK)
            {
                formMain?.GetEmployeeList();
                RefreshEmployeeList();
            }
        }

        private async void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (buttonEdit.Text == "Save")
            {
                try
                {
                    var ee = Helper.NewEmployee(
                        textBoxEmployeeId.Text,
                        textBoxFirstName.Text,
                        textBoxLastName.Text,
                        textBoxMiddleName.Text,
                        pictureBox.Image,
                        textBoxBiometricId.Text
                        );

                    ee.id = this.ee.id;

                    var result = await Helper.UpdateEmployee(ee);
                    if (result)
                    {
                        formMain?.GetEmployeeList();
                        RefreshEmployeeList();
                        DisableComponents();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                EnableComponents();
            }
            
        }

        private void DisableComponents() 
        {
            buttonDelete.Enabled = true;
            buttonEnroll.Enabled = true;
            buttonEdit.Text = "Edit";
            buttonNew.Text = "New";
            buttonRemove.Enabled = false;
            buttonRemove.Visible = false;
            buttonBrowse.Visible = false;
            
            textBoxEmployeeId.ReadOnly = true;
            textBoxFirstName.ReadOnly = true;
            textBoxMiddleName.ReadOnly = true;
            textBoxLastName.ReadOnly = true;
        }
        private void EnableComponents()
        {
            buttonEdit.Text = "Save";
            buttonNew.Text = "Cancel";
            buttonDelete.Enabled = false;
            buttonEnroll.Enabled = false;

            buttonRemove.Visible = true;
            buttonBrowse.Visible = true;
            buttonBrowse.Text = ee?.image == null ? "Browse..." : "Change...";

            textBoxEmployeeId.ReadOnly = false;
            textBoxFirstName.ReadOnly = false;
            textBoxMiddleName.ReadOnly = false;
            textBoxLastName.ReadOnly = false;
        }

        private async void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (ee != null) 
            {
                ready = false;
                var result = await Helper.DeleteEmployee(ee);
                if (result)
                {
                    ee = null;
                    DisplayEmployee(null);
                    formMain.GetEmployeeList();
                    RefreshEmployeeList(true);
                }
            }
        }

        private void RefreshEmployeeList(bool clearSelection = false)
        {
            ready = false;
            employeeDataGridView.Rows.Clear();
            foreach (ModelEmployee employee in formMain.employeeList)
            {
                employeeDataGridView.Rows.Add(new object[] {
                        employee.employee_id,
                        employee.biometric_id,
                        employee.first_name,
                        employee.middle_name,
                        employee.last_name,
                     });

            }
            if (clearSelection)
            {
                index = -1;
                employeeDataGridView.ClearSelection();
            }
            else
            {
                employeeDataGridView.Rows[index].Selected = true;
            }
            ready = true;
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            var image = Helper.BrowseImage();
            pictureBox.Image = image;
            if (image != null)
            {
                buttonRemove.Enabled = true;
                buttonBrowse.Text = "Change...";
            }

        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            buttonBrowse.Text = "Browse...";
            buttonRemove.Enabled = false;
            pictureBox.Image = null;
        }
    }
}
