using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
//using InnoMetricsAPI;
//using InnoMetricsAPI.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SystemInfo.classes;

namespace SystemInfo
{
    public partial class LoginForm : Form
    {
        public string secret_token;
        

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoginForm()
        {
            InitializeComponent();
        }

        //clear user inputs 
        private void ClearTexts(string user, string pass)
        {
            user = String.Empty;
            pass = String.Empty;
        }

        bool IsLoggedIn(string user, string pass)
        {
            //check user name empty 
            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Enter the user name!");
                return false;

            }
            else//check password is empty 
                if (string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Enter the passowrd!");
                return false;
            }
            else
            {
                return true;
            }
        }
        

        private void Button1_Click(object sender, EventArgs e)
        {
            //define local variables from the user inputs 
            string email = txtemail.Text;
            string password = txtpassword.Text;

            String token = APIClient.Client.getLoginToken(email, password);
            

            if(!String.IsNullOrEmpty(token))
            {
                secret_token = token;
                frmMain.myConfig["TOKEN"] = token;
                frmMain.myConfig["USERNAME"] = email;
                frmMain.myConfig["PASSWORD"] = password;
                SqliteDataAccess.saveMyConfig(frmMain.myConfig);
                //frmMain newForm = new frmMain();
                //newForm.sendToServer(secret_token);
                this.Close();
            }
            else
            {
                MessageBox.Show("username and password are incorrect, please verify them", "InnoMetrics", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Form nForm = new RegistrationForm();
            nForm.ShowDialog();
            this.Show();

        }

        private void password_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            /*
            log.Info("Login form load");

            myConfig = SqliteDataAccess.loadInitialConfig();

            if (!String.IsNullOrEmpty(myConfig["USERNAME"]) && !String.IsNullOrEmpty(myConfig["PASSWORD"]))
            {
                myConfig["TOKEN"] = getLoginToken(myConfig["USERNAME"], myConfig["PASSWORD"]);
                MessageBox.Show("try to login automatically with username and password", "InnoMetrics", MessageBoxButtons.OK);
            }
            */
        }
    }
}
