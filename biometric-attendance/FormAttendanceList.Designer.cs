namespace BiometricAttendance
{
    partial class FormAttendanceList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.attendanceDataGridView = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.employee_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // attendanceDataGridView
            // 
            this.attendanceDataGridView.AllowUserToAddRows = false;
            this.attendanceDataGridView.AllowUserToDeleteRows = false;
            this.attendanceDataGridView.AllowUserToResizeRows = false;
            this.attendanceDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attendanceDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.employee_id,
            this.name,
            this.date});
            this.attendanceDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attendanceDataGridView.Location = new System.Drawing.Point(0, 0);
            this.attendanceDataGridView.MultiSelect = false;
            this.attendanceDataGridView.Name = "attendanceDataGridView";
            this.attendanceDataGridView.ReadOnly = true;
            this.attendanceDataGridView.RowHeadersVisible = false;
            this.attendanceDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.attendanceDataGridView.ShowEditingIcon = false;
            this.attendanceDataGridView.Size = new System.Drawing.Size(654, 450);
            this.attendanceDataGridView.TabIndex = 0;
            // 
            // id
            // 
            this.id.FillWeight = 50F;
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Width = 50;
            // 
            // employee_id
            // 
            this.employee_id.HeaderText = "Employee ID";
            this.employee_id.Name = "employee_id";
            this.employee_id.ReadOnly = true;
            // 
            // name
            // 
            this.name.FillWeight = 200F;
            this.name.HeaderText = "Employee Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 200;
            // 
            // date
            // 
            this.date.FillWeight = 300F;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 300;
            // 
            // FormAttendanceList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 450);
            this.Controls.Add(this.attendanceDataGridView);
            this.Name = "FormAttendanceList";
            this.Text = "FormAttendanceList";
            this.Load += new System.EventHandler(this.FormAttendanceList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.attendanceDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView attendanceDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn employee_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
    }
}