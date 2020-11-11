using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using InnoMetricsCollector;
using InnoMetricsCollector.classes;
using InnoMetricsDataCollector.classes;

namespace InnoMetricsDataCollector
{
    public partial class SrvDataCollector : ServiceBase
    {
        Timer timer = new Timer();
        private ReportGenerator myCollector;

        CollectorReport myLastReport;
        //CollectorReport myClosedActivities;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Dictionary<String, String> myConfig;


        public SrvDataCollector()
        {
            InitializeComponent();
            
        }

        protected override void OnStart(string[] args)
        {
            //WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 10000; //number in milisecinds  
            timer.Enabled = true;
            log.Info("Service started...");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //WriteToFile("Service is recall at " + DateTime.Now);
            log.Info("Collecting data process started...");
            loadReport();
            log.Info("Collecting data process finished...");
        }


        protected override void OnStop()
        {
            //WriteToFile("Service is stopped at " + DateTime.Now);
            loadReport();

            foreach (var app in myLastReport.activities)
            {
                DataAccess.SaveMyActivity(app);
            }

            log.Info("Collecting data process stopped...");
        }


        private void loadReport()
        {
            myCollector = new ReportGenerator();
            CollectorReport myReport = myCollector.getCurrentProcessReport();
            log.Info("Currect active activities -> " + myReport.activities.Count);
            if (myLastReport != null)
            {
                var result = myLastReport.activities.Where(p => !myReport.activities.Any(p2 => p2.ProcessId == p.ProcessId && p2.ExecutableName == p.ExecutableName));

                foreach (var app in result)
                {
                    log.Info("Saving activity data...");
                    DataAccess.SaveMyActivity(app);
                }

                foreach (var app in myReport.activities)
                {
                    log.Info("App -> " + app.ExecutableName + ", " + app.Description);
                    var lastState = myLastReport.activities.Where(p => app.ProcessId == p.ProcessId && app.ExecutableName == p.ExecutableName).FirstOrDefault();
                    if (lastState != null)
                    {
                        var myLastMeasure = lastState.Measurements.FirstOrDefault(m => m.MeasurementType == "EstimatedChargeRemaining");
                        var myCurrentMeasure = app.Measurements.FirstOrDefault(m => m.MeasurementType == "EstimatedChargeRemaining");

                        var BatteryConsumptionLast = lastState.Measurements.FirstOrDefault(m => m.MeasurementType == "BatteryConsumption");

                        if (BatteryConsumptionLast != null)
                        {
                            app.Measurements.Add(new Metrics
                            {
                                MeasurementType = "BatteryConsumption",
                                Value = BatteryConsumptionLast.Value
                            });
                        }

                        if (myCurrentMeasure.Value != "-1")
                        {
                            if (Double.Parse(myLastMeasure.Value) > Double.Parse(myCurrentMeasure.Value))
                            {

                                var BatteryConsumption = app.Measurements.FirstOrDefault(m => m.MeasurementType == "BatteryConsumption");

                                if (BatteryConsumption != null)
                                {
                                    BatteryConsumption.Value = (Double.Parse(BatteryConsumption.Value) + (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value))).ToString();
                                }
                                else
                                {
                                    app.Measurements.Add(new Metrics
                                    {
                                        MeasurementType = "BatteryConsumption",
                                        Value = (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value)).ToString()
                                    });
                                }
                            }
                        }
                    }
                }
            }

            myLastReport = myReport;
        }
    }
}
