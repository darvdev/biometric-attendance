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
            connectionString = formMain.connectionString;
            serialPort = formMain.serialPort;
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
        private string connectionString;
        private SerialPort serialPort;

        private void FormEnroll_Load(object sender, EventArgs e)
        {

            for (int i = 1; i < 140; i++)
            {
                comboBoxBiometricId.Items.Add(i);
            }

            using IDbConnection con = new SQLiteConnection(connectionString);
            var output = con.Query<ModelEmployee>("select * from employees", new DynamicParameters());
            con.Close();
            con.Dispose();
            foreach (ModelEmployee employee in output.ToList())
            {
                comboBoxEmployeeList.Items.Add($"{employee.employee_id} - {employee.name}");
            }

        }

        private void comboBoxEmployeeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(((ComboBox)sender).SelectedIndex);

            comboBoxBiometricId.Enabled = comboBoxEmployeeList.SelectedIndex > -1;
        }

        private void comboBoxBiometricId_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonEnroll.Enabled = comboBoxEmployeeList.SelectedIndex > -1 && comboBoxBiometricId.SelectedIndex > -1; 
        }

        private void Enroll(object sender, EventArgs e)
        {
            
            try
            {
                serialPort.WriteLine("enroll");
                serialPort.WriteLine($"id={comboBoxBiometricId.Text}");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
