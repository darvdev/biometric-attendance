using biometric_attendance;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
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
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
  
        private void FormEmployeeList_Load(object sender, EventArgs e)
        {
            
            foreach (ModelEmployee employee in formMain.employees)
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
        }
    }
}
