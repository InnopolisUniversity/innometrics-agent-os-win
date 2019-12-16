using InnoMetricDataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataCollectorUI
{
    public partial class frmSettings : Form
    {
        public static Dictionary<String, String> myConfig;

        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnSesionControl_Click(object sender, EventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.ShowDialog();
            frmLogin.Dispose();
            populateUserFrame();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataAccess.saveMyConfig(myConfig);

            myConfig["COLLECTION_INTERVAL"] = nudCollectionInterval.Value.ToString();
            myConfig["SENDING_INTERVAL"] = nudSendingInterval.Value.ToString();
            myConfig["STARTONLOGIN"] = chkStart.Checked ? "Y" : "N";
            myConfig["CHKUPDATE"] = chkUpdate.Checked ? "Y" : "N";



            if (srvController.Status != System.ServiceProcess.ServiceControllerStatus.Running)
            {
                srvController.Start();
                showNotification("The sync service is runnning now", ToolTipIcon.Info);
            }

            MessageBox.Show("The configuration was saved", "InnoMetrics data collector", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            myConfig = DataAccess.loadInitialConfig();

            lblUserName.Text = "User name: " + myConfig["USERNAME"];

            populateUserFrame();

            if (myConfig["COLLECTION_INTERVAL"] != String.Empty)
            {
                nudCollectionInterval.Value = Decimal.Parse(myConfig["COLLECTION_INTERVAL"]);
            }

            if (myConfig["SENDING_INTERVAL"] != String.Empty)
            {
                nudSendingInterval.Value = Decimal.Parse(myConfig["SENDING_INTERVAL"]);
            }

            chkStart.Checked = myConfig["STARTONLOGIN"] == "Y";
            chkUpdate.Checked = myConfig["CHKUPDATE"] == "Y";
        }

        private void populateUserFrame()
        {
            if (myConfig["USERNAME"] != String.Empty)
            {
                lblUserName.Text = "User name: " + myConfig["USERNAME"];
                lblUserStatus.Text = "Status: " + "Logged";
                lblLastDataCollectedTime.Text = "Latest data submission: " + myConfig["LATEST_SUBMISSION"];
                btnSesionControl.Text = "Log&out";
            }
            else
            {
                lblUserName.Text = "User name: ";
                lblUserStatus.Text = "Status: " + "Not available";
                lblLastDataCollectedTime.Text = "Latest data submission: " + myConfig["LATEST_SUBMISSION"];
                btnSesionControl.Text = "Log&in";
            }
        }
        public void showNotification(String message, ToolTipIcon icon)
        {
            notifyIcon1.Visible = false;
            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", message, icon);
        }
    }
}
