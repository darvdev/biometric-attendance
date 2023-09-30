using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormEnroll : Form
    {
        public FormEnroll(int index = -1)
        {
            InitializeComponent();
            LoadCombobox(index);
            formMain.EnrollStatusEvent += EnrollStatusEvent;
            formMain.EnrollStudentEvent += EnrollStudentEvent;
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
        private int studentIndex = -1;
        private int biometricIndex = -1;
        private string status;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (buttonEnroll.Text == "Cancel")
            {
                var result = MessageBox.Show("Do you want to cancel the fingerprint enrollment of a student?", "Cancel Enrollennt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            formMain.serial.WriteLine("standby");
            formMain.EnrollStatusEvent -= EnrollStatusEvent;
            formMain.EnrollStudentEvent -= EnrollStudentEvent;
            base.OnFormClosing(e);
        }

        private void Invoke(Action action) => this.Invoke((MethodInvoker)delegate { action(); });

        private void LoadCombobox(int index = -1) {

            comboBoxStudentList.Items.Clear();
            comboBoxBiometricId.Items.Clear();

            for (int i = 1; i < 140; i++)
            {
                var student = Array.Find(formMain.students, (e) => e.biometric_id == i);
                var item = $"{i} - " + (student == null ? "Available" : student.name);
                comboBoxBiometricId.Items.Add(item);
            }

            foreach (ModelStudent student in formMain.students)
            {
                int? id = student.biometric_id;
                comboBoxStudentList.Items.Add($"{student.student_id} - {student.name} - " + (id == null ? "NOT REG" : $"REG [{id}]"));
            }
        }

        private void ComboBoxStudentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            studentIndex = comboBoxStudentList.SelectedIndex;
            comboBoxBiometricId.Enabled = studentIndex >= 0;         

            if (studentIndex >= 0)
            {
                labelStatus.Text = biometricIndex >=0 ? "Click enroll button to start" : "Select biometric id from dropdown";
                pictureBoxStudent.Image = formMain.students[studentIndex].image;
            }
            else
            {
                pictureBoxStudent.Image = null;
                labelStatus.Text = "Select student from dropdown";
            }
        }

        private void ComboBoxBiometricId_SelectedIndexChanged(object sender, EventArgs e)
        {
            biometricIndex = comboBoxBiometricId.SelectedIndex;
            buttonEnroll.Enabled = studentIndex >= 0 && biometricIndex >= 0;
            labelStatus.Text = biometricIndex >= 0 ? "Click enroll button to start" : "Select biometric id from dropdown";
        }

        private async void ButtonEnroll_Click(object sender, EventArgs e)
        {
            buttonEnroll.Enabled = false;

            if (buttonEnroll.Text == "Cancel")
            {
                formMain.serial.WriteLine("standby");
                buttonEnroll.Text = "Canceling...";
                await Task.Delay(1000);
                if (status == "000")
                {
                    status = "";
                    this.MinimumSize = new Size(440, 200);
                    this.MaximumSize = new Size(440, 200);
                    comboBoxStudentList.Enabled = true;
                    comboBoxBiometricId.Enabled = true;
                    buttonEnroll.Text = "Enroll";
                    buttonEnroll.Enabled = true;
                    labelStatus.Text = "Enroll canceled";
                }
                
                return;
            }

            comboBoxStudentList.Enabled = false;
            comboBoxBiometricId.Enabled = false;
            buttonEnroll.Text = "Enrolling...";
            
            try
            {
                await Task.Delay(500);
                formMain.serial.WriteLine("enroll");
                await Task.Delay(100);
                formMain.serial.WriteLine($"id={biometricIndex + 1}");
                await Task.Delay(500);
                     
                buttonEnroll.Enabled = true;
                if (status == "100")
                {
                    this.MinimumSize = new Size(440, 430);
                    this.MaximumSize = new Size(440, 430);
                    buttonEnroll.Text = "Cancel";
                }
                else
                {
                    comboBoxStudentList.Enabled = true;
                    comboBoxBiometricId.Enabled = true;
                    buttonEnroll.Text = "Enroll";
                }

            }
            catch (Exception ex)
            {
                comboBoxStudentList.Enabled = true;
                comboBoxBiometricId.Enabled = true;
                buttonEnroll.Enabled = true;
                buttonEnroll.Text = "Enroll";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Focus();

        }

        private void EnrollStudentEvent(object sender, CustomEventArgs e) //Called from FormMain SerialPort_DataReceived
        {
            Invoke(async () => {
                try
                {
                    Helper.PlayBeep();
                    pictureBoxEnroll.Image = Properties.Resources.ok;
                    int id = int.Parse(e.value);
                    var student = formMain.students[studentIndex];
                    buttonEnroll.Enabled = false;

                    labelStatus.Text = $"Fingerprint OK. Updating {student.name} details...";

                    //Updating employee databse
                    bool result = await Helper.EnrollStudent(student.id, id);

                    buttonEnroll.Text = "Enroll";
                    buttonEnroll.Enabled = true;
                    comboBoxStudentList.Enabled = true;
                    comboBoxBiometricId.Enabled = true;
                    this.MinimumSize = new Size(440, 200);
                    this.MaximumSize = new Size(440, 200);


                    if (result)
                    {
                        //update successs
                        var dialog = MessageBox.Show($"{student.name} fingerprint registered.\nEnroll another student?", "Enrolled", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dialog == DialogResult.Yes)
                        {
                            formMain.GetStudentList();
                            LoadCombobox();
                            pictureBoxStudent.Image = null;
                            comboBoxStudentList.SelectedIndex = -1;
                            comboBoxBiometricId.SelectedIndex = -1;
                            pictureBoxEnroll.Image = Properties.Resources.fingerprint;

                            studentIndex = -1;
                            biometricIndex = -1;
                            labelStatus.Text = "Select student from dropdown";
                        }
                        else
                        {
                            this.Close();
                            return;
                        }

                    }
                    else
                    {
                        //Update error
                        labelStatus.Text = $"Error enrolling {student.name}";
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void EnrollStatusEvent(object sender, CustomEventArgs e)
        {
            //000 = Cancel enrollment
            //100 = Place finger in the sensor
            //101 = Enroll step 1 success (Remove finger in the Sensor)
            //102 = Place same finger in the Sensor
            //10  = Fingerprint mismatch
            status = e.value;
            Invoke(() => {
                switch (e.value)
                {
                    case "000":
                        
                        break;
                    case "100":
                        labelStatus.Text = "Place your finger in the sensor";
                        pictureBoxEnroll.Image = Properties.Resources.fingerprint;
                        break;
                    case "101":
                        Helper.PlayBeep();
                        labelStatus.Text = "Fingerprint OK. Remove finger in the Sensor";
                        pictureBoxEnroll.Image = Properties.Resources.ok;
                        break;
                    case "102":
                        labelStatus.Text = "Place the same finger in the Sensor";
                        pictureBoxEnroll.Image = Properties.Resources.fingerprint;
                        break;
                    case "10":
                        Helper.PlayError();
                        labelStatus.Text = "Fingerprint mismatch. Please try again.";
                        pictureBoxEnroll.Image = Properties.Resources.error;
                        Task.Run(async () => {
                            await Task.Delay(3000);
                            EnrollStatusEvent(this, new CustomEventArgs("102"));
                        });
                        break;
                    default:
                        Helper.PlayError();
                        labelStatus.Text = "Fingerprint error: " + status;
                        pictureBoxEnroll.Image = Properties.Resources.error;
                        break;
                }
            });
        }
    
    }

}
