using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InnoMetricDataAccess;
using Microsoft.Win32;

namespace DataCollectorUI
{
    public partial class frmSettings : Form
    {
        public static Dictionary<string, string> myConfig;
        private readonly bool _requestLogin;

        public frmSettings()
        {
            InitializeComponent();
            _requestLogin = false;
        }

        public frmSettings(bool requestLogIn)
        {
            InitializeComponent();
            _requestLogin = requestLogIn;
        }

        private void btnSesionControl_Click(object sender, EventArgs e)
        {
            var frmLogin = new frmLogin(_requestLogin);
            frmLogin.ShowDialog();
            frmLogin.Dispose();
            populateUserFrame();
            if (_requestLogin)
            {
                button1_Click(sender, e);
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var da = new DataAccess();
            myConfig["COLLECTION_INTERVAL"] = nudCollectionInterval.Value.ToString();
            myConfig["SENDING_INTERVAL"] = nudSendingInterval.Value.ToString();
            myConfig["STARTONLOGIN"] = chkStart.Checked ? "Y" : "N";
            myConfig["CHKUPDATE"] = chkUpdate.Checked ? "Y" : "N";

            da.SaveMyConfig(myConfig);

            var startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs)
                            + @"\Innopolis university\InnoMetrics data collector\Data collector.appref-ms";

            //MessageBox.Show(startPath, "InnoMetrics data collector", MessageBoxButtons.OK, MessageBoxIcon.Information);

            using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (chkStart.Checked)
                    key.SetValue("InnoMetrics", startPath);
                else
                    key.DeleteValue("InnoMetrics", false);
            }

            /*
            if (srvController.Status != System.ServiceProcess.ServiceControllerStatus.Running)
            {
                srvController.Start();
                showNotification("The sync service is runnning now", ToolTipIcon.Info);
            }
            */

            MessageBox.Show("The configuration was saved", "InnoMetrics data collector", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            try
            {
                //textBox1.Text = Application.ExecutablePath;
                var da = new DataAccess();
                myConfig = da.LoadInitialConfig();

                lblUserName.Text = "User name: " + myConfig["USERNAME"];

                populateUserFrame();

                if (myConfig["COLLECTION_INTERVAL"] != string.Empty)
                    nudCollectionInterval.Value = decimal.Parse(myConfig["COLLECTION_INTERVAL"]);

                if (myConfig["SENDING_INTERVAL"] != string.Empty)
                    nudSendingInterval.Value = decimal.Parse(myConfig["SENDING_INTERVAL"]);

                chkStart.Checked = myConfig["STARTONLOGIN"] == "Y";
                chkUpdate.Checked = myConfig["CHKUPDATE"] == "Y";
                if (_requestLogin) btnSesionControl_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void populateUserFrame()
        {
            if (myConfig["USERNAME"] != string.Empty)
            {
                lblUserName.Text = "User name: " + myConfig["USERNAME"];
                lblUserStatus.Text = "Status: " + "Logged";
                lblLastDataCollectedTime.Text = "Latest data submission: " + myConfig["LATEST_SUBMISSION"];
                btnSesionControl.Text = "Log&in";
            }
            else
            {
                lblUserName.Text = "User name: ";
                lblUserStatus.Text = "Status: " + "Not available";
                lblLastDataCollectedTime.Text = "Latest data submission: " + myConfig["LATEST_SUBMISSION"];
                btnSesionControl.Text = "Log&in";
            }
        }
    }
}