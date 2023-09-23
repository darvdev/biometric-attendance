using BiometricAttendance;
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

namespace biometric_attendance
{
    public partial class FormEmployee : Form
    {
        public FormEmployee()
        {
            InitializeComponent();
            textBoxBiometricId.KeyPress += Helper.NumericKeyPress;
        }

        private string connectionString = ((FormMain)Application.OpenForms["FormMain"]).connectionString;

        private void Save(object sender, EventArgs e)
        {
            Console.WriteLine("Saving...");
            try
            {
                ModelEmployee employee = new ModelEmployee
                {
                    employee_id = textBoxEemployeeId.Text,
                    first_name = textBoxFirstName.Text,
                    middle_name = textBoxMiddleName.Text,
                    last_name = textBoxLastName.Text,
                    biometric_id = TryParseNullable(textBoxBiometricId.Text),
                    username = textBoxUsername.Text == String.Empty ? null : textBoxUsername.Text,
                    password = textBoxPassword.Text == String.Empty ? null : textBoxPassword.Text,
                };

                using (IDbConnection con = new SQLiteConnection(connectionString))
                {

                    con.Execute("insert into employees (employee_id, first_name, middle_name, last_name, biometric_id, username, password) values (@employee_id, @first_name, @middle_name, @last_name, @biometric_id, @username, @password)", employee);
                    con.Close();
                    con.Dispose();
                }
                
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save error: {0}", ex.Message);
            }

        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Close();
        }

        public int? TryParseNullable(string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? (int?)outValue : null;
        }
    }

}
