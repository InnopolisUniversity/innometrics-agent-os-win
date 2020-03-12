using InnoMetricDataAccess;
using Microsoft.Win32;
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
        Boolean _requestLogin;
        public static Dictionary<String, String> myConfig;

        public frmSettings()
        {
            InitializeComponent();
            _requestLogin = false;
        }

        public frmSettings(Boolean requestLogIn)
        {
            InitializeComponent();
            _requestLogin = requestLogIn;
        }

        private void btnSesionControl_Click(object sender, EventArgs e)
        {
            
            frmLogin frmLogin = new frmLogin(_requestLogin);
            frmLogin.ShowDialog();
           frmLogin.Dispose();
            populateUserFrame();
            if (_requestLogin)
            {
                button1_Click(sender, e);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            myConfig["COLLECTION_INTERVAL"] = nudCollectionInterval.Value.ToString();
            myConfig["SENDING_INTERVAL"] = nudSendingInterval.Value.ToString();
            myConfig["STARTONLOGIN"] = chkStart.Checked ? "Y" : "N";
            myConfig["CHKUPDATE"] = chkUpdate.Checked ? "Y" : "N";

            da.SaveMyConfig(myConfig);

            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs)
                   + @"\Innopolis university\InnoMetrics data collector\Data collector.appref-ms";

            //MessageBox.Show(startPath, "InnoMetrics data collector", MessageBoxButtons.OK, MessageBoxIcon.Information);

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
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

            MessageBox.Show("The configuration was saved", "InnoMetrics data collector", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            try
            {
                //textBox1.Text = Application.ExecutablePath;
                DataAccess da = new DataAccess();
                myConfig = da.LoadInitialConfig();

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
                if (_requestLogin) btnSesionControl_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void populateUserFrame()
        {
            if (myConfig["USERNAME"] != String.Empty)
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
