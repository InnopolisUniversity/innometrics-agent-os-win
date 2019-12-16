namespace SystemInfo
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.MetricView = new System.Windows.Forms.ListView();
            this.Process_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.P_ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stats = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StartTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EndTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ip_address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mac_address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.des = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Battery = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSendData = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnLoadHistory = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Others = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // MetricView
            // 
            this.MetricView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Process_name,
            this.P_ID,
            this.stats,
            this.StartTime,
            this.EndTime,
            this.ip_address,
            this.mac_address,
            this.des,
            this.Battery,
            this.Others});
            this.MetricView.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MetricView.HideSelection = false;
            this.MetricView.Location = new System.Drawing.Point(12, 12);
            this.MetricView.Name = "MetricView";
            this.MetricView.Size = new System.Drawing.Size(1231, 250);
            this.MetricView.TabIndex = 0;
            this.MetricView.UseCompatibleStateImageBehavior = false;
            this.MetricView.View = System.Windows.Forms.View.Details;
            // 
            // Process_name
            // 
            this.Process_name.Text = "Process name";
            this.Process_name.Width = 145;
            // 
            // P_ID
            // 
            this.P_ID.Text = "PID";
            this.P_ID.Width = 79;
            // 
            // stats
            // 
            this.stats.Text = "Status";
            this.stats.Width = 87;
            // 
            // StartTime
            // 
            this.StartTime.Text = "Start time";
            this.StartTime.Width = 100;
            // 
            // EndTime
            // 
            this.EndTime.Text = "End time";
            this.EndTime.Width = 104;
            // 
            // ip_address
            // 
            this.ip_address.Text = "ip address";
            this.ip_address.Width = 78;
            // 
            // mac_address
            // 
            this.mac_address.Text = "mac address";
            this.mac_address.Width = 134;
            // 
            // des
            // 
            this.des.Text = "Description";
            this.des.Width = 166;
            // 
            // Battery
            // 
            this.Battery.Text = "Battery Consumed";
            this.Battery.Width = 115;
            // 
            // btnSendData
            // 
            this.btnSendData.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendData.Location = new System.Drawing.Point(384, 268);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(240, 40);
            this.btnSendData.TabIndex = 1;
            this.btnSendData.Text = "Send Data";
            this.btnSendData.UseVisualStyleBackColor = true;
            this.btnSendData.Click += new System.EventHandler(this.Button1_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnLoadHistory
            // 
            this.btnLoadHistory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadHistory.Location = new System.Drawing.Point(630, 268);
            this.btnLoadHistory.Name = "btnLoadHistory";
            this.btnLoadHistory.Size = new System.Drawing.Size(240, 40);
            this.btnLoadHistory.TabIndex = 2;
            this.btnLoadHistory.Text = "Show data collected";
            this.btnLoadHistory.UseVisualStyleBackColor = true;
            this.btnLoadHistory.Click += new System.EventHandler(this.btnLoadHistory_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 326);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1231, 231);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // Others
            // 
            this.Others.Text = "Other";
            this.Others.Width = 300;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1255, 568);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnLoadHistory);
            this.Controls.Add(this.btnSendData);
            this.Controls.Add(this.MetricView);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data collector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView MetricView;
        private System.Windows.Forms.ColumnHeader Process_name;
        private System.Windows.Forms.ColumnHeader P_ID;
        private System.Windows.Forms.ColumnHeader stats;
        private System.Windows.Forms.ColumnHeader EndTime;
        private System.Windows.Forms.ColumnHeader ip_address;
        private System.Windows.Forms.ColumnHeader des;
        private System.Windows.Forms.ColumnHeader StartTime;
        private System.Windows.Forms.ColumnHeader mac_address;
        private System.Windows.Forms.Button btnSendData;
        private System.Windows.Forms.ColumnHeader Battery;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnLoadHistory;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ColumnHeader Others;
    }
}

