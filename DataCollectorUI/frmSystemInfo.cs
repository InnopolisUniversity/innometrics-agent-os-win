using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using InnoMetricDataAccess;
using InnoMetricsCollector;
using InnoMetricsCollector.DTO;
using log4net;

namespace DataCollectorUI
{
    public partial class FrmSystemInfo : Form
    {
        public static DateTime idleTimeStart;
        public static bool isIdle;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CollectorActivity topActivity;
        public List<string> topIdleApp;

        public FrmSystemInfo()
        {
            InitializeComponent();
            UpdateView();
            UpdateTopIdleApps();
            idleTimeStart = new DateTime();
            //isIdle = false;
        }

        private void FrmSystemInfo_Load(object sender, EventArgs e)
        {
            var generator = new ReportGenerator();
            var da = new DataAccess();
            var myConfig = da.LoadInitialConfig();
            try
            {
                lblOS.Text = generator.GetOSVersion();
                lblUserName.Text = generator.GetCurrentUser();
                lblLogin.Text = myConfig["USERNAME"];
                lblIP.Text = generator.GetIpAddress();
                lblMAC.Text = generator.GetMACAddress();
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }

            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = ""; // fileVersionInfo.ProductVersion;

            if (Debugger.IsAttached)
                version = "Debug Mode";
            else
                try
                {
                    version = ApplicationDeployment.CurrentDeployment.CurrentVersion.Major + "." +
                              ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor + "." +
                              ApplicationDeployment.CurrentDeployment.CurrentVersion.Build + "." +
                              ApplicationDeployment.CurrentDeployment.CurrentVersion
                                  .Revision;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }

            Text = "InnoMetrics collector - " + version;
        }

        public void UpdateView()
        {
            topActivity = frmMain.myCurrentActivity;
            if (topActivity != null)
            {
                //isIdle = false;
                //timerIdleCounter.Enabled = false;
                pbActiveAppIcon.Image = Icon.ExtractAssociatedIcon(topActivity.mainAppPath).ToBitmap();
                lblActiveAppName.Text = "App name: \t" + topActivity.ExecutableName;
                lblActiveAppTime.Text = "Running time: \t" +
                                        new DateTime((DateTime.Now - topActivity.StartTime).Ticks)
                                            .ToString("HH:mm:ss"); // topActivity.StartTime.ToString();
            }
            /*else
            {
                pbActiveAppIcon.Image = null;
                lblActiveAppName.Text = "The is not active application";
                lblActiveAppTime.Text = "";
                if (!isIdle)
                {
                    isIdle = true;
                    idleTimeStart = DateTime.Now;
                    //timerIdleCounter.Enabled = true;
                }

            }

            /*

            lblTopApp1.Text = "No data available";
            lblTopApp2.Text = "No data available";
            lblTopApp3.Text = "No data available";

            if (topIdleApp != null)
            {
                try {
                    lblTopApp1.Text = topIdleApp[0].ExecutableName;
                    lblTopApp2.Text = topIdleApp[1].ExecutableName;
                    lblTopApp3.Text = topIdleApp[2].ExecutableName;
                }catch(Exception ex)
                {
                    
                }
                
            }*/

            Refresh();
        }

        private void UpdateTopIdleApps()
        {
            lblTopApp1.Text = "No data available";
            lblTopApp2.Text = "No data available";
            lblTopApp3.Text = "No data available";

            var da = new DataAccess();
            var topIdleApp = da.LoadGetTopIdleApps();
            if (topIdleApp != null)
                try
                {
                    lblTopApp1.Text = topIdleApp.Count != 0 ? topIdleApp[0] : "";
                    lblTopApp2.Text = topIdleApp.Count > 1 ? topIdleApp[1] : lblTopApp2.Text;
                    lblTopApp3.Text = topIdleApp.Count > 2 ? topIdleApp[2] : lblTopApp3.Text;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }

            Refresh();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void TimerIdleCounter_Tick(object sender, EventArgs e)
        {
            //if (isIdle)
            //{
            lblTotalIdleTime.Text = new DateTime((DateTime.Now - idleTimeStart).Ticks).ToString("HH:mm:ss");
            /*}
            else
            {
                if(topActivity != null)
                    if (topActivity.StartTime != null)
                        lblActiveAppTime.Text = "Running time: \t" + new DateTime((DateTime.Now - topActivity.StartTime).Ticks).ToString("HH:mm:ss");// topActivity.StartTime.ToString();
            }*/
        }

        private void TimerTopApps_Tick(object sender, EventArgs e)
        {
            UpdateTopIdleApps();
        }
    }
}