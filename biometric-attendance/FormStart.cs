using System;
using System.Drawing;
using System.Windows.Forms;

namespace BiometricAttendance
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
            formMain.serial.WriteLine("standby");
            formMain.BiometricEvent -= BiometricEvent;
            formMain.DateEvent -= DateEvent;
            formMain.AttendanceListEvent -= AttendanceListEvent;
            formMain.Show();
            base.OnFormClosing(e);
            
        }

        private void Invoke(Action action) => this.Invoke((MethodInvoker)delegate { action(); });

        private void FormAttendance_Load(object sender, EventArgs e)
        {
            formMain.BiometricEvent += BiometricEvent;
            formMain.DateEvent += DateEvent;
            formMain.AttendanceListEvent += AttendanceListEvent;

            formMain.serial.WriteLine("start");
            timer.Interval = 1000;
            timer.Tick += TimerTick;
            AttendanceListEvent(this, new CustomEventArgs(attendanceList: formMain.attendaceList));
        }

        private void DateEvent(object o, CustomEventArgs e)
        {
            Invoke(() => labelDateTime.Text = e.value);
        }

        private void BiometricEvent(object sender, CustomEventArgs e)
        {

            //status description
            /*
             * 100 = ok
             * 9 = Finger Print not found
             * other = Unknown
             */

            Invoke(() => {
                try
                {
                    if (e.value == "100")
                    {
                        if (e.attendance == null)
                        {
                            fingerPictureBox.Image = Properties.Resources.ee_error;
                            labelFinger.Text = "Fingerprint OK. Employee not found";
                            labelFinger.BackColor = Color.Red;
                        }
                        else
                        {
                            var ee = Array.Find(formMain.employeeList, (o) => o.employee_id == e.attendance.employee_id);

                            fingerPictureBox.Image = ee != null && ee.image != null ? ee.image : Properties.Resources.ee_ok;

                            labelFinger.BackColor = Color.Green;
                            labelFinger.Text = e.attendance.date + " recorded to " + e.attendance.name;
                        }
                    }
                    else
                    {
                        fingerPictureBox.Image = BiometricAttendance.Properties.Resources.error;
                        labelFinger.BackColor = Color.Red;
                        if (e.value == "9")
                        {
                            labelFinger.Text = "Fingerprint not registered";
                        }
                        else if (e.value == "23")
                        {
                            labelFinger.Text = "Remove finger in the sensor and try again";
                        }
                        else
                        {
                            labelFinger.Text = "Fingerprint error: " + e.value;
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
            });

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
                fingerPictureBox.Image = Properties.Resources.fingerprint;
            }
        }

        private void AttendanceListEvent(object o, CustomEventArgs e)
        {
            Invoke(() => listBoxAttendance.Items.Clear());

            var today = DateTime.Now;
            string[] attendanceToday = Array.Empty<string>();

            for (int i = e.attendanceList.Length; i > 0; i--)
            {
                var at = e.attendanceList[i - 1];
                var dt = DateTime.Parse(at.date);

                if (today.Year == dt.Year && today.Month == dt.Month && today.Day == dt.Day)
                {
                    var time = dt.ToLongTimeString();
                    Invoke(() => listBoxAttendance.Items.Add($"{time} - {at.name}"));
                }

            }
        }

    }
}
