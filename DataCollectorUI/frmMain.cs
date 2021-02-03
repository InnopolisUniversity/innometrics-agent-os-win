using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using APIClient;
using InnoMetricDataAccess;
using InnoMetricsCollector;
using InnoMetricsCollector.DTO;
using log4net;
using HWND = System.IntPtr;

namespace DataCollectorUI
{
    public partial class frmMain : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_OBJECT_SELECTION = 32774;
        public static CollectorActivity myCurrentActivity;

        public static Dictionary<string, string> myConfig;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool abortDataCollection;
        private bool abortDataSync;
        private Thread activityTracking;
        private Thread check;

        private int COLLECTION_INTERVAL;
        private Thread dataSync;

        private readonly KeyboardTracker keyboard;
        private DateTime last_keyboard_touch;

        //LAST MOUSE AND KEYBOARD MOVEMENTS
        public DateTime last_mouse_signal;

        private IntPtr m_hhook;

        private readonly MouseTracker mouse;
        private ReportGenerator myCollector;

        private CollectorProcessReport myLastReport;
        private FrmSystemInfo mySystemInfoForm;
        private int SENDING_INTERVAL;

        private IntPtr tab_hhook;
        private WinEventDelegate tabEventProcDel;

        private WinEventDelegate winEventProcDel;

