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
        private int index = 0;

        private void FormAttendanceList_Load(object sender, EventArgs e)
        {
            LoadAttendance();
            LoadCombobox();
        }

        private void LoadAttendance() 
        {
            attendanceDataGridView.Rows.Clear();
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

        private void LoadCombobox()
        {
            comboBoxEmployee.Items.Clear();
            comboBoxEmployee.Items.Add("All");
            comboBoxEmployee.SelectedIndex = 0;
            foreach (ModelEmployee ee in formMain.employeeList)
            {
                comboBoxEmployee.Items.Add($"{ee.employee_id} - {ee.name}");
            }
        }

        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            Console.WriteLine("from: {0}", dateTimePickerStart.Value);
            Console.WriteLine("to: {0}", dateTimePickerStart.Value);

            

            if (index > 0)
            {
                var ee = formMain.employeeList[index - 1];
                Console.WriteLine("EE: {0}", ee.name);
            }
        }

        private void ComboBoxEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = comboBoxEmployee.SelectedIndex;
        }
    }

}
