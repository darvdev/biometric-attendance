using System;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];

        private bool success = false;
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (success)
            {
                base.OnFormClosing(e);
            }
            else
            {
                //Application.Exit();
                Environment.Exit(0);
                e.Cancel = true;
            }
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Admin {0}", formMain.admins);

            var username = textBoxUsername.Text;
            var password = textBoxPassword.Text;

            var result = Array.Find<ModelStudent>(formMain.students, (s) => s.username == username && s.password == password);

            if (result == null)
            {
                MessageBox.Show("Invalid username or password.\nPlease try again.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                success = true;
                formMain.admin = result;
                this.Close();
            }

        }
    }
}
