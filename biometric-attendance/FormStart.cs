using BiometricAttendance;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace biometric_attendance
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];
        private Timer timer = new Timer();
        private int counter = 0;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            formMain.serial.WriteLine("standby");
            formMain.Show();
        }

        private void FormAttendance_Load(object sender, EventArgs e)
        {
            formMain.serial.WriteLine("start");
            timer.Interval = 1000;
            timer.Tick += TimerTick;
            DisplayAttendanceToday();
        }

        public void UpdateDateTime(string dt) 
        {
            labelDateTime.Text = dt;
        }

        public void UpdateBiometric(string status, ModelAttendance attendance)
        {

            //status description
            /*
             * 100 = ok
             * 9 = Finger Print not found
             * other = Unknown
             */
            try
            {
                if (status == "100")
                {
                    if (attendance == null)
                    {
                        fingerPictureBox.Image = BiometricAttendance.Properties.Resources.ee_error;
                        labelFinger.Text = "Fingerprint OK. Employee not found";
                        labelFinger.BackColor = Color.Red;
                    }
                    else
                    {
                        var ee = Array.Find(formMain.employeeList, (e) => e.employee_id == attendance.employee_id);

                        fingerPictureBox.Image = ee != null && ee.image != null ? ee.image : BiometricAttendance.Properties.Resources.ee_ok;

                        labelFinger.BackColor = Color.Green;
                        labelFinger.Text = attendance.date + " recorded to " + attendance.name;
                    }
                }
                else
                {
                    fingerPictureBox.Image = BiometricAttendance.Properties.Resources.error;
                    labelFinger.BackColor = Color.Red;
                    if (status == "9")
                    {
                        labelFinger.Text = "Fingerprint not registered";
                    }
                    else if (status == "23") 
                    {
                        labelFinger.Text = "Remove finger in the sensor and try again";
                    }
                    else
                    {
                        labelFinger.Text = "Fingerprint error: " + status;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            counter = 0;
            timer.Stop();
            timer.Start();

        }

        private void TimerTick(object sender, EventArgs e) 
        {
            counter++;
            if (counter >= 5)
            {
                timer.Stop();
                counter = 0;
                labelFinger.Text = "Place your finger in the sensor";
                labelFinger.BackColor = Color.DodgerBlue;
                fingerPictureBox.Image = BiometricAttendance.Properties.Resources.fingerprint;
            }
        }

        public void DisplayAttendanceToday() 
        {
            this.Invoke((MethodInvoker)delegate {
                listBoxAttendance.Items.Clear();
            });

            var today = DateTime.Now;
            string[] attendanceToday = Array.Empty<string>();

            for (int i = formMain.attendaceList.Length; i > 0; i--)
            {
                var at = formMain.attendaceList[i - 1];
                var dt = DateTime.Parse(at.date);

                if (today.Year == dt.Year && today.Month == dt.Month && today.Day == dt.Day)
                {
                    var time = dt.ToLongTimeString();
                    this.Invoke((MethodInvoker)delegate {
                        listBoxAttendance.Items.Add($"{time} - {at.name}");
                    });
                }

            }
        }

    }
}
