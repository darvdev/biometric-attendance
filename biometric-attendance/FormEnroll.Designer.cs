namespace biometric_attendance
{
    partial class FormEnroll
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
            this.comboBoxEmployeeList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonEnroll = new System.Windows.Forms.Button();
            this.comboBoxBiometricId = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelEnrollStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxEmployeeList
            // 
            this.comboBoxEmployeeList.FormattingEnabled = true;
            this.comboBoxEmployeeList.Location = new System.Drawing.Point(31, 37);
            this.comboBoxEmployeeList.Name = "comboBoxEmployeeList";
            this.comboBoxEmployeeList.Size = new System.Drawing.Size(209, 21);
            this.comboBoxEmployeeList.TabIndex = 0;
            this.comboBoxEmployeeList.SelectedIndexChanged += new System.EventHandler(this.comboBoxEmployeeList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Employee";
            // 
            // buttonEnroll
            // 
            this.buttonEnroll.Enabled = false;
            this.buttonEnroll.Location = new System.Drawing.Point(167, 78);
            this.buttonEnroll.Name = "buttonEnroll";
            this.buttonEnroll.Size = new System.Drawing.Size(73, 23);
            this.buttonEnroll.TabIndex = 2;
            this.buttonEnroll.Text = "Enroll";
            this.buttonEnroll.UseVisualStyleBackColor = true;
            this.buttonEnroll.Click += new System.EventHandler(this.Enroll);
            // 
            // comboBoxBiometricId
            // 
            this.comboBoxBiometricId.Enabled = false;
            this.comboBoxBiometricId.FormattingEnabled = true;
            this.comboBoxBiometricId.Location = new System.Drawing.Point(31, 80);
            this.comboBoxBiometricId.Name = "comboBoxBiometricId";
            this.comboBoxBiometricId.Size = new System.Drawing.Size(130, 21);
            this.comboBoxBiometricId.TabIndex = 1;
            this.comboBoxBiometricId.SelectedIndexChanged += new System.EventHandler(this.comboBoxBiometricId_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select Biometric ID";
            // 
            // labelEnrollStatus
            // 
            this.labelEnrollStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEnrollStatus.AutoSize = true;
            this.labelEnrollStatus.Location = new System.Drawing.Point(12, 143);
            this.labelEnrollStatus.Name = "labelEnrollStatus";
            this.labelEnrollStatus.Size = new System.Drawing.Size(66, 13);
            this.labelEnrollStatus.TabIndex = 3;
            this.labelEnrollStatus.Text = "Enroll Status";
            this.labelEnrollStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormEnroll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 191);
            this.Controls.Add(this.labelEnrollStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxBiometricId);
            this.Controls.Add(this.comboBoxEmployeeList);
            this.Controls.Add(this.buttonEnroll);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEnroll";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enroll";
            this.Load += new System.EventHandler(this.FormEnroll_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxEmployeeList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonEnroll;
        private System.Windows.Forms.ComboBox comboBoxBiometricId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelEnrollStatus;
    }
}