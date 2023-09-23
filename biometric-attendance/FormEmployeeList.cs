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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormEmployeeList : Form
    {
        public FormEmployeeList()
        {
            this.AllowTransparency = true;
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
        private ModelEmployee ee = null;
  
        private void FormEmployeeList_Load(object sender, EventArgs e)
        {
            
            foreach (ModelEmployee employee in formMain.employeeList)
            {
                employeeDataGridView.Rows.Add(new object[] {
                        employee.id,
                        employee.employee_id,
                        employee.biometric_id,
                        employee.first_name,
                        employee.middle_name,
                        employee.last_name,
                     });

            }
            employeeDataGridView.ClearSelection();
            ready = true;
        }
        private void EmployeeDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (ready) 
            {
                if (employeeDataGridView.SelectedRows.Count > 0)
                {
                    var index = employeeDataGridView.SelectedRows[0].Index;
                    ee = formMain.employeeList[index];
                }
                else
                {
                    employeeDataGridView.ClearSelection();
                    ee = null;
                }

                DisplayEmployee(ee);

            }
        }

        private void DisplayEmployee(ModelEmployee ee) 
        {
            if (ee == null)
            {
                pictureBox.Image = null;
                textBoxEemployeeId.Text = "";
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
                pictureBox.Image = Helper.GetImageFromBase64String(ee.img_base64);
                textBoxEemployeeId.Text = ee.employee_id;
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
            var ht = employeeDataGridView.HitTest(e.X, e.Y);
            if (ht.Type == DataGridViewHitTestType.None)
            {
                employeeDataGridView.ClearSelection();
                ee = null;
            }
            DisplayEmployee(ee);
        }

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            FormEmployee formEmployee = new FormEmployee();
            formEmployee.ShowDialog();
        }
    }
}
