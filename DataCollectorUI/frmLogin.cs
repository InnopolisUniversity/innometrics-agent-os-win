﻿using System;
using System.Windows.Forms;
using APIClient;

namespace DataCollectorUI
{
    public partial class frmLogin : Form
    {
        private bool _requestLogin;


        public frmLogin()
        {
            InitializeComponent();
            _requestLogin = false;
        }

        public frmLogin(bool requestLogin)
        {
            InitializeComponent();
            _requestLogin = requestLogin;
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            //define local variables from the user inputs 
            var email = txtemail.Text;
            var password = txtpassword.Text;

            var token = Client.GetLoginToken(email, password);


            if (!string.IsNullOrEmpty(token))
            {
                frmSettings.myConfig["TOKEN"] = token;
                frmSettings.myConfig["USERNAME"] = email;
                frmSettings.myConfig["PASSWORD"] = password;
                //DataAccess.saveMyConfig(frmSettings.myConfig);
                //frmMain newForm = new frmMain();
                //newForm.sendToServer(secret_token);
                _requestLogin = false;
                Close();
            }
            else
            {
                MessageBox.Show("username and password are incorrect, please verify them", "InnoMetrics",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            label1.Visible = _requestLogin;
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Environment.Exit(0);

            if (_requestLogin)
            {
                var result = MessageBox.Show("Do you really want to close InnoMetrics data collecctor?", "InnoMetrics",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                    Environment.Exit(0);
                else
                    e.Cancel = true;
            }
        }

        private void txtemail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) 13) txtpassword.Focus();
        }

        private void txtpassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) 13) btnSignIn_Click(sender, e);
        }
    }
}