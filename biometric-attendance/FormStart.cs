using BiometricAttendance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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
            Console.WriteLine("Send: standby");
            formMain.serial.WriteLine("standby");
            formMain.Show();

            
        }


        private void FormAttendance_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Send: start");
            formMain.serial.WriteLine("start");
            timer.Interval = 1000;
            timer.Tick += TimerTick;
            DisplayAttendance();
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
                        fingerPictureBox.Image = BiometricAttendance.Properties.Resources.error;
                        labelFinger.Text = "Employee not found";
                    }
                    else
                    {
                        fingerPictureBox.Image = BiometricAttendance.Properties.Resources.ok;
                        labelFinger.Text = "Hello " + attendance.name;
                    }
                    
                }
                else
                {
                    fingerPictureBox.Image = BiometricAttendance.Properties.Resources.error;

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
                        labelFinger.Text = "Error: " + status;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateBiometric error: {0}", ex.Message);
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
                fingerPictureBox.Image = BiometricAttendance.Properties.Resources.fingerprint;
            }
        }

        public void DisplayAttendance() 
        {
            
            this.Invoke((MethodInvoker)delegate {
                listBoxAttendance.Items.Clear();
            });

            Task.Run(() => {

                var today = DateTime.Now;
                string[] attendanceToday = Array.Empty<string>();

                for (int i = formMain.attendaceList.Length; i > 0; i--)
                {
                    var at = formMain.attendaceList[i-1];
                    var dt = DateTime.Parse(at.date);

                    if (today.Year == dt.Year && today.Month == dt.Month && today.Day == dt.Day)
                    {
                        var time = dt.ToLongTimeString();
                        this.Invoke((MethodInvoker)delegate {
                            listBoxAttendance.Items.Add($"{time} - {at.name}");
                        });
                    }

                }
            });
        }

    }
}
