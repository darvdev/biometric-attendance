using System;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormStudentList : Form
    {
        public FormStudentList()
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
        private ModelStudent student = null;

        private void Invoke(Action method) => this.Invoke((MethodInvoker)delegate { method(); });

        private void FormLoad(object sender, EventArgs e)
        {
            RefreshStudentList(true);
        }

        private void EmployeeDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (ready) 
            {
                if (studentDataGridView.SelectedRows.Count > 0)
                {
                    index = studentDataGridView.SelectedRows[0].Index;
                    student = formMain.students[index];
                }
                else
                {
                    index = -1;
                    studentDataGridView.ClearSelection();
                    student = null;
                }
                DisableComponents();
                DisplayStudent(student);
            }
        }

        private void DisplayStudent(ModelStudent student) 
        {
            if (student == null)
            {
                index = -1;
                pictureBox.Image = null;
                textBoxStudentId.Text = "";
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
                pictureBox.Image = student.image;
                textBoxStudentId.Text = student.student_id;
                textBoxFirstName.Text = student.first_name;
                textBoxMiddleName.Text = student.middle_name;
                textBoxLastName.Text = student.last_name;
                textBoxBiometricId.Text = student.biometric_id?.ToString();

                buttonDelete.Enabled = true;
                buttonEnroll.Enabled = true;
                buttonEdit.Enabled = true;
            }
        }

        private void EmployeeDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                var ht = studentDataGridView.HitTest(e.X, e.Y);
                if (ht.Type == DataGridViewHitTestType.None)
                {
                    studentDataGridView.ClearSelection();
                    student = null;
                    DisableComponents();
                }
                DisplayStudent(student);
            }
        }

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            if (buttonNew.Text == "Cancel") 
            {
                DisableComponents();
                return;
            }  

            FormStudent formEmployee = new FormStudent();
            var result = formEmployee.ShowDialog();
            if (result == DialogResult.OK)
            {
                formMain?.GetStudentList();
                RefreshStudentList();
            }
        }

        private async void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (buttonEdit.Text == "Save")
            {
                try
                {
                    Console.WriteLine("punt dito");
                    var student = Helper.NewStudent(
                        textBoxStudentId.Text,
                        textBoxFirstName.Text,
                        textBoxLastName.Text,
                        textBoxMiddleName.Text,
                        pictureBox.Image,
                        textBoxBiometricId.Text
                        );

                    this.student.id = student.id;

                    var result = await Helper.UpdateStudent(student);
                    if (result)
                    {
                        formMain?.GetStudentList();
                        RefreshStudentList();
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
            
            textBoxStudentId.ReadOnly = true;
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
            buttonRemove.Enabled = student.image != null;
            buttonBrowse.Visible = true;
            buttonBrowse.Text = student?.image == null ? "Browse..." : "Change...";
            
            textBoxStudentId.ReadOnly = false;
            textBoxFirstName.ReadOnly = false;
            textBoxMiddleName.ReadOnly = false;
            textBoxLastName.ReadOnly = false;
        }

        private async void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (student != null) 
            {
                var dialog = MessageBox.Show($"WARNING: Do you really want to delete {student.name} from database?", "Delete Student", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialog == DialogResult.Yes)
                {
                    ready = false;
                    var result = await Helper.DeleteStudent(student);
                    if (result)
                    {
                        student = null;
                        DisplayStudent(null);
                        formMain.GetStudentList();
                        RefreshStudentList(true);
                    }
                }

            }
        }

        private void RefreshStudentList(bool clearSelection = false)
        {
            ready = false;
            studentDataGridView.Rows.Clear();
            foreach (ModelStudent student in formMain.students)
            {
                studentDataGridView.Rows.Add(new object[] {
                        student.student_id,
                        student.biometric_id,
                        student.first_name,
                        student.middle_name,
                        student.last_name,
                        student.username,
                        student.password,
                     });

            }
            if (clearSelection)
            {
                index = -1;
                studentDataGridView.ClearSelection();
            }
            else
            {
                if (index >= 0) {
                    studentDataGridView.Rows[index].Selected = true;
                }
                
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

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            buttonBrowse.Text = "Browse...";
            buttonRemove.Enabled = false;
            pictureBox.Image = null;
        }
    }
}
