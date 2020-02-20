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

namespace DataCollectorUI
{
    public partial class frmSystemInfo : Form
    {
        public CollectorActivity topActivity;
        public List<CollectorActivity> topIdleApp;
        public DateTime idleTimeStart;
        private Boolean isIdle;

        public frmSystemInfo()
        {
            InitializeComponent();
            updateView();
            idleTimeStart = new DateTime();
            isIdle = false;
        }

        private void frmSystemInfo_Load(object sender, EventArgs e)
        {
            ReportGenerator generator = new ReportGenerator();
            DataAccess da = new DataAccess();
            Dictionary<String, String> myConfig = da.loadInitialConfig();

            lblOS.Text = generator.getOSVersion();
            lblUserName.Text = generator.getCurrentUser();
            lblLogin.Text = myConfig["USERNAME"];
            lblIP.Text = generator.getIpAddress();
            lblMAC.Text = generator.getMACAddress();
        }

        public void updateView()
        {

            if (topActivity != null) {
                isIdle = false;
                //timerIdleCounter.Enabled = false;
                pbActiveAppIcon.Image = Icon.ExtractAssociatedIcon(topActivity.mainAppPath).ToBitmap();
                lblActiveAppName.Text = "App name: \t" + topActivity.ExecutableName;
                lblActiveAppTime.Text = "Running time: \t" + new DateTime((DateTime.Now - topActivity.StartTime).Ticks).ToString("HH:mm:ss");// topActivity.StartTime.ToString();
            }
            else
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
                
            }

            this.Refresh();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateView();
        }

        private void timerIdleCounter_Tick(object sender, EventArgs e)
        {
            if (isIdle)
            {
                lblTotalIdleTime.Text = new DateTime((DateTime.Now - idleTimeStart).Ticks).ToString("HH:mm:ss");
            }
            else
            {
                if(topActivity != null)
                    if (topActivity.StartTime != null)
                        lblActiveAppTime.Text = "Running time: \t" + new DateTime((DateTime.Now - topActivity.StartTime).Ticks).ToString("HH:mm:ss");// topActivity.StartTime.ToString();
            }
        }
    }
}
