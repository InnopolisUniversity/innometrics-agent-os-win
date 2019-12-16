using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using InnoMetricsCollector.classes;

namespace InnoMetricsDataCollector.classes
{
    class DataAccess
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static Dictionary<String, String> loadInitialConfig()
        {
            Dictionary<String, String> myConfig = new Dictionary<String, String>();
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try {
                    DataSet ds = new DataSet();
                    SQLiteDataAdapter da = new SQLiteDataAdapter("select LABEL, VALUE from configs", cnn);
                    SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(da);
                    da.Fill(ds);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        myConfig.Add(r[0].ToString(), r[1].ToString());
                    }
                }
                catch(Exception ex)
                {
                    log.Error("Error process data -> " + ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                }
                
            }
            return myConfig;
        }

        public static Boolean saveMyConfig(Dictionary<String, String> myconfig)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                try
                {
                    cnn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(cnn);
                    cmd.CommandText = @"Update configs set VALUE = @Value where LABEL = @Label";
                    foreach (String k in myconfig.Keys)
                    {
                        cmd.Prepare();
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Value", myconfig[k].ToString());
                        cmd.Parameters.AddWithValue("@Label", k);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    log.Error("Error process data -> " + ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                }
                return false;
            }
        }


        public static void SaveMyActivity(CollectorActivity activity)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();

                    log.Info("Saving activity data");

                    activity.ActivityID = getNextActivityId(cnn);

                    var cmd = new SQLiteCommand(cnn);
                    cmd.CommandText = "insert into CollectorData (ActivityId, ProcessName, ProcessId, StartTime, " +
                        "EndTime, IPAddress, MacAddress, Description, Status, ServerStatus) values (@ActivityId, @ProcessName, " +
                        "@ProcessId, @StartTime, @EndTime, @IPAddress, @MacAddress, @Description, @Status, @serverStatus)";
                    cmd.Prepare();
                    log.Info("Preparing command");
                    cmd.Parameters.AddWithValue("@ActivityId", activity.ActivityID);
                    cmd.Parameters.AddWithValue("@ProcessName", activity.ExecutableName);
                    cmd.Parameters.AddWithValue("@ProcessId", activity.ProcessId);
                    cmd.Parameters.AddWithValue("@StartTime", activity.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", activity.EndTime);
                    cmd.Parameters.AddWithValue("@IPAddress", activity.IpAddress);
                    cmd.Parameters.AddWithValue("@MacAddress", activity.MacAddress);
                    cmd.Parameters.AddWithValue("@Description", activity.Description);
                    cmd.Parameters.AddWithValue("@Status", activity.Status);
                    cmd.Parameters.AddWithValue("@serverStatus", "0");
                    log.Info("Executing...");
                    cmd.ExecuteNonQuery();


                    foreach (var m in activity.Measurements)
                    {
                        log.Info("Saving measurements");
                        SaveMyMeasurement(m, cnn, activity.ActivityID.ToString());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    log.Info("Error process data -> " + ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                }
            }
        }


        private static void SaveMyMeasurement(Metrics metric, SQLiteConnection cnn, string ActivityId)
        {
            try
            {
                if (cnn.State != ConnectionState.Open)
                    cnn.Open();

                var cmd = new SQLiteCommand(cnn);
                cmd.CommandText = @"insert into CollectorMetrics (ActivityId, MetricTypeId, Value) 
                                    values (@ActivityId, @MetricTypeId, @Value)";
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@ActivityId", ActivityId);
                cmd.Parameters.AddWithValue("@MetricTypeId", metric.MeasurementType);
                cmd.Parameters.AddWithValue("@Value", metric.Value);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                log.Info("Error process data -> " + ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
        }

        private static int getNextActivityId(SQLiteConnection cnn)
        {
            var myId = "0";

            try
            {
                if (cnn.State != ConnectionState.Open)
                    cnn.Open();

                Dictionary<String, String> myConfig = new Dictionary<String, String>();


                //cnn.Open();
                SQLiteCommand cmd = new SQLiteCommand(cnn);
                cmd.CommandText = @"select count(*) + 1 from CollectorData";

                myId = cmd.ExecuteScalar().ToString();

                
            }
            catch (Exception ex)
            {
                log.Error("Error process data -> " + ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }

            return int.Parse(myId);
        }

        /*
        public static Report reportGenerator()
        {
            Report myReport = new Report();

            myReport.Activities = new List<ActivityReport>();

            updateActivityStatus("0", "1");//Updating server Status from 0 -> new to 1 -> Processing

            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                DataSet ds = new DataSet();
                SQLiteDataAdapter da = new SQLiteDataAdapter(@"select ActivityId, 
                                                                      ProcessName, 
                                                                      'OS' ActivityType, 
                                                                      '' BrowserTitle,
                                                                      '' BrowserUrl,
                                                                      StartTime, 
                                                                      EndTime, 
                                                                      '' IdleActivity,
                                                                      IPAddress, 
                                                                      MacAddress, 
                                                                      Description, 
                                                                      Status, 
                                                                      ServerStatus 
                                                                 from CollectorData
                                                                where ServerStatus = '1'", cnn);
                SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    String ActivityId = r.ItemArray[0].ToString();

                    ActivityReport activity = new ActivityReport
                    {
                        ExecutableName = r.ItemArray[1].ToString(),
                        ActivityType = r.ItemArray[2].ToString(),
                        BrowserTitle = r.ItemArray[3].ToString(),
                        BrowserUrl = r.ItemArray[4].ToString(),
                        StartTime = DateTime.Parse(r.ItemArray[5].ToString()),
                        EndTime = DateTime.Parse(r.ItemArray[6].ToString()),
                        IdleActivity = false,//r.ItemArray[7].ToString(),
                        IpAddress = r.ItemArray[8].ToString(),
                        MacAddress = r.ItemArray[9].ToString(),
                        UserID = "x.vasquez"
                    };

                    activity = loadReportMetrics(ActivityId, activity);

                    myReport.Activities.Add(activity);

                }
            }
            return myReport;
        }


        private static ActivityReport loadReportMetrics(String ActivityId, ActivityReport myActivity)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                DataSet ds = new DataSet();
                SQLiteDataAdapter da = new SQLiteDataAdapter("select MetricTypeId, Value from CollectorMetrics where ActivityId = @ActivityId", cnn);
                da.SelectCommand.Parameters.AddWithValue("@ActivityId", ActivityId);
                SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(da);

                da.Fill(ds);
                myActivity.Measurements = new List<MeasurementReport>();

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    MeasurementReport metric = new MeasurementReport
                    {
                        MeasurementTypeId = r.ItemArray[0].ToString(),
                        Value = r.ItemArray[1].ToString(),
                        AlternativeLabel = ""
                    };

                    myActivity.Measurements.Add(metric);
                }
            }
            return myActivity;
        }

        private static void updateActivityStatus(String oldStatus, String newStatus)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand cmd = new SQLiteCommand(cnn);
                cmd.CommandText = @"Update CollectorData 
                                       set ServerStatus = @newStatus 
                                     where ServerStatus = @oldStatus";
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@oldStatus", oldStatus);
                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                cmd.ExecuteNonQuery();
            }
        }
        */
    }
}
