using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APIClient;
using InnoMetricDataAccess;

namespace DataCollectorUI
{
    public partial class frmLogin : Form
    {
        Boolean _requestLogin;


        public frmLogin()
        {
            InitializeComponent();
            _requestLogin = false;
        }

        public frmLogin(Boolean requestLogin)
        {
            InitializeComponent();
            _requestLogin = requestLogin;
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            //define local variables from the user inputs 
            string email = txtemail.Text;
            string password = txtpassword.Text;

            String token = Client.getLoginToken(email, password);


            if (!String.IsNullOrEmpty(token))
            {
                frmSettings.myConfig["TOKEN"] = token;
                frmSettings.myConfig["USERNAME"] = email;
                frmSettings.myConfig["PASSWORD"] = password;
                //DataAccess.saveMyConfig(frmSettings.myConfig);
                //frmMain newForm = new frmMain();
                //newForm.sendToServer(secret_token);
                _requestLogin = false;
                this.Close();
            }
            else
            {
                MessageBox.Show("username and password are incorrect, please verify them", "InnoMetrics", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            
            if (_requestLogin)
            {
                MessageBox.Show("Please introduce you credential and clic in login.", "InnoMetrics", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }
    }
}
