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

        private void LoadAttendance(ModelEmployee ee = null, DateTime? start = null, DateTime? end = null)
        {
            attendanceDataGridView.Rows.Clear();

            var list = formMain.attendaceList;

            if (ee != null) 
            {
                list = Array.FindAll(formMain.attendaceList, (a) => a.employee_id == ee.employee_id);
            }

            if (checkBoxFilter.Checked)
            {
                if (start != null)
                {
                    list = Array.FindAll(list, (a) => {
                        var date = DateTime.Parse(a.date);
                        return date >= start;
                    });
                }

                if (end != null)
                {
                    list = Array.FindAll(list, (a) => {
                        var date = DateTime.Parse(a.date);
                        return date <= end;
                    });
                }
            }

            foreach (ModelAttendance attendance in list)
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
            Console.WriteLine("to: {0}", dateTimePickerEnd.Value);

            ModelEmployee ee = null;
            if (index > 0)
            {
                ee = formMain.employeeList[index - 1];
            }
            LoadAttendance(ee, dateTimePickerStart.Value, dateTimePickerEnd.Value);
        }

        private void ComboBoxEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = comboBoxEmployee.SelectedIndex;
        }

        private void CheckBoxFilter_CheckedChanged(object sender, EventArgs e)
        {
            panelDate.Enabled = checkBoxFilter.Checked;
        }
    }

}
