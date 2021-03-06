﻿namespace DataCollectorUI
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.nudCollectionInterval = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudSendingInterval = new System.Windows.Forms.NumericUpDown();
            this.btnSesionControl = new System.Windows.Forms.Button();
            this.chkUpdate = new System.Windows.Forms.CheckBox();
            this.chkStart = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLastDataCollectedTime = new System.Windows.Forms.Label();
            this.lblUserStatus = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.srvController = new System.ServiceProcess.ServiceController();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudCollectionInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSendingInterval)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nudCollectionInterval
            // 
            this.nudCollectionInterval.Location = new System.Drawing.Point(240, 228);
            this.nudCollectionInterval.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudCollectionInterval.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.nudCollectionInterval.Name = "nudCollectionInterval";
            this.nudCollectionInterval.Size = new System.Drawing.Size(107, 22);
            this.nudCollectionInterval.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 228);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data collection interval (minutes)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sending data interval (minutes)";
            // 
            // nudSendingInterval
            // 
            this.nudSendingInterval.Location = new System.Drawing.Point(240, 260);
            this.nudSendingInterval.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudSendingInterval.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.nudSendingInterval.Name = "nudSendingInterval";
            this.nudSendingInterval.Size = new System.Drawing.Size(107, 22);
            this.nudSendingInterval.TabIndex = 2;
            // 
            // btnSesionControl
            // 
            this.btnSesionControl.Location = new System.Drawing.Point(24, 102);
            this.btnSesionControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSesionControl.Name = "btnSesionControl";
            this.btnSesionControl.Size = new System.Drawing.Size(67, 24);
            this.btnSesionControl.TabIndex = 4;
            this.btnSesionControl.Text = "Log&in";
            this.btnSesionControl.UseVisualStyleBackColor = true;
            this.btnSesionControl.Click += new System.EventHandler(this.btnSesionControl_Click);
            // 
            // chkUpdate
            // 
            this.chkUpdate.AutoSize = true;
            this.chkUpdate.Location = new System.Drawing.Point(14, 17);
            this.chkUpdate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.Size = new System.Drawing.Size(203, 21);
            this.chkUpdate.TabIndex = 6;
            this.chkUpdate.Text = "Check update automatically";
            this.chkUpdate.UseVisualStyleBackColor = true;
            // 
            // chkStart
            // 
            this.chkStart.AutoSize = true;
            this.chkStart.Location = new System.Drawing.Point(14, 41);
            this.chkStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkStart.Name = "chkStart";
            this.chkStart.Size = new System.Drawing.Size(200, 21);
            this.chkStart.TabIndex = 7;
            this.chkStart.Text = "Start on login automatically";
            this.chkStart.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLastDataCollectedTime);
            this.groupBox1.Controls.Add(this.lblUserStatus);
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.btnSesionControl);
            this.groupBox1.Location = new System.Drawing.Point(14, 72);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(367, 135);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "User account";
            // 
            // lblLastDataCollectedTime
            // 
            this.lblLastDataCollectedTime.AutoSize = true;
            this.lblLastDataCollectedTime.Location = new System.Drawing.Point(20, 76);
            this.lblLastDataCollectedTime.Name = "lblLastDataCollectedTime";
            this.lblLastDataCollectedTime.Size = new System.Drawing.Size(161, 17);
            this.lblLastDataCollectedTime.TabIndex = 2;
            this.lblLastDataCollectedTime.Text = "Latest data submission: ";
            // 
            // lblUserStatus
            // 
            this.lblUserStatus.AutoSize = true;
            this.lblUserStatus.Location = new System.Drawing.Point(20, 50);
            this.lblUserStatus.Name = "lblUserStatus";
            this.lblUserStatus.Size = new System.Drawing.Size(56, 17);
            this.lblUserStatus.TabIndex = 1;
            this.lblUserStatus.Text = "Status: ";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(20, 25);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(85, 17);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User name: ";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(167, 289);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(67, 24);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // srvController
            // 
            this.srvController.ServiceName = "InnoMetricsDataSync";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "InnoMetrics Collector";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(18, 331);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(89, 22);
            this.textBox1.TabIndex = 9;
            this.textBox1.Visible = false;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 331);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkStart);
            this.Controls.Add(this.chkUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudSendingInterval);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudCollectionInterval);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data collector settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCollectionInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSendingInterval)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudCollectionInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudSendingInterval;
        private System.Windows.Forms.Button btnSesionControl;
        private System.Windows.Forms.CheckBox chkUpdate;
        private System.Windows.Forms.CheckBox chkStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLastDataCollectedTime;
        private System.Windows.Forms.Label lblUserStatus;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Button btnSave;
        private System.ServiceProcess.ServiceController srvController;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox textBox1;
    }
}