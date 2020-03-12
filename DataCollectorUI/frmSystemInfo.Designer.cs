namespace DataCollectorUI
{
    partial class FrmSystemInfo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSystemInfo));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMAC = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblOS = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblActiveAppTime = new System.Windows.Forms.Label();
            this.lblActiveAppName = new System.Windows.Forms.Label();
            this.pbActiveAppIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblTopApp3 = new System.Windows.Forms.Label();
            this.lblTopApp2 = new System.Windows.Forms.Label();
            this.lblTopApp1 = new System.Windows.Forms.Label();
            this.lblTotalIdleTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerIdleCounter = new System.Windows.Forms.Timer(this.components);
            this.timerTopApps = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbActiveAppIcon)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblMAC);
            this.groupBox1.Controls.Add(this.lblIP);
            this.groupBox1.Controls.Add(this.lblLogin);
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.lblOS);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lbl);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(11, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(377, 173);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current session";
            // 
            // lblMAC
            // 
            this.lblMAC.AutoSize = true;
            this.lblMAC.Location = new System.Drawing.Point(141, 143);
            this.lblMAC.Name = "lblMAC";
            this.lblMAC.Size = new System.Drawing.Size(96, 19);
            this.lblMAC.TabIndex = 9;
            this.lblMAC.Text = "MAC-address:";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(141, 115);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(77, 19);
            this.lblIP.TabIndex = 8;
            this.lblIP.Text = "IP-address:";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(141, 87);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(78, 19);
            this.lblLogin.TabIndex = 7;
            this.lblLogin.Text = "User name:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(141, 59);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(78, 19);
            this.lblUserName.TabIndex = 6;
            this.lblUserName.Text = "User name:";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.Location = new System.Drawing.Point(141, 31);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(117, 19);
            this.lblOS.TabIndex = 5;
            this.lblOS.Text = "Operated system:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 19);
            this.label5.TabIndex = 4;
            this.label5.Text = "MAC-address:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "IP-address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "User login:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "User name:";
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl.Location = new System.Drawing.Point(17, 31);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(127, 19);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "Operated system:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(11, 187);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(377, 163);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblActiveAppTime);
            this.tabPage1.Controls.Add(this.lblActiveAppName);
            this.tabPage1.Controls.Add(this.pbActiveAppIcon);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Size = new System.Drawing.Size(369, 133);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Focus Application Metric";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblActiveAppTime
            // 
            this.lblActiveAppTime.AutoSize = true;
            this.lblActiveAppTime.Location = new System.Drawing.Point(107, 69);
            this.lblActiveAppTime.Name = "lblActiveAppTime";
            this.lblActiveAppTime.Size = new System.Drawing.Size(0, 19);
            this.lblActiveAppTime.TabIndex = 3;
            // 
            // lblActiveAppName
            // 
            this.lblActiveAppName.AutoSize = true;
            this.lblActiveAppName.Location = new System.Drawing.Point(107, 43);
            this.lblActiveAppName.Name = "lblActiveAppName";
            this.lblActiveAppName.Size = new System.Drawing.Size(0, 19);
            this.lblActiveAppName.TabIndex = 2;
            // 
            // pbActiveAppIcon
            // 
            this.pbActiveAppIcon.Location = new System.Drawing.Point(17, 43);
            this.pbActiveAppIcon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbActiveAppIcon.Name = "pbActiveAppIcon";
            this.pbActiveAppIcon.Size = new System.Drawing.Size(71, 55);
            this.pbActiveAppIcon.TabIndex = 1;
            this.pbActiveAppIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Currently active process";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblTopApp3);
            this.tabPage2.Controls.Add(this.lblTopApp2);
            this.tabPage2.Controls.Add(this.lblTopApp1);
            this.tabPage2.Controls.Add(this.lblTotalIdleTime);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage2.Size = new System.Drawing.Size(369, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Idle Time Metric";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblTopApp3
            // 
            this.lblTopApp3.AutoSize = true;
            this.lblTopApp3.Location = new System.Drawing.Point(138, 104);
            this.lblTopApp3.Name = "lblTopApp3";
            this.lblTopApp3.Size = new System.Drawing.Size(0, 19);
            this.lblTopApp3.TabIndex = 5;
            // 
            // lblTopApp2
            // 
            this.lblTopApp2.AutoSize = true;
            this.lblTopApp2.Location = new System.Drawing.Point(138, 80);
            this.lblTopApp2.Name = "lblTopApp2";
            this.lblTopApp2.Size = new System.Drawing.Size(0, 19);
            this.lblTopApp2.TabIndex = 4;
            // 
            // lblTopApp1
            // 
            this.lblTopApp1.AutoSize = true;
            this.lblTopApp1.Location = new System.Drawing.Point(138, 54);
            this.lblTopApp1.Name = "lblTopApp1";
            this.lblTopApp1.Size = new System.Drawing.Size(0, 19);
            this.lblTopApp1.TabIndex = 3;
            // 
            // lblTotalIdleTime
            // 
            this.lblTotalIdleTime.AutoSize = true;
            this.lblTotalIdleTime.Location = new System.Drawing.Point(138, 16);
            this.lblTotalIdleTime.Name = "lblTotalIdleTime";
            this.lblTotalIdleTime.Size = new System.Drawing.Size(44, 19);
            this.lblTotalIdleTime.TabIndex = 2;
            this.lblTotalIdleTime.Text = "00:00";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 19);
            this.label7.TabIndex = 1;
            this.label7.Text = "Top 3 Idle apps";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 19);
            this.label6.TabIndex = 0;
            this.label6.Text = "Total Idle Time";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // timerIdleCounter
            // 
            this.timerIdleCounter.Enabled = true;
            this.timerIdleCounter.Interval = 1000;
            this.timerIdleCounter.Tick += new System.EventHandler(this.TimerIdleCounter_Tick);
            // 
            // timerTopApps
            // 
            this.timerTopApps.Enabled = true;
            this.timerTopApps.Interval = 60000;
            this.timerTopApps.Tick += new System.EventHandler(this.TimerTopApps_Tick);
            // 
            // frmSystemInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 358);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSystemInfo";
            this.Text = "InnoMetrics collector";
            this.Load += new System.EventHandler(this.FrmSystemInfo_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbActiveAppIcon)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMAC;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblOS;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblActiveAppTime;
        private System.Windows.Forms.Label lblActiveAppName;
        private System.Windows.Forms.PictureBox pbActiveAppIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblTopApp3;
        private System.Windows.Forms.Label lblTopApp2;
        private System.Windows.Forms.Label lblTopApp1;
        private System.Windows.Forms.Label lblTotalIdleTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerIdleCounter;
        private System.Windows.Forms.Timer timerTopApps;
    }
}