        public frmMain()
        {
            InitializeComponent();
            /*
            keyboard = new KeyboardTracker();
            keyboard.KeyBoardKeyPressed += keyboard_KeyBoardKeyPressed;

            mouse = new MouseTracker();
            mouse.MouseMoved += mouse_MouseMoved;
            */
        }

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin,
            uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);


        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                var da = new DataAccess();
                da.CheckDB();

                log.Info("Loading app...");

                ShowInTaskbar = false;
                Visible = false;
                WindowState = FormWindowState.Minimized;
                Hide();

                UpdateConfig();

                if (myConfig.ContainsKey("TOKEN"))
                    if (string.IsNullOrEmpty(myConfig["TOKEN"]))
                    {
                        log.Error("Error in the service starting process, please check the user credentials...");
                        ShowNotification(
                            "There is an error sending the data collected, please check the user credentials...",
                            ToolTipIcon.Error);

                        var myForm = new frmSettings(true);
                        myForm.ShowDialog();
                        //myForm.Dispose();

                        UpdateConfig();
                    }

                notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...",
                    ToolTipIcon.Info);

                abortDataCollection = false;
                abortDataSync = false;
                myCollector = new ReportGenerator();

                //check = new Thread(LoadReport);
                check = new Thread(processReportHandler);
                check.Start();

                dataSync = new Thread(SyncData);
                dataSync.Start();

                activityTracking = new Thread(simulation_method);
                activityTracking.Start();

                mySystemInfoForm = new FrmSystemInfo();
                myCurrentActivity = null;

                winEventProcDel = WinEventProc;
                tabEventProcDel = WinEventProc;
                m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND,
                    EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    winEventProcDel, 0, 0, WINEVENT_OUTOFCONTEXT);

                tab_hhook = SetWinEventHook(EVENT_OBJECT_SELECTION,
                    EVENT_OBJECT_SELECTION, IntPtr.Zero,
                    tabEventProcDel, 0, 0, WINEVENT_OUTOFCONTEXT);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateConfig()
        {
            var da = new DataAccess();
            myConfig = da.LoadInitialConfig();

            COLLECTION_INTERVAL =
                (myConfig.ContainsKey("COLLECTION_INTERVAL")
                    ? int.Parse(myConfig["COLLECTION_INTERVAL"])
                    : 1) * 60 * 1000;
            SENDING_INTERVAL =
                (myConfig.ContainsKey("SENDING_INTERVAL") ? int.Parse(myConfig["SENDING_INTERVAL"]) : 5) *
                60 * 1000;
        }

        private void LoadData()
        {
            log.Info("Loading data...");
            var da = new DataAccess();
            richTextBox1.Text = "";
            var records = da.LoadProcessHistory();

            foreach (var r in records) richTextBox1.Text += r + "\n";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void processReportHandler() {
            log.Info("Loading report...");
            
            List<Task> TasksList = new List<Task>();
            while (true)
            {
                var reportTask = Task.Run(async () => await ProcessReport());
                
#if DEBUG
                Thread.Sleep(15000);
#else
                Thread.Sleep(COLLECTION_INTERVAL);
#endif

                if (abortDataCollection)
                {
                    log.Info("stopping thread DataCollection");
                    break;
                }
            }
        }

        private async Task <Boolean> ProcessReport()
        {
            try
            {
                DateTime dataCollectionTime;
                //myCollector = new ReportGenerator();
                log.Info("loadReport is being executed...");
                dataCollectionTime = DateTime.Now;
                var myReport = myCollector.GetCurrentProcessReport(dataCollectionTime);
                log.Info("Currect active process -> " + myReport.processes.Count);
                //activeApp = null;
                if (myLastReport != null)
                {
                    foreach (var app in myReport.processes)
                    {
                        log.Info("App -> " + app.ProcessName + ", " + app.Description + ", " + app.PID + ", " +
                                 app.ProcessID);
                        var lastState = myLastReport.processes
                            .Where(p => app.PID == p.PID && app.ProcessName == p.ProcessName).FirstOrDefault();
                        if (lastState != null)
                        {
                            app.ProcessID = lastState.ProcessID;
                            var myLastMeasure =
                                lastState.Measurements.FirstOrDefault(m =>
                                    m.MeasurementType == "1"); //1 -> EstimatedChargeRemaining
                            var myCurrentMeasure =
                                app.Measurements.FirstOrDefault(m =>
                                    m.MeasurementType == "1"); //1 -> EstimatedChargeRemaining

                            if (myCurrentMeasure != null && myCurrentMeasure.Value != "-1")
                            {
                                if (double.Parse(myLastMeasure.Value) > double.Parse(myCurrentMeasure.Value))
                                {
                                    var BatteryConsumption =
                                        app.Measurements.FirstOrDefault(m =>
                                            m.MeasurementType == "6"); // 6 - BatteryConsumption


                                    if (BatteryConsumption != null)
                                        BatteryConsumption.Value =
                                            (double.Parse(myLastMeasure.Value) -
                                             double.Parse(myCurrentMeasure.Value))
                                            .ToString(); // (Double.Parse(BatteryConsumption.Value) + (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value))).ToString();
                                    else
                                        app.Measurements.Add(new ProcessMetrics
                                        {
                                            MeasurementType = "6", // 6 - BatteryConsumption
                                            Value = (double.Parse(myLastMeasure.Value) -
                                                     double.Parse(myCurrentMeasure.Value)).ToString(),
                                            CapturedTime = dataCollectionTime
                                        });
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

                        log.Info("App -> " + app.ProcessName + ", " + app.Description + ", " + app.PID + ", " +
                                 app.ProcessID);
                    }


                    var da = new DataAccess();

                    foreach (var app in myReport.processes) //result)
                    {
                        log.Info("Saving process data...");
                        da.SaveMyProcess(app);
                    }
                }

                myLastReport = myReport;
            }
            catch (Exception ex)
            {
                log.Info($"{ex.Message}, {ex.StackTrace}, {ex.Source}");
                return false;
            }
            return true;
        }



        #region OLD_PROCESS
        /*
        private void LoadReport()
        {
            log.Info("Loading report...");
            DateTime dataCollectionTime;
            while (true)
            {
                try
                {
                    //myCollector = new ReportGenerator();
                    log.Info("loadReport is being executed...");
                    dataCollectionTime = DateTime.Now;
                    var myReport = myCollector.GetCurrentProcessReport(dataCollectionTime);
                    log.Info("Currect active process -> " + myReport.processes.Count);
                    //activeApp = null;
                    if (myLastReport != null)
                    {
                        foreach (var app in myReport.processes)
                        {
                            log.Info("App -> " + app.ProcessName + ", " + app.Description + ", " + app.PID + ", " +
                                     app.ProcessID);
                            var lastState = myLastReport.processes
                                .Where(p => app.PID == p.PID && app.ProcessName == p.ProcessName).FirstOrDefault();
                            if (lastState != null)
                            {
                                app.ProcessID = lastState.ProcessID;
                                var myLastMeasure =
                                    lastState.Measurements.FirstOrDefault(m =>
                                        m.MeasurementType == "1"); //1 -> EstimatedChargeRemaining
                                var myCurrentMeasure =
                                    app.Measurements.FirstOrDefault(m =>
                                        m.MeasurementType == "1"); //1 -> EstimatedChargeRemaining

                                if (myCurrentMeasure != null && myCurrentMeasure.Value != "-1")
                                {
                                    if (double.Parse(myLastMeasure.Value) > double.Parse(myCurrentMeasure.Value))
                                    {
                                        var BatteryConsumption =
                                            app.Measurements.FirstOrDefault(m =>
                                                m.MeasurementType == "6"); // 6 - BatteryConsumption


                                        if (BatteryConsumption != null)
                                            BatteryConsumption.Value =
                                                (double.Parse(myLastMeasure.Value) -
                                                 double.Parse(myCurrentMeasure.Value))
                                                .ToString(); // (Double.Parse(BatteryConsumption.Value) + (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value))).ToString();
                                        else
                                            app.Measurements.Add(new ProcessMetrics
                                            {
                                                MeasurementType = "6", // 6 - BatteryConsumption
                                                Value = (double.Parse(myLastMeasure.Value) -
                                                         double.Parse(myCurrentMeasure.Value)).ToString(),
                                                CapturedTime = dataCollectionTime
                                            });
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

                            log.Info("App -> " + app.ProcessName + ", " + app.Description + ", " + app.PID + ", " +
                                     app.ProcessID);
                        }


                        var da = new DataAccess();

                        foreach (var app in myReport.processes) //result)
                        {
                            log.Info("Saving process data...");
                            da.SaveMyProcess(app);
                        }
                    }

                    myLastReport = myReport;

                }
                catch (Exception ex)
                {
                    log.Info($"{ex.Message}, {ex.StackTrace}, {ex.Source}");
                }
#if DEBUG
                Thread.Sleep(15000);
#else
                Thread.Sleep(COLLECTION_INTERVAL);
#endif

                if (abortDataCollection)
                {
                    log.Info("stopping thread DataCollection");
                    break;
                }
            }
        }
        */
        #endregion


        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.Debug(check.ThreadState.ToString());
            log.Debug(dataSync.ThreadState.ToString());

            AbortThreads();

            if (check.ThreadState == ThreadState.Running)
                check.Abort();

            if (dataSync.ThreadState == ThreadState.Running)
                dataSync.Abort();

            if (activityTracking.ThreadState == ThreadState.Running)
                activityTracking.Abort();

            log.Debug(check.ThreadState.ToString());
            log.Debug(dataSync.ThreadState.ToString());
            log.Debug(activityTracking.ThreadState.ToString());


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
                var myForm = new frmSettings();
                myForm.ShowDialog();
                myForm.Dispose();

                UpdateConfig();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
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
            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
                Visible = false;
                notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...",
                    ToolTipIcon.Info);
            }

            notifyIcon1.Visible = true;
        }

        public void ShowNotification(string message, ToolTipIcon icon)
        {
            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", message, icon);
        }

        private void SyncData()
        {
            log.Info("Initializing sync data method...");
            while (true)
            {
                try
                {
                    log.Info("syncData is being executed...");
                    if (myConfig.ContainsKey("TOKEN"))
                    {
                        if (!string.IsNullOrEmpty(myConfig["TOKEN"]))
                        {
                            var da = new DataAccess();

                            var token = Client.GetLoginToken(myConfig["USERNAME"], myConfig["PASSWORD"]);

                            var records = da.ProcessReportGenerator(myConfig["USERNAME"]);

                            log.Info("Submiting process request... sending " + records.ProcessesReport.Count +
                                     " records");
                            
                            var result = Client.SaveProcessReport(records, token); // myConfig["TOKEN"]);
                            log.Info("Updating activity status...");
                            da.UpdateProcessStatus(DataAccess.ActivityStatus.Processing,
                                result ? DataAccess.ActivityStatus.Accepted : DataAccess.ActivityStatus.Error);
                            da.CleanProcessDataHistory();
                            log.Info("Process finished...");


                            var Activityrecords = da.ReportGenerator(myConfig["USERNAME"]);
                            log.Info("Submiting activity request... sending " + Activityrecords.Activities.Count +
                                     " records");
                            result = Client.SaveReport(Activityrecords, token);
                            da.UpdateActivityStatus(DataAccess.ActivityStatus.Processing,
                                result ? DataAccess.ActivityStatus.Accepted : DataAccess.ActivityStatus.Error);
                            da.CleanDataHistory();
                        }
                        else
                        {
                            log.Error("Error in the service starting process, please check the user credentials...");
                            ShowNotification(
                                "There is an error sending the data collected, please check the user credentials...",
                                ToolTipIcon.Error);

                            var myForm = new frmSettings(true);
                            myForm.ShowDialog();
                            myForm.Dispose();

                            UpdateConfig();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                }

#if DEBUG
                Thread.Sleep(120000);
#else
                Thread.Sleep(SENDING_INTERVAL);
#endif
                if (abortDataSync)
                {
                    log.Info("stopping thread DataSync");
                    break;
                }
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
                log.Info(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                var dataCollectionTime = DateTime.Now;
                if (eventType == EVENT_SYSTEM_FOREGROUND || eventType == EVENT_OBJECT_SELECTION)
                {
                    var tmp = myCurrentActivity;

                    if (tmp != null)
                    {
                        log.Info("WinEventProc is being executed..., tracking app -> " + tmp.ExecutableName);
                        Console.WriteLine("WinEventProc is being executed..., tracking app -> " + tmp.ExecutableName);
                        var da = new DataAccess();
                        tmp.EndTime = dataCollectionTime;
                        da.SaveMyActivity(tmp);
                        log.Debug("End of app tracked -> " + tmp.ExecutableName + " - " +
                                  tmp.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + " to " +
                                  tmp.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    myCurrentActivity = myCollector.GetCurrentActivity(hwnd, dataCollectionTime);
                    
                    if(myCurrentActivity != null)
                        myCurrentActivity.StartTime = dataCollectionTime;
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                Console.WriteLine(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }

        private void keyboard_KeyBoardKeyPressed(object sender, EventArgs e)
        {
            last_keyboard_touch = DateTime.Now;
            FrmSystemInfo.idleTimeStart = last_keyboard_touch;
            if (myCurrentActivity != null)
            {
                myCurrentActivity.IdleActivity = false;
                logApp(myCurrentActivity, last_keyboard_touch, "Keyboard");
            }
        }

        private void mouse_MouseMoved(object sender, EventArgs e)
        {
            last_mouse_signal = DateTime.Now;
            FrmSystemInfo.idleTimeStart = last_mouse_signal;
            if (myCurrentActivity != null)
            {
                myCurrentActivity.IdleActivity = false;
                logApp(myCurrentActivity, last_mouse_signal, "Mouse");
            }
        }


        private void logApp(CollectorActivity app, DateTime caputuredTime, string source)
        {
            log.Info("Tracking apps for presence tracking, AppName:" + app.AppName + "|ExecutableFile:" +
                     app.ExecutableName + "|CaptureTime:" + caputuredTime.ToString("yyyy-MM-dd HH:mm:ss") + "|Source:" +
                     source);
        }

        //ON CURRENT WINDOW UPDATE ACTION
        //INITIALIZE LAST_KEYBOARD_TOUCH TO ZERO(NULL)
        private void simulation_method()
        {
            while (true)
            {
                if (last_keyboard_touch != null || last_mouse_signal != null)
                {
                    var maximum_date = MaxDate(last_mouse_signal, last_keyboard_touch);
                    var present = DateTime.Now;
                    var totalminutes = (present - maximum_date).TotalMinutes;
                    var isIdle = false;

                    var tmp = myCurrentActivity;
                    if (tmp != null)
                    {
                        if (tmp == null)
                            isIdle = true;
                        else
                            isIdle = tmp.IdleActivity;

                        if (totalminutes > 2 && !isIdle)
                            try
                            {
                                var da = new DataAccess();
                                tmp.EndTime = maximum_date;
                                log.Info(tmp.AppName + " became IDLE....");
                                //notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", tmp.AppName + " has become IDLE", ToolTipIcon.Warning);
                                da.SaveMyActivity(tmp);
                            }
                            catch (Exception ex)
                            {
                                log.Info(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                            }
                            finally
                            {
                                
                                myCurrentActivity.StartTime = present;
                                myCurrentActivity.IdleActivity = true;
                                myCurrentActivity.EndTime = new DateTime();
                            }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public DateTime MaxDate(DateTime first, DateTime second)
        {
            if (first > second) return first;
            return second;
        }

        private delegate void WinEventDelegate(IntPtr hWinEventHook,
            uint eventType, IntPtr hwnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime);

        private void cmenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}