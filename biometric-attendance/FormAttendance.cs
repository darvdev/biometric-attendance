using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace biometric_attendance
{
    public partial class FormAttendance : Form
    {
        public FormAttendance()
        {
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];

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
        }
    }
}
