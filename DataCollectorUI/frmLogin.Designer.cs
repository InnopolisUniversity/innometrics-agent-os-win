namespace DataCollectorUI
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.lblPasswordRecovery = new System.Windows.Forms.LinkLabel();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.txtemail = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.Label();
            this.email = new System.Windows.Forms.Label();
            this.btnSignIn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPasswordRecovery
            // 
            this.lblPasswordRecovery.AutoSize = true;
            this.lblPasswordRecovery.Location = new System.Drawing.Point(121, 170);
            this.lblPasswordRecovery.Name = "lblPasswordRecovery";
            this.lblPasswordRecovery.Size = new System.Drawing.Size(162, 17);
            this.lblPasswordRecovery.TabIndex = 9;
            this.lblPasswordRecovery.TabStop = true;
            this.lblPasswordRecovery.Text = "Don\'t have an account ?";
            this.lblPasswordRecovery.Visible = false;
            // 
            // txtpassword
            // 
            this.txtpassword.Font = new System.Drawing.Font("Segoe UI Light", 10F);
            this.txtpassword.Location = new System.Drawing.Point(107, 73);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PasswordChar = '*';
            this.txtpassword.Size = new System.Drawing.Size(286, 30);
            this.txtpassword.TabIndex = 5;
            this.txtpassword.UseSystemPasswordChar = true;
            // 
            // txtemail
            // 
            this.txtemail.Font = new System.Drawing.Font("Segoe UI Light", 10F);
            this.txtemail.Location = new System.Drawing.Point(107, 39);
            this.txtemail.Name = "txtemail";
            this.txtemail.Size = new System.Drawing.Size(286, 30);
            this.txtemail.TabIndex = 4;
            // 
            // password
            // 
            this.password.AutoSize = true;
            this.password.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password.Location = new System.Drawing.Point(11, 73);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(84, 23);
            this.password.TabIndex = 7;
            this.password.Text = "Password:";
            // 
            // email
            // 
            this.email.AutoSize = true;
            this.email.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.email.Location = new System.Drawing.Point(11, 42);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(55, 23);
            this.email.TabIndex = 6;
            this.email.Text = "Email:";
            // 
            // btnSignIn
            // 
            this.btnSignIn.BackColor = System.Drawing.SystemColors.Control;
            this.btnSignIn.Location = new System.Drawing.Point(145, 119);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(113, 42);
            this.btnSignIn.TabIndex = 8;
            this.btnSignIn.Text = "Sign In";
            this.btnSignIn.UseVisualStyleBackColor = false;
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(93, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Please introduce your credentials below";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 169);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPasswordRecovery);
            this.Controls.Add(this.txtpassword);
            this.Controls.Add(this.txtemail);
            this.Controls.Add(this.password);
            this.Controls.Add(this.email);
            this.Controls.Add(this.btnSignIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InnoMetrics - Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblPasswordRecovery;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.TextBox txtemail;
        private System.Windows.Forms.Label password;
        private System.Windows.Forms.Label email;
        private System.Windows.Forms.Button btnSignIn;
        private System.Windows.Forms.Label label1;
    }
}