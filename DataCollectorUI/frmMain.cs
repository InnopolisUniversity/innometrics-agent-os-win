using APIClient;
using InnoMetricDataAccess;
using InnoMetricsCollector;
using InnoMetricsCollector.classes;
using InnoMetric.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



using System.Management;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;


using HWND = System.IntPtr;

namespace DataCollectorUI
{
    public partial class frmMain : Form
    {

        ReportGenerator myCollector;

        CollectorReport myLastReport;
        Thread check;

        Thread dataSync;
        frmSystemInfo mySystemInfoForm;
        CollectorActivity activeApp;
        CollectorActivity prevActiveApp;

        int COLLECTION_INTERVAL;
        int SENDING_INTERVAL;

        bool abortDataCollection;
        bool abortDataSync;

        public static Dictionary<String, String> myConfig;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            da.CheckDB();

            log.Info("Loading app...");

            this.ShowInTaskbar = false;
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            updateConfig();

            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...", ToolTipIcon.Info);

            abortDataCollection = false;
            abortDataSync = false;

            check = new Thread(loadReport);
            check.Start();

            dataSync = new Thread(syncData);
            dataSync.Start();
            mySystemInfoForm = new frmSystemInfo();
        }
        
        private void updateConfig()
        {
            DataAccess da = new DataAccess();
            myConfig = da.loadInitialConfig();

            COLLECTION_INTERVAL = (myConfig.ContainsKey("COLLECTION_INTERVAL") ? int.Parse(myConfig["COLLECTION_INTERVAL"].ToString()) : 1) * 60 * 1000;
            SENDING_INTERVAL = (myConfig.ContainsKey("SENDING_INTERVAL") ? int.Parse(myConfig["SENDING_INTERVAL"].ToString()) : 5) * 60 * 1000;
        }

        private void loadData()
        {
            log.Info("Loading data...");
            DataAccess da = new DataAccess();
            richTextBox1.Text = "";
            List<String> records = da.loadProcessHistory();

            //log.Info(records.ToString());

            foreach (string r in records)
            {
                richTextBox1.Text += r + "\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadData();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            //log.Info("Executing process...");
            //loadReport();
            //await loadReport2();
        }

        private void loadReport()
        {
            try
            {
                while (true)
                {
                    
                    myCollector = new ReportGenerator();
                    CollectorReport myReport = myCollector.getCurrentProcessReport();
                    log.Info("Currect active activities -> " + myReport.activities.Count);
                    activeApp = null;
                    if (myLastReport != null)
                    {
                        //var result = myLastReport.activities.Where(p => !myReport.activities.Any(p2 => p2.ProcessId == p.ProcessId && p2.ExecutableName == p.ExecutableName));

                        foreach (var app in myReport.activities)
                        {
                            log.Info("App -> " + app.ExecutableName + ", " + app.Description);
                            var lastState = myLastReport.activities.Where(p => app.ProcessId == p.ProcessId && app.ExecutableName == p.ExecutableName).FirstOrDefault();
                            if (lastState != null)
                            {
                                var myLastMeasure = lastState.Measurements.FirstOrDefault(m => m.MeasurementType == "1"); //1 -> EstimatedChargeRemaining
                                var myCurrentMeasure = app.Measurements.FirstOrDefault(m => m.MeasurementType == "1");//1 -> EstimatedChargeRemaining

                                if (myCurrentMeasure.Value != "-1")
                                {
                                    if (Double.Parse(myLastMeasure.Value) > Double.Parse(myCurrentMeasure.Value))
                                    {

                                        var BatteryConsumption = app.Measurements.FirstOrDefault(m => m.MeasurementType == "6");// 6 - BatteryConsumption


                                        if (BatteryConsumption != null)
                                        {
                                            BatteryConsumption.Value = (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value)).ToString();// (Double.Parse(BatteryConsumption.Value) + (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value))).ToString();
                                        }
                                        else
                                        {
                                            app.Measurements.Add(new Metrics
                                            {
                                                MeasurementType = "6", // 6 - BatteryConsumption
                                                Value = (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value)).ToString()
                                            });
                                        }
                                    }
                                }
                            }

                            
                        }

                        

                        DataAccess da = new DataAccess();

                        foreach (var app in myReport.activities)//result)
                        {
                            log.Info("Saving activity data...");
                            da.SaveMyActivity(app);
                        }
                    }

                    myLastReport = myReport;

                    mySystemInfoForm.topActivity = null;
                    mySystemInfoForm.topIdleApp = new List<CollectorActivity>();
                    int counter = 1;

                    var idleApps = myLastReport.activities.OrderByDescending(a => a.StartTime);

                    foreach (var app in myLastReport.activities)
                    {
                        if (!app.IdleActivity) mySystemInfoForm.topActivity = app;
                        if (counter <= 3) mySystemInfoForm.topIdleApp.Add(app);
                        counter++;
                    }

                    //mySystemInfoForm.updateView();
#if DEBUG
                    Thread.Sleep(30000);
#else
                    Thread.Sleep(COLLECTION_INTERVAL);
#endif


                    if (abortDataCollection)
                    {
                        log.Debug("stopping thread DataCollection");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.Debug(check.ThreadState.ToString());
            log.Debug(dataSync.ThreadState.ToString());

            abortThreads();

            if (check.ThreadState == System.Threading.ThreadState.Running)
                check.Abort();

            if (dataSync.ThreadState == System.Threading.ThreadState.Running)
                dataSync.Abort();
            
            log.Debug(check.ThreadState.ToString());
            log.Debug(dataSync.ThreadState.ToString());

            Application.Exit();
        }

        private void abortThreads()
        {
            Environment.Exit(0);
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                frmSettings myForm = new frmSettings();
                myForm.ShowDialog();
                myForm.Dispose();

                updateConfig();
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
            Visible = true;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to quit InnoMetrics Collector?", "InnoMetrics data collector", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                abortThreads();
                if (check.ThreadState == System.Threading.ThreadState.Running)
                    check.Abort();

                if (dataSync.ThreadState == System.Threading.ThreadState.Running)
                    dataSync.Abort();

                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
                Visible = false;
                notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...", ToolTipIcon.Info);
            }
            notifyIcon1.Visible = true;

        }
        public void showNotification(String message, ToolTipIcon icon)
        {
            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", message, icon);
        }


       

        private void syncData()
        {
            try
            {
                while (true)
                {
                    if (myConfig.ContainsKey("TOKEN"))
                    {
                        if (!String.IsNullOrEmpty(myConfig["TOKEN"]))
                        {
                            DataAccess da = new DataAccess();

                            String token = Client.getLoginToken(myConfig["USERNAME"], myConfig["PASSWORD"]);

                            Report records = da.reportGenerator(myConfig["USERNAME"]);
                            log.Info("Submiting request...");
                            bool result = Client.SaveReport(records, token);// myConfig["TOKEN"]);
                            log.Info("Updating activity status...");
                            da.updateActivityStatus(DataAccess.ActivityStatus.Processing, result ? DataAccess.ActivityStatus.Accepted : DataAccess.ActivityStatus.Error);
                            log.Info("Process finished...");
                        }
                        else
                        {
                            log.Error("Error in the service starting process, please check the user credentials...");
                            showNotification("There is an error sending the data collected, please check the user credentials...", ToolTipIcon.Error);
                        }
                    }
                    Thread.Sleep(SENDING_INTERVAL);
                    if (abortDataSync)
                    {
                        log.Debug("stopping thread DataSync");
                        break;

                    }
                }
            }
            catch (Exception ex) {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);

            }
            

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                mySystemInfoForm.ShowDialog();
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }
    }
}
