using System;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormEmployee : Form
    {
        public FormEmployee()
        {
            InitializeComponent();
            textBoxBiometricId.KeyPress += Helper.NumericKeyPress;
        }

        private void Invoke(Action method) => this.Invoke((MethodInvoker)delegate { method(); });

        private async void Save(object sender, EventArgs e)
        {
            try
            {
                ModelEmployee ee = Helper.NewEmployee(
                    textBoxEmployeeId.Text,
                    textBoxFirstName.Text,
                    textBoxLastName.Text,
                    textBoxMiddleName.Text,
                    pictureBox.Image,
                    textBoxBiometricId.Text,
                    textBoxUsername.Text,
                    textBoxPassword.Text
                    );

                var result = await Helper.AddEmployee(ee);
                if (result)
                {
                    MessageBox.Show($"Employee {ee.name} created successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var image = Helper.BrowseImage();
            pictureBox.Image = image;
            if (image != null)
            {
                buttonRemove.Enabled = true;
                buttonBrowse.Text = "Change...";
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            buttonBrowse.Text = "Browse...";
            buttonRemove.Enabled = false;
            pictureBox.Image = null;
        }

        
    }

}
