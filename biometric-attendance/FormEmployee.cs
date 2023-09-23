using BiometricAttendance;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
                    img_base64 = Helper.ImageToBase64String(pictureBox.Image),
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
                    con.Execute("insert into employees (employee_id, first_name, middle_name, last_name, img_base64, biometric_id, username, password) values (@employee_id, @first_name, @middle_name, @last_name, @img_base64, @biometric_id, @username, @password)", employee);
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

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp; *.png;";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = new Bitmap(open.FileName);

                if (pictureBox.Image != null)
                {
                    removeButton.Enabled = true;
                    browseButton.Text = "Change...";
                }
            }

        }


        private void RemoveButton_Click(object sender, EventArgs e)
        {
            browseButton.Text = "Browse...";
            removeButton.Enabled = false;
            pictureBox.Image = null;
            
        }
    }

}
