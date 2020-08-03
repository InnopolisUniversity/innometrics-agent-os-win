using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnoMetricsCollector;
using InnoMetricsCollector.classes;
using InnoMetricDataAccess;
using log4net;

namespace DataCollectorUI
{
    public partial class FrmSystemInfo : Form
    {
        public CollectorActivity topActivity;
        public List<String> topIdleApp;
        public static DateTime idleTimeStart;
        public static Boolean isIdle;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            ReportGenerator generator = new ReportGenerator();
            DataAccess da = new DataAccess();
            Dictionary<String, String> myConfig = da.LoadInitialConfig();
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
            string version = "";// fileVersionInfo.ProductVersion;

            if (System.Diagnostics.Debugger.IsAttached)
                version = "Debug Mode";
            else
            {
                try
                {
                    version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Major.ToString() + "." +
                    System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor.ToString() + "." +
                    System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Build.ToString() + "." +
                    System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision.ToString();
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }

            this.Text = "InnoMetrics collector - " + version;

        }

        public void UpdateView()
        {
            topActivity = frmMain.myCurrentActivity;
            if (topActivity != null) {
                //isIdle = false;
                //timerIdleCounter.Enabled = false;
                pbActiveAppIcon.Image = Icon.ExtractAssociatedIcon(topActivity.mainAppPath).ToBitmap();
                lblActiveAppName.Text = "App name: \t" + topActivity.ExecutableName;
                lblActiveAppTime.Text = "Running time: \t" + new DateTime((DateTime.Now - topActivity.StartTime).Ticks).ToString("HH:mm:ss");// topActivity.StartTime.ToString();
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

            this.Refresh();
            
        }

        private void UpdateTopIdleApps()
        {
            lblTopApp1.Text = "No data available";
            lblTopApp2.Text = "No data available";
            lblTopApp3.Text = "No data available";

            DataAccess da = new DataAccess();
            List<String> topIdleApp = da.LoadGetTopIdleApps();
            if (topIdleApp != null)
            {
                try
                {
                    lblTopApp1.Text = topIdleApp[0];
                    lblTopApp2.Text = topIdleApp[1];
                    lblTopApp3.Text = topIdleApp[2];
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }

            }

            this.Refresh();
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
