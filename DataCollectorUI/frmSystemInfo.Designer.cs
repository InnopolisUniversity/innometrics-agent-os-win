namespace DataCollectorUI
{
    partial class frmSystemInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSystemInfo));
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
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 216);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current session";
            // 
            // lblMAC
            // 
            this.lblMAC.AutoSize = true;
            this.lblMAC.Location = new System.Drawing.Point(159, 179);
            this.lblMAC.Name = "lblMAC";
            this.lblMAC.Size = new System.Drawing.Size(107, 21);
            this.lblMAC.TabIndex = 9;
            this.lblMAC.Text = "MAC-address:";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(159, 144);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(86, 21);
            this.lblIP.TabIndex = 8;
            this.lblIP.Text = "IP-address:";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(159, 109);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(88, 21);
            this.lblLogin.TabIndex = 7;
            this.lblLogin.Text = "User name:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(159, 74);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(88, 21);
            this.lblUserName.TabIndex = 6;
            this.lblUserName.Text = "User name:";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.Location = new System.Drawing.Point(159, 39);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(131, 21);
            this.lblOS.TabIndex = 5;
            this.lblOS.Text = "Operated system:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(19, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 21);
            this.label5.TabIndex = 4;
            this.label5.Text = "MAC-address:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 21);
            this.label4.TabIndex = 3;
            this.label4.Text = "IP-address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "User login:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "User name:";
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl.Location = new System.Drawing.Point(19, 39);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(142, 21);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "Operated system:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 234);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(424, 204);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblActiveAppTime);
            this.tabPage1.Controls.Add(this.lblActiveAppName);
            this.tabPage1.Controls.Add(this.pbActiveAppIcon);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(416, 170);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Focus Application Metric";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblActiveAppTime
            // 
            this.lblActiveAppTime.AutoSize = true;
            this.lblActiveAppTime.Location = new System.Drawing.Point(120, 86);
            this.lblActiveAppTime.Name = "lblActiveAppTime";
            this.lblActiveAppTime.Size = new System.Drawing.Size(0, 21);
            this.lblActiveAppTime.TabIndex = 3;
            // 
            // lblActiveAppName
            // 
            this.lblActiveAppName.AutoSize = true;
            this.lblActiveAppName.Location = new System.Drawing.Point(120, 54);
            this.lblActiveAppName.Name = "lblActiveAppName";
            this.lblActiveAppName.Size = new System.Drawing.Size(0, 21);
            this.lblActiveAppName.TabIndex = 2;
            // 
            // pbActiveAppIcon
            // 
            this.pbActiveAppIcon.Location = new System.Drawing.Point(19, 54);
            this.pbActiveAppIcon.Name = "pbActiveAppIcon";
            this.pbActiveAppIcon.Size = new System.Drawing.Size(80, 69);
            this.pbActiveAppIcon.TabIndex = 1;
            this.pbActiveAppIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
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
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(416, 170);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Idle Time Metric";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblTopApp3
            // 
            this.lblTopApp3.AutoSize = true;
            this.lblTopApp3.Location = new System.Drawing.Point(155, 130);
            this.lblTopApp3.Name = "lblTopApp3";
            this.lblTopApp3.Size = new System.Drawing.Size(0, 21);
            this.lblTopApp3.TabIndex = 5;
            // 
            // lblTopApp2
            // 
            this.lblTopApp2.AutoSize = true;
            this.lblTopApp2.Location = new System.Drawing.Point(155, 100);
            this.lblTopApp2.Name = "lblTopApp2";
            this.lblTopApp2.Size = new System.Drawing.Size(0, 21);
            this.lblTopApp2.TabIndex = 4;
            // 
            // lblTopApp1
            // 
            this.lblTopApp1.AutoSize = true;
            this.lblTopApp1.Location = new System.Drawing.Point(155, 68);
            this.lblTopApp1.Name = "lblTopApp1";
            this.lblTopApp1.Size = new System.Drawing.Size(0, 21);
            this.lblTopApp1.TabIndex = 3;
            // 
            // lblTotalIdleTime
            // 
            this.lblTotalIdleTime.AutoSize = true;
            this.lblTotalIdleTime.Location = new System.Drawing.Point(155, 20);
            this.lblTotalIdleTime.Name = "lblTotalIdleTime";
            this.lblTotalIdleTime.Size = new System.Drawing.Size(49, 21);
            this.lblTotalIdleTime.TabIndex = 2;
            this.lblTotalIdleTime.Text = "00:00";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 21);
            this.label7.TabIndex = 1;
            this.label7.Text = "Top 3 Idle apps";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 21);
            this.label6.TabIndex = 0;
            this.label6.Text = "Total Idle Time";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerIdleCounter
            // 
            this.timerIdleCounter.Enabled = true;
            this.timerIdleCounter.Interval = 1000;
            this.timerIdleCounter.Tick += new System.EventHandler(this.timerIdleCounter_Tick);
            // 
            // frmSystemInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 447);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSystemInfo";
            this.Text = "InnoMetrics collector";
            this.Load += new System.EventHandler(this.frmSystemInfo_Load);
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
    }
}