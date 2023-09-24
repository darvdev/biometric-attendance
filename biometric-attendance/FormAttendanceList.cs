using biometric_attendance;
using System;
using System.Windows.Forms;

namespace BiometricAttendance
{
    public partial class FormAttendanceList : Form
    {
        public FormAttendanceList()
        {
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];

        private void FormAttendanceList_Load(object sender, EventArgs e)
        {
            foreach (ModelAttendance attendance in formMain.attendaceList)
            {
                attendanceDataGridView.Rows.Add(new object[] {
                        attendance.employee_id,
                        attendance.name,
                        attendance.date,
                     });

            }

            attendanceDataGridView.ClearSelection();
        }
    }
}
