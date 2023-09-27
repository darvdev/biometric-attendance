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
             * 101 = Finger print read ok from sensor
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
                            labelFinger.Text = "Student not found";
                            labelFinger.BackColor = Color.Red;
                            //Helper.Speak("Employee not found");
                            Helper.PlayError();
                        }
                        else
                        {
                            var student = Array.Find(formMain.studentList, (o) => o.student_id == e.attendance.student_id);

                            fingerPictureBox.Image = student != null && student.image != null ? student.image : Properties.Resources.ee_ok;                                  

                            labelFinger.BackColor = Color.Green;

                            var date = DateTime.Parse(e.attendance.date);
                            labelFinger.Text = date.ToLongTimeString() + " today is recorded to " + e.attendance.name;
                            Helper.PlayOk();
                            //Helper.Speak("Thank you!");
                        }
                    }
                    else if (e.value == "101")
                    {
                        Helper.PlayBeep();
                        fingerPictureBox.Image = Properties.Resources.ok;
                        labelFinger.BackColor = Color.DodgerBlue;
                        labelFinger.Text = "Fingerprint OK";
                    }
                    else
                    {
                        fingerPictureBox.Image = Properties.Resources.error;
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
                        Helper.PlayError();
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

        //private void fingerPictureBox_Paint(object sender, PaintEventArgs e)
        //{
        //    Console.WriteLine("on paint");
        //    System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        //    path.AddEllipse(0, 0, fingerPictureBox.Width, fingerPictureBox.Height);
        //    fingerPictureBox.Region = new Region(path);
        //}
    }
}
