﻿using APIClient;
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

        CollectorProcessReport myLastReport;
        public static CollectorActivity myCurrentActivity;
        Thread check;

        Thread dataSync;
        FrmSystemInfo mySystemInfoForm;

        private int COLLECTION_INTERVAL;
        private int SENDING_INTERVAL;

        bool abortDataCollection;
        bool abortDataSync;

        public static Dictionary<String, String> myConfig;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private KeyboardTracker keyboard;
        private MouseTracker mouse;


        //LAST MOUSE AND KEYBOARD MOVEMENTS
        private DateTime last_mouse_signal;
        private DateTime last_keyboard_touch;

        ///////////////////////////////////////////////////////////////////////////////////////////
        const uint WINEVENT_OUTOFCONTEXT = 0;
        const uint EVENT_SYSTEM_FOREGROUND = 3;

        delegate void WinEventDelegate(IntPtr hWinEventHook,
            uint eventType, IntPtr hwnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin,
            uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);

        IntPtr m_hhook;

        private WinEventDelegate winEventProcDel;

        public frmMain()
        {
            InitializeComponent();

            keyboard = new KeyboardTracker();
            keyboard.KeyBoardKeyPressed += keyboard_KeyBoardKeyPressed;

            mouse = new MouseTracker();
            mouse.MouseMoved += mouse_MouseMoved;
        }


        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                DataAccess da = new DataAccess();
                da.CheckDB();

                log.Info("Loading app...");

                this.ShowInTaskbar = false;
                this.Visible = false;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();

                UpdateConfig();

                if (myConfig.ContainsKey("TOKEN"))
                {
                    if (String.IsNullOrEmpty(myConfig["TOKEN"]))
                    {
                        log.Error("Error in the service starting process, please check the user credentials...");
                        ShowNotification("There is an error sending the data collected, please check the user credentials...", ToolTipIcon.Error);

                        frmSettings myForm = new frmSettings(true);
                        myForm.ShowDialog();
                        //myForm.Dispose();

                        UpdateConfig();
                    }
                }

                notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...", ToolTipIcon.Info);

                abortDataCollection = false;
                abortDataSync = false;
                myCollector = new ReportGenerator();

                check = new Thread(LoadReport);
                check.Start();

                dataSync = new Thread(SyncData);
                dataSync.Start();

                mySystemInfoForm = new FrmSystemInfo();
                myCurrentActivity = null;

                winEventProcDel = new WinEventDelegate(WinEventProc);
                m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND,
                    EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    winEventProcDel, 0, 0, WINEVENT_OUTOFCONTEXT);
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                MessageBox.Show(ex.ToString());

            }
        }

        private void UpdateConfig()
        {
            DataAccess da = new DataAccess();
            myConfig = da.LoadInitialConfig();

            COLLECTION_INTERVAL = (myConfig.ContainsKey("COLLECTION_INTERVAL") ? int.Parse(myConfig["COLLECTION_INTERVAL"].ToString()) : 1) * 60 * 1000;
            SENDING_INTERVAL = (myConfig.ContainsKey("SENDING_INTERVAL") ? int.Parse(myConfig["SENDING_INTERVAL"].ToString()) : 5) * 60 * 1000;
        }

        private void LoadData()
        {
            log.Info("Loading data...");
            DataAccess da = new DataAccess();
            richTextBox1.Text = "";
            List<String> records = da.LoadProcessHistory();

            foreach (string r in records)
            {
                richTextBox1.Text += r + "\n";
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadReport()
        {
            try
            {
                log.Info("Loading report...");
                DateTime dataCollectionTime;
                while (true)
                {
                    //myCollector = new ReportGenerator();
                    log.Info("loadReport is being executed...");
                    dataCollectionTime = DateTime.Now;
                    CollectorProcessReport myReport = myCollector.GetCurrentProcessReport(dataCollectionTime);
                    log.Info("Currect active process -> " + myReport.processes.Count);
                    //activeApp = null;
                    if (myLastReport != null)
                    {
                        foreach (var app in myReport.processes)
                        {
                            log.Info("App -> " + app.ProcessName + ", " + app.Description + ", " + app.PID + ", " + app.ProcessID);
                            var lastState = myLastReport.processes.Where(p => app.PID == p.PID && app.ProcessName == p.ProcessName).FirstOrDefault();
                            if (lastState != null)
                            {
                                app.ProcessID = lastState.ProcessID;
                                var myLastMeasure = lastState.Measurements.FirstOrDefault(m => m.MeasurementType == "1"); //1 -> EstimatedChargeRemaining
                                var myCurrentMeasure = app.Measurements.FirstOrDefault(m => m.MeasurementType == "1");//1 -> EstimatedChargeRemaining

                                if (myCurrentMeasure != null && myCurrentMeasure.Value != "-1")
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
                                            app.Measurements.Add(new ProcessMetrics
                                            {
                                                MeasurementType = "6", // 6 - BatteryConsumption
                                                Value = (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value)).ToString(),
                                                CapturedTime = dataCollectionTime
                                            });
                                        }
                                    }
                                    else
                                    {
                                        // no battery consumption
                                        app.Measurements.Add(new ProcessMetrics
                                        {
                                            MeasurementType = "6", // 6 - BatteryConsumption
                                            Value = "0",
                                            CapturedTime = dataCollectionTime
                                        });
                                    }
                                }
                                else
                                {
                                    // no battery consumption
                                    app.Measurements.Add(new ProcessMetrics
                                    {
                                        MeasurementType = "6", // 6 - BatteryConsumption
                                        Value = "0",
                                        CapturedTime = dataCollectionTime
                                    });
                                }
                            }
                            else
                            {
                                // no battery consumption
                                app.Measurements.Add(new ProcessMetrics
                                {
                                    MeasurementType = "6", // 6 - BatteryConsumption
                                    Value = "0",
                                    CapturedTime = dataCollectionTime
                                });
                            }

                            log.Info("App -> " + app.ProcessName + ", " + app.Description + ", " + app.PID + ", " + app.ProcessID);
                        }



                        DataAccess da = new DataAccess();

                        foreach (var app in myReport.processes)//result)
                        {
                            log.Info("Saving process data...");
                            da.SaveMyProcess(app);
                        }
                    }

                    myLastReport = myReport;

                    /*
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
                    */

                    //mySystemInfoForm.updateView();
#if DEBUG
                    Thread.Sleep(15000);
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

        void keyboard_KeyBoardKeyPressed(object sender, EventArgs e)
        {
            last_keyboard_touch = DateTime.Now;
        }

        void mouse_MouseMoved(object sender, EventArgs e)
        {
            last_mouse_signal = DateTime.Now;
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.Debug(check.ThreadState.ToString());
            log.Debug(dataSync.ThreadState.ToString());

            AbortThreads();

            if (check.ThreadState == System.Threading.ThreadState.Running)
                check.Abort();

            if (dataSync.ThreadState == System.Threading.ThreadState.Running)
                dataSync.Abort();

            log.Debug(check.ThreadState.ToString());
            log.Debug(dataSync.ThreadState.ToString());

            Application.Exit();
        }

        private void AbortThreads()
        {
            log.Info("abort threads...");
            Environment.Exit(0);
        }


        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                frmSettings myForm = new frmSettings();
                myForm.ShowDialog();
                myForm.Dispose();

                UpdateConfig();
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
            Visible = true;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*log.Info("form closing...");
            if (MessageBox.Show("Do you want to quit InnoMetrics Collector?", "InnoMetrics data collector", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                AbortThreads();
                if (check.ThreadState == System.Threading.ThreadState.Running)
                    check.Abort();

                if (dataSync.ThreadState == System.Threading.ThreadState.Running)
                    dataSync.Abort();

                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }*/
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
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
        public void ShowNotification(String message, ToolTipIcon icon)
        {
            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", message, icon);
        }

        private void SyncData()
        {
            try
            {
                log.Info("Initializing sync data method...");
                while (true)
                {
                    log.Info("syncData is being executed...");
                    if (myConfig.ContainsKey("TOKEN"))
                    {
                        if (!String.IsNullOrEmpty(myConfig["TOKEN"]))
                        {
                            DataAccess da = new DataAccess();

                            String token = Client.getLoginToken(myConfig["USERNAME"], myConfig["PASSWORD"]);

                            AddProcessReportRequest records = da.ProcessReportGenerator(myConfig["USERNAME"]);

                            log.Info("Submiting process request... sending " + records.ProcessesReport.Count + " records");
                            bool result = Client.SaveProcessReport(records, token);// myConfig["TOKEN"]);
                            log.Info("Updating activity status...");
                            da.UpdateProcessStatus(DataAccess.ActivityStatus.Processing, result ? DataAccess.ActivityStatus.Accepted : DataAccess.ActivityStatus.Error);
                            da.CleanProcessDataHistory();
                            log.Info("Process finished...");


                            Report Activityrecords = da.ReportGenerator(myConfig["USERNAME"]);
                            log.Info("Submiting activity request... sending " + Activityrecords.Activities.Count + " records");
                            result = Client.SaveReport(Activityrecords, token);
                            da.UpdateActivityStatus(DataAccess.ActivityStatus.Processing, result ? DataAccess.ActivityStatus.Accepted : DataAccess.ActivityStatus.Error);
                            //da.CleanDataHistory();
                        }
                        else
                        {
                            log.Error("Error in the service starting process, please check the user credentials...");
                            ShowNotification("There is an error sending the data collected, please check the user credentials...", ToolTipIcon.Error);

                            frmSettings myForm = new frmSettings(true);
                            myForm.ShowDialog();
                            myForm.Dispose();

                            UpdateConfig();

                        }
                    }

#if DEBUG
                    Thread.Sleep(120000);
#else
                    Thread.Sleep(SENDING_INTERVAL);
#endif
                    if (abortDataSync)
                    {
                        log.Debug("stopping thread DataSync");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);

            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
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

        void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                log.Info("WinEventProc is being executed...");
                if (eventType == EVENT_SYSTEM_FOREGROUND)
                {
                    DateTime dataCollectionTime = DateTime.Now;
                    if (myCurrentActivity != null)
                    {
                        DataAccess da = new DataAccess();
                        myCurrentActivity.EndTime = dataCollectionTime;
                        da.SaveMyActivity(myCurrentActivity);
                        log.Debug("End of app tracked -> " + myCurrentActivity.ExecutableName + " - " + myCurrentActivity.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + " to " + myCurrentActivity.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    }
                    myCurrentActivity = myCollector.GetCurrentActivity(hwnd, dataCollectionTime);
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }

        }

        //ON CURRENT WINDOW UPDATE ACTION
        //INITIALIZE LAST_KEYBOARD_TOUCH TO ZERO(NULL)
        void simulation_method()
        {
            if (last_keyboard_touch != null || last_mouse_signal != null)
            {
                DateTime maximum_date = MaxDate(last_mouse_signal, last_keyboard_touch);
                DateTime present = DateTime.Now;
                double totalminutes = (present - maximum_date).TotalMinutes;

                if (totalminutes > 2)
                {
                    // set activity to idle, and start Idle counter
                    // on next windows update end Idle counter
                }
            }
        }

        public DateTime MaxDate(DateTime first, DateTime second)
        {
            if (first > second) return first;
            else return second;
        }
    }
}
