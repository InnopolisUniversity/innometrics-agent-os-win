using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using APIClient;
//using InnoMetric.Models;
using log4net;

namespace InnoMetricsDataSync
{
    public partial class InnoMetricsDataSync : ServiceBase
    {
        public static Dictionary<string, string> myConfig;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Timer timer = new Timer();

        public InnoMetricsDataSync()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log.Info("Starting service...");
                myConfig = null; //loadInitialConfig();
                timer.Elapsed += OnElapsedTime;
                timer.Interval = 5 * 60 * 1000; //number in milisecinds (minutes * seconds * miliseconds)
                timer.Enabled = true;

                log.Info("Loading the main window");
                log.Info("Loading the initial configuration");


                if (!string.IsNullOrEmpty(myConfig["USERNAME"]) && !string.IsNullOrEmpty(myConfig["PASSWORD"]))
                {
                    log.Info("There is a user registered");
                    log.Info("try to login automatically with username and password");
                    myConfig["TOKEN"] = Client.GetLoginToken(myConfig["USERNAME"], myConfig["PASSWORD"]);
                    log.Debug("token -> " + myConfig["TOKEN"]);
                }
                else
                {
                    log.Error("Error in the service starting process, please check the user credentials...");
                    Stop();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in the service starting process, " + ex.Message + ", " + ex.StackTrace + ", " +
                          ex.Source);
                Stop();
            }
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            log.Info("Sending data process started...");
            syncData();
            log.Info("Sending data process finished...");
        }

        protected override void OnStop()
        {
            log.Info("The service was stopped...");
        }

        private void syncData()
        {
            if (myConfig.ContainsKey("TOKEN"))
            {
                if (myConfig["TOKEN"] != null)
                {
                    APIClient.InnoMetricClient.Models.Report records = null; //reportGenerator();
                    log.Info("Submiting request...");
                    var result = Client.SaveReport(records, myConfig["TOKEN"]);
                    log.Info("Updating activity status...");
                    //updateActivityStatus(ActivityStatus.Processing, result ? ActivityStatus.Accepted : ActivityStatus.Error);
                    log.Info("Process finished...");
                }
                else
                {
                    log.Error("Error in the service starting process, please check the user credentials...");
                }
            }
        }
    }
}