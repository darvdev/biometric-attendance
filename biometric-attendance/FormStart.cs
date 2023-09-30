using Dapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            if (formMain.admins.Length == 0)
            {
                CloseForm();
                base.OnFormClosing(e);
            }
            else
            {
                var result = new FormLogin().ShowDialog();

                if (result == DialogResult.OK)
                {
                    CloseForm();
                    base.OnFormClosing(e);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            
        }

        private void CloseForm()
        {
            formMain.serial.WriteLine("standby");
            formMain.BiometricEvent -= BiometricEvent;
            formMain.DateEvent -= DateEvent;
            formMain.AttendanceListEvent -= AttendanceListEvent;
            formMain.Show();
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
                            var student = Array.Find(formMain.students, (o) => o.student_id == e.attendance.student_id);

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

            ModelAttendance[] attendanceToday = Array.FindAll(e.attendanceList, (a) => {
                var dt = DateTime.Parse(a.date);
                return today.Year == dt.Year && today.Month == dt.Month && today.Day == dt.Day;
            });

            //List<string> ids = new List<string>();

            //List<object> list = new List<object>();

            attendanceToday = attendanceToday.Reverse().ToArray();

            for (int i = 0; i< attendanceToday.Length; i++)
            {
                var at = attendanceToday[i];
                var dt = DateTime.Parse(at.date);


                //var result = Array.FindAll(attendanceToday, (a) => a.student_id == at.student_id);

                //bool even1 = result.Length % 2 == 0;


                var time = dt.ToLongTimeString();

                //var output = ids.FindAll((id) => id == at.student_id);

                //var type = (output.Count()) % 2 == 0 ? "IN" : "OUT";


                //Console.WriteLine("name: {0}, count: {1}, i: {2}, TIME {3}, result: {4}", at.name, output.Count(), i, type, result.Length);

                //ids.Add(at.student_id);

                Invoke(() => listBoxAttendance.Items.Add($"{time} - {at.name}"));

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
