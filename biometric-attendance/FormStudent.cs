using System;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormStudent : Form
    {
        public FormStudent()
        {
            InitializeComponent();
            textBoxBiometricId.KeyPress += Helper.NumericKeyPress;
        }

        private void Invoke(Action method) => this.Invoke((MethodInvoker)delegate { method(); });

        private async void Save(object sender, EventArgs e)
        {
            try
            {
                ModelStudent student = Helper.NewStudent(
                    textBoxStudentId.Text,
                    textBoxFirstName.Text,
                    textBoxLastName.Text,
                    textBoxMiddleName.Text,
                    pictureBox.Image,
                    textBoxBiometricId.Text,
                    textBoxUsername.Text,
                    textBoxPassword.Text
                    );

                var result = await Helper.AddStudent(student);
                if (result)
                {
                    MessageBox.Show($"Student {student.name} created successfully!", "Student Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
