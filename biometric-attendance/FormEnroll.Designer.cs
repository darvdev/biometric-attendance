﻿namespace BiometricAttendance
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
            this.comboBoxStudentList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonEnroll = new System.Windows.Forms.Button();
            this.comboBoxBiometricId = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxStudent = new System.Windows.Forms.PictureBox();
            this.pictureBoxEnroll = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStudent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEnroll)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxStudentList
            // 
            this.comboBoxStudentList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStudentList.FormattingEnabled = true;
            this.comboBoxStudentList.Location = new System.Drawing.Point(133, 37);
            this.comboBoxStudentList.Name = "comboBoxStudentList";
            this.comboBoxStudentList.Size = new System.Drawing.Size(262, 21);
            this.comboBoxStudentList.TabIndex = 0;
            this.comboBoxStudentList.SelectedIndexChanged += new System.EventHandler(this.ComboBoxStudentList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Student";
            // 
            // buttonEnroll
            // 
            this.buttonEnroll.Enabled = false;
            this.buttonEnroll.Location = new System.Drawing.Point(317, 90);
            this.buttonEnroll.Name = "buttonEnroll";
            this.buttonEnroll.Size = new System.Drawing.Size(78, 23);
            this.buttonEnroll.TabIndex = 2;
            this.buttonEnroll.Text = "Enroll";
            this.buttonEnroll.UseVisualStyleBackColor = true;
            this.buttonEnroll.Click += new System.EventHandler(this.ButtonEnroll_Click);
            // 
            // comboBoxBiometricId
            // 
            this.comboBoxBiometricId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBiometricId.Enabled = false;
            this.comboBoxBiometricId.FormattingEnabled = true;
            this.comboBoxBiometricId.Location = new System.Drawing.Point(133, 90);
            this.comboBoxBiometricId.Name = "comboBoxBiometricId";
            this.comboBoxBiometricId.Size = new System.Drawing.Size(178, 21);
            this.comboBoxBiometricId.TabIndex = 1;
            this.comboBoxBiometricId.SelectedIndexChanged += new System.EventHandler(this.ComboBoxBiometricId_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select Biometric ID";
            // 
            // pictureBoxStudent
            // 
            this.pictureBoxStudent.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBoxStudent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxStudent.Location = new System.Drawing.Point(25, 21);
            this.pictureBoxStudent.Name = "pictureBoxStudent";
            this.pictureBoxStudent.Size = new System.Drawing.Size(90, 90);
            this.pictureBoxStudent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxStudent.TabIndex = 4;
            this.pictureBoxStudent.TabStop = false;
            // 
            // pictureBoxEnroll
            // 
            this.pictureBoxEnroll.Image = global::BiometricAttendance.Properties.Resources.fingerprint;
            this.pictureBoxEnroll.Location = new System.Drawing.Point(111, 141);
            this.pictureBoxEnroll.Name = "pictureBoxEnroll";
            this.pictureBoxEnroll.Size = new System.Drawing.Size(200, 200);
            this.pictureBoxEnroll.TabIndex = 6;
            this.pictureBoxEnroll.TabStop = false;
            // 
            // labelStatus
            // 
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.ForeColor = System.Drawing.Color.Black;
            this.labelStatus.Location = new System.Drawing.Point(0, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(424, 24);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "Select student from dropdown";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 137);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(424, 24);
            this.panel1.TabIndex = 8;
            // 
            // FormEnroll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(424, 161);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBoxEnroll);
            this.Controls.Add(this.pictureBoxStudent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxBiometricId);
            this.Controls.Add(this.comboBoxStudentList);
            this.Controls.Add(this.buttonEnroll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 200);
            this.Name = "FormEnroll";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enroll Student";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStudent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEnroll)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxStudentList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonEnroll;
        private System.Windows.Forms.ComboBox comboBoxBiometricId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBoxStudent;
        private System.Windows.Forms.PictureBox pictureBoxEnroll;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Panel panel1;
    }
}