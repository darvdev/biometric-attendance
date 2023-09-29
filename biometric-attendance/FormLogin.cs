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
            if (success || formMain.admin != null)
            {
                base.OnFormClosing(e);
            }
            else
            {
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
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            if (formMain.admin != null)
            {
                textBoxUsername.Text = formMain.admin.username;
                textBoxUsername.Enabled = false;
            }
        }

        private void TextBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }

        private void TextBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBoxPassword.Focus();
            }
        }
    }
}
