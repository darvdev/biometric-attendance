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
            dateTimePickerStart.Value = DateTime.Parse(DateTime.Now.ToShortDateString() + " 12:00:00 am");
            dateTimePickerEnd.Value = DateTime.Parse(DateTime.Now.ToShortDateString() + " 11:59:59 pm");

            LoadAttendance();
            LoadCombobox();
        }

        private void LoadAttendance(ModelStudent student = null, DateTime? start = null, DateTime? end = null)
        {
            attendanceDataGridView.Rows.Clear();

            var list = formMain.attendaceList;

            if (student != null) 
            {
                list = Array.FindAll(formMain.attendaceList, (a) => a.student_id == student.student_id);
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
                        attendance.student_id,
                        attendance.name,
                        attendance.date,
                     });
            }

            attendanceDataGridView.ClearSelection();
        }

        private void LoadCombobox()
        {
            comboBoxStudent.Items.Clear();
            comboBoxStudent.Items.Add("All");
            comboBoxStudent.SelectedIndex = 0;
            foreach (ModelStudent student in formMain.students)
            {
                comboBoxStudent.Items.Add($"{student.student_id} - {student.name}");
            }
        }

        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            ModelStudent student = null;
            if (index > 0)
            {
                student = formMain.students[index - 1];
            }
            LoadAttendance(student, dateTimePickerStart.Value, dateTimePickerEnd.Value);
        }

        private void ComboBoxStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = comboBoxStudent.SelectedIndex;
        }

        private void CheckBoxFilter_CheckedChanged(object sender, EventArgs e)
        {
            panelDate.Enabled = checkBoxFilter.Checked;
        }
    }

}

