namespace BiometricAttendance
{
    partial class FormStart
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
            this.labelFinger = new System.Windows.Forms.Label();
            this.labelDateTime = new System.Windows.Forms.Label();
            this.listBoxAttendance = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fingerPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fingerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // labelFinger
            // 
            this.labelFinger.BackColor = System.Drawing.Color.DodgerBlue;
            this.labelFinger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFinger.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFinger.ForeColor = System.Drawing.Color.White;
            this.labelFinger.Location = new System.Drawing.Point(3, 374);
            this.labelFinger.Name = "labelFinger";
            this.labelFinger.Size = new System.Drawing.Size(418, 30);
            this.labelFinger.TabIndex = 0;
            this.labelFinger.Text = "Place your finger in the sensor";
            this.labelFinger.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDateTime
            // 
            this.labelDateTime.BackColor = System.Drawing.SystemColors.Control;
            this.labelDateTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDateTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDateTime.Location = new System.Drawing.Point(3, 0);
            this.labelDateTime.Name = "labelDateTime";
            this.labelDateTime.Size = new System.Drawing.Size(418, 30);
            this.labelDateTime.TabIndex = 0;
            this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listBoxAttendance
            // 
            this.listBoxAttendance.BackColor = System.Drawing.Color.White;
            this.listBoxAttendance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxAttendance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxAttendance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxAttendance.FormattingEnabled = true;
            this.listBoxAttendance.ItemHeight = 16;
            this.listBoxAttendance.Location = new System.Drawing.Point(3, 452);
            this.listBoxAttendance.Name = "listBoxAttendance";
            this.listBoxAttendance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBoxAttendance.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxAttendance.Size = new System.Drawing.Size(418, 194);
            this.listBoxAttendance.TabIndex = 0;
            this.listBoxAttendance.TabStop = false;
            this.listBoxAttendance.UseTabStops = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 424);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(418, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Attendance Today";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelDateTime, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBoxAttendance, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelFinger, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(424, 649);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fingerPictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(418, 338);
            this.panel1.TabIndex = 1;
            // 
            // fingerPictureBox
            // 
            this.fingerPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fingerPictureBox.Image = global::BiometricAttendance.Properties.Resources.fingerprint;
            this.fingerPictureBox.Location = new System.Drawing.Point(0, 0);
            this.fingerPictureBox.Name = "fingerPictureBox";
            this.fingerPictureBox.Padding = new System.Windows.Forms.Padding(20);
            this.fingerPictureBox.Size = new System.Drawing.Size(418, 338);
            this.fingerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.fingerPictureBox.TabIndex = 1;
            this.fingerPictureBox.TabStop = false;
            // 
            // FormStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(424, 649);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormStart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Biometric Attendance";
            this.Load += new System.EventHandler(this.FormAttendance_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fingerPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelFinger;
        private System.Windows.Forms.Label labelDateTime;
        private System.Windows.Forms.ListBox listBoxAttendance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox fingerPictureBox;
    }
}