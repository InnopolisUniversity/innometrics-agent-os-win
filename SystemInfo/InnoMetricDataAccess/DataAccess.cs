using InnoMetric.Models;
using InnoMetricsCollector.classes;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnoMetricDataAccess
{
    public class DataAccess
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const string regKeyFolders = @"HKEY_USERS\<SID>\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
        const string regValueAppData = @"AppData";

        public enum ActivityStatus
        {
            Collected = 0,
            Processing = 1,
            Accepted = 2,
            Error = -1
        };

        private static string LoadConnectionString(string id = "Default")
        {
            //return Configs.getConnectionString().Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)); // ConfigurationManager.ConnectionStrings[id].ConnectionString;
            return Configs.getConnectionString().Replace("%AppData%", Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Temp"));
            
        }

        public static Dictionary<String, String> loadInitialConfig()
        {
            Dictionary<String, String> myConfig = new Dictionary<String, String>();
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                log.Debug(cnn.ConnectionString);

                DataSet ds = new DataSet();
                SQLiteDataAdapter da = new SQLiteDataAdapter("select LABEL, VALUE from configs", cnn);
                SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    myConfig.Add(r[0].ToString(), r[1].ToString());
                }
            }
            return myConfig;

        }

        public static Boolean saveMyConfig(Dictionary<String, String> myconfig)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
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
        }

        public static void SaveMyActivity(CollectorActivity activity)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();

                    activity.ActivityID = getNextActivityId(cnn);

                    var cmd = new SQLiteCommand(cnn);
                    cmd.CommandText = "insert into CollectorData (ActivityId, ProcessName, ProcessId, StartTime, " +
                        "EndTime, IPAddress, MacAddress, Description, Status, ServerStatus) values (@ActivityId, @ProcessName, " +
                        "@ProcessId, @StartTime, @EndTime, @IPAddress, @MacAddress, @Description, @Status, @serverStatus)";
                    cmd.Prepare();

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

                    cmd.ExecuteNonQuery();


                    foreach (var m in activity.Measurements)
                    {
                        SaveMyMeasurement(m, cnn, activity.ActivityID.ToString());
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    MessageBox.Show(e.Message + ", " + e.StackTrace + ", " + e.Source, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static int getNextActivityId(SQLiteConnection cnn)
        {

            if (cnn.State != ConnectionState.Open)
                cnn.Open();

            Dictionary<String, String> myConfig = new Dictionary<String, String>();


            //cnn.Open();
            SQLiteCommand cmd = new SQLiteCommand(cnn);
            cmd.CommandText = @"select count(*) + 1 from CollectorData";

            var myId = cmd.ExecuteScalar().ToString();

            return int.Parse(myId);

        }

        public static List<String> loadProcessHistory()
        {
            List<String> myHistory = new List<string>();
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                DataSet ds = new DataSet();
                SQLiteDataAdapter da = new SQLiteDataAdapter("select * from CollectorData where ServerStatus = '0'", cnn);
                SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    string activity = "";
                    foreach (object c in r.ItemArray)
                    {
                        activity += c.ToString() + "; ";
                    }

                    activity += "  Metrics -> {";

                    List<String> metrics = loadMetricsHistory(activity.Split(';')[0]);

                    foreach (String m in metrics)
                    {
                        activity += m + " - ";
                    }

                    activity += "}";
                    myHistory.Add(activity);
                }
            }
            return myHistory;
        }

        public static List<String> loadMetricsHistory(String ActivityId)
        {
            List<String> myHistory = new List<string>();
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                DataSet ds = new DataSet();
                SQLiteDataAdapter da = new SQLiteDataAdapter("select MetricTypeId, Value from CollectorMetrics where ActivityId = @ActivityId", cnn);
                da.SelectCommand.Parameters.AddWithValue("@ActivityId", ActivityId);
                SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(da);

                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    string metric = "{";
                    foreach (object c in r.ItemArray)
                    {
                        metric += c.ToString() + "; ";
                    }

                    metric += "}";

                    myHistory.Add(metric);
                }
            }
            return myHistory;
        }

        public static Report reportGenerator()
        {
            Report myReport = new Report();

            myReport.Activities = new List<ActivityReport>();

            updateActivityStatus(ActivityStatus.Collected, ActivityStatus.Processing);//Updating server Status from 0 -> new to 1 -> Processing

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

        public static void updateActivityStatus(ActivityStatus oldStatus, ActivityStatus newStatus)
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
    }
}
