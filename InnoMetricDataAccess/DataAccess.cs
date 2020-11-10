using InnoMetric.Models;
using InnoMetricsCollector;
using InnoMetricsCollector.classes;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnoMetricDataAccess
{
    public class DataAccess
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //const string regKeyFolders = @"HKEY_USERS\<SID>\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
        //const string regValueAppData = @"AppData";
        const String dbDirectory = @"C:\TMP\InnoMetrics\db\";

#if DEBUG
        const Int16 LIMIT_QUERY = 10;
#else
        const Int16 LIMIT_QUERY = 500;
#endif

        public enum ActivityStatus
        {
            Collected = 0,
            Processing = 1,
            Accepted = 2,
            Error = -1
        };

        public void CheckDB()
        {
            var sourceFile = AppDomain.CurrentDomain.BaseDirectory + @"CollectorDB.db";
            var destinationFile = dbDirectory + @"CollectorDB.db";

            if (!System.IO.File.Exists(destinationFile))
            {
                try
                {
                    var exists = System.IO.Directory.Exists(dbDirectory);

                    if (!exists)
                        System.IO.Directory.CreateDirectory(dbDirectory);

                    File.Copy(sourceFile, destinationFile, true);
                }
                catch (IOException iox)
                {
                    MessageBox.Show(iox.ToString());
                }
            }
            else
            {
                var myConfig = LoadInitialConfig();
                var myVersion = ConfigurationManager.AppSettings["db_version"];
                //MessageBox.Show("myVersion -> " + myVersion + ". current version: " + myConfig["VERSION"].ToString());
                if (myVersion.ToString() != myConfig["VERSION"].ToString())
                {
                    try
                    {
                        //File.Delete(destinationFile);
                        File.Copy(sourceFile, destinationFile, true);
                    }
                    catch (IOException iox)
                    {
                        MessageBox.Show(iox.ToString());
                    }

                }
            }
        }

        private string LoadConnectionString()
        {
            return Configs.GetConnectionString();
        }

        public Dictionary<string, string> LoadInitialConfig()
        {
            var myConfig = new Dictionary<string, string>();
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                log.Debug(cnn.ConnectionString);

                var ds = new DataSet();
                var da = new SQLiteDataAdapter("select LABEL, VALUE from configs", cnn);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    myConfig.Add(r[0].ToString(), r[1].ToString());
                }
                cnn.Close();
            }
            return myConfig;

        }

        public bool SaveMyConfig(Dictionary<string, string> myConfig)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    cnn.Open();
                    var cmd = new SQLiteCommand(cnn)
                    {
                        CommandText = @"Update configs set VALUE = @Value where LABEL = @Label"
                    };
                    foreach (var pair in myConfig)
                    {
                        cmd.Prepare();
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Value", pair.Value);
                        cmd.Parameters.AddWithValue("@Label", pair.Key);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show($"{e.Message}, {e.StackTrace}, {e.Source}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return true;
            }


        }

        public void SaveMyActivity(CollectorActivity activity)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();

                    activity.ActivityID = GetNextActivityId(cnn);

                    var cmd = new SQLiteCommand(cnn)
                    {
                        CommandText = "insert into CollectorData (ActivityId, ProcessName, ProcessId, StartTime, " +
                        "EndTime, IPAddress, MacAddress, Description, PID, Status, ServerStatus) values (@ActivityId, @ProcessName, " +
                        "@ProcessId, @StartTime, @EndTime, @IPAddress, @MacAddress, @Description, @PID, @Status, @serverStatus)"
                    };
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@ActivityId", activity.ActivityID);
                    cmd.Parameters.AddWithValue("@ProcessName", activity.AppName);
                    cmd.Parameters.AddWithValue("@ProcessId", activity.ProcessId);
                    cmd.Parameters.AddWithValue("@StartTime", activity.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EndTime", activity.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@IPAddress", activity.IpAddress);
                    cmd.Parameters.AddWithValue("@MacAddress", activity.MacAddress);
                    cmd.Parameters.AddWithValue("@Description", activity.Description);
                    cmd.Parameters.AddWithValue("@PID", activity.ProcessId);
                    cmd.Parameters.AddWithValue("@Status", (activity.IdleActivity ? "I" : "A"));
                    cmd.Parameters.AddWithValue("@serverStatus", ActivityStatus.Collected);

                    cmd.ExecuteNonQuery();


                    foreach (var m in activity.Measurements)
                    {
                        SaveMyMeasurement(m, cnn, activity.ActivityID.ToString());
                    }

                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    //Console.WriteLine(e.Message);
                    //MessageBox.Show(e.Message + ", " + e.StackTrace + ", " + e.Source, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveMyMeasurement(Metrics metric, SQLiteConnection cnn, string ActivityId)
        {
            try
            {
                if (cnn.State != ConnectionState.Open)
                    cnn.Open();

                var cmd = new SQLiteCommand(cnn)
                {
                    CommandText = @"insert into CollectorMetrics (ActivityId, MetricTypeId, Value) 
                                    values (@ActivityId, @MetricTypeId, @Value)"
                };
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@ActivityId", ActivityId);
                cmd.Parameters.AddWithValue("@MetricTypeId", metric.MeasurementType);
                cmd.Parameters.AddWithValue("@Value", metric.Value);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
               // MessageBox.Show(e.Message + ", " + e.StackTrace + ", " + e.Source, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error(e.Message);
            }
        }

        private int GetNextActivityId(SQLiteConnection cnn)
        {
            if (cnn.State != ConnectionState.Open)
                cnn.Open();

            var ds = new DataSet();
            var da = new SQLiteDataAdapter("select ifnull(max(CAST(activityid as decimal)) + 1, 1) from CollectorData", cnn);
            da.Fill(ds);

            var myId = "";

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                myId = r[0].ToString();
            }

            return int.Parse(myId);

        }

        private int GetNextProcessId(SQLiteConnection cnn)
        {
            if (cnn.State != ConnectionState.Open)
                cnn.Open();


            var ds = new DataSet();
            var da = new SQLiteDataAdapter("select ifnull(max(CAST(ProcessId as decimal)) + 1, 1) from CollectorProcessData", cnn);
            var cmd = new SQLiteCommandBuilder(da);
            da.Fill(ds);

            var myId = "";

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                myId = r[0].ToString();
            }

            return int.Parse(myId);

        }

        private bool ExistProcessById(SQLiteConnection cnn, string ProcessId)
        {
            if (cnn.State != ConnectionState.Open)
                cnn.Open();

            var cmd = new SQLiteCommand(cnn)
            {
                CommandText = @"select count(*) from CollectorProcessData where ProcessId = @ProcessId"
            };


            cmd.Prepare();

            cmd.Parameters.AddWithValue("@ProcessId", ProcessId);

            var myId = cmd.ExecuteScalar().ToString();



            var ds = new DataSet();
            var da = new SQLiteDataAdapter("select * from CollectorProcessData  where ProcessId = @ProcessId", cnn);
            da.SelectCommand.Parameters.AddWithValue("@ProcessId", ProcessId);
            da.Fill(ds);

            myId = "";

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                myId = r[0].ToString();
            }


            return ds.Tables[0].Rows.Count > 0;

        }

        public CollectorProcess SaveMyProcess(CollectorProcess process)
        {
            log.Debug("Executing SaveMyProcess...");
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var recordExist = ExistProcessById(cnn, process.ProcessID.ToString());
                    log.Debug(process.ProcessID + ", Process name: " + process.ProcessName);

                    //if (process.ProcessID == -1 || !recordExist)
                    //{
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();

                    //if (process.ProcessID == -1)
                    process.ProcessID = GetNextProcessId(cnn);

                    var cmd = new SQLiteCommand(cnn)
                    {
                        CommandText = @"insert into CollectorProcessData (ProcessId, ProcessType, ProcessName, 
                                        WindowsTitle, BrowserURL, IPAddress, MacAddress, PID, CollectedTime, ServerStatus) 
                                        values (@ProcessId, @ProcessType, @ProcessName, @WindowsTitle, 
                                        @BrowserURL, @IPAddress, @MacAddress, @PID, @CollectedTime, @serverStatus)"
                    };
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@ProcessId", process.ProcessID);
                    cmd.Parameters.AddWithValue("@ProcessType", process.ProcessType);
                    cmd.Parameters.AddWithValue("@ProcessName", process.ProcessName);
                    cmd.Parameters.AddWithValue("@WindowsTitle", process.WindowsTitle);
                    cmd.Parameters.AddWithValue("@BrowserURL", process.BrowserUrl);
                    cmd.Parameters.AddWithValue("@IPAddress", process.IpAddress);
                    cmd.Parameters.AddWithValue("@MacAddress", process.MacAddress);
                    cmd.Parameters.AddWithValue("@PID", process.PID);
                    cmd.Parameters.AddWithValue("@CollectedTime", process.collectedTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@serverStatus", ActivityStatus.Collected);

                    cmd.ExecuteNonQuery();
                    //}

                    ProcessMetrics batteryMeuasure = null;// new ProcessMetrics();

                    foreach (var m in process.Measurements)
                    {
                        SaveProcessMeasurement(m, cnn, process);
                        if (m.MeasurementType == "1") batteryMeuasure = m;
                    }
                    process.Measurements.Clear();

                    if (batteryMeuasure != null) process.Measurements.Add(batteryMeuasure);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    log.Error(e.Message);
                    //MessageBox.Show(e.Message + ", " + e.StackTrace + ", " + e.Source, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return process;
            }
        }

        private void SaveProcessMeasurement(ProcessMetrics metric, SQLiteConnection cnn, CollectorProcess Process)
        {
            try
            {
                if (cnn.State != ConnectionState.Open)
                    cnn.Open();

                var cmd = new SQLiteCommand(cnn)
                {
                    CommandText = @"insert into CollectorProcessMetrics (ProcessID, MetricTypeId, Value, collectedTime, IPAddress, MacAddress) 
                                    values (@ProcessID, @MetricTypeId, @Value, @CollectedTime, @IPAddress, @MacAddress)"
                };
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@ProcessID", Process.ProcessID);
                cmd.Parameters.AddWithValue("@MetricTypeId", metric.MeasurementType);
                cmd.Parameters.AddWithValue("@Value", metric.Value);
                cmd.Parameters.AddWithValue("@CollectedTime", metric.CapturedTime);
                cmd.Parameters.AddWithValue("@IPAddress", Process.IpAddress);
                cmd.Parameters.AddWithValue("@MacAddress", Process.MacAddress);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //MessageBox.Show(e.Message + ", " + e.StackTrace + ", " + e.Source, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error(e.Message);
            }
        }



        public List<String> LoadProcessHistory()
        {
            var myHistory = new List<string>();
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var ds = new DataSet();
                var da = new SQLiteDataAdapter("select * from CollectorProcessData where ServerStatus = '0'", cnn);
                var cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var activity = "";
                    foreach (var c in r.ItemArray)
                    {
                        activity += c.ToString() + "; ";
                    }

                    activity += "  Metrics -> {";

                    var metrics = LoadMetricsHistory(activity.Split(';')[0]);

                    foreach (var m in metrics)
                    {
                        activity += m + " - ";
                    }

                    activity += "}";
                    myHistory.Add(activity);
                }
            }
            return myHistory;
        }

        public List<String> LoadMetricsHistory(String ActivityId)
        {
            var myHistory = new List<string>();
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var ds = new DataSet();
                var da = new SQLiteDataAdapter("select MetricTypeId, Value from CollectorProcessMetrics where ProcessID = @ProcessID", cnn);
                da.SelectCommand.Parameters.AddWithValue("@ProcessID", ActivityId);
                var cmd = new SQLiteCommandBuilder(da);

                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var metric = "{";
                    foreach (var c in r.ItemArray)
                    {
                        metric += c.ToString() + "; ";
                    }

                    metric += "}";

                    myHistory.Add(metric);
                }
            }
            return myHistory;
        }

        public Report ReportGenerator(String account)
        {
            var myReport = new Report
            {
                Activities = new List<ActivityReport>()
            };

            var generator = new ReportGenerator();
            var OSVERION = generator.GetOSVersion();
            UpdateActivityStatus(ActivityStatus.Collected, ActivityStatus.Processing);//Updating server Status from 0 -> new to 1 -> Processing

            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var ds = new DataSet();
                var da = new SQLiteDataAdapter(@"select ActivityId, 
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
                                                                      ServerStatus,
                                                                      PID
                                                                 from CollectorData
                                                                where ServerStatus = '1'", cnn);
                var cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var ActivityId = r.ItemArray[0].ToString();

                    var activity = new ActivityReport
                    {
                        ExecutableName = r.ItemArray[1].ToString(),
                        ActivityType = r.ItemArray[2].ToString(),
                        BrowserTitle = r.ItemArray[10].ToString(),
                        BrowserUrl = r.ItemArray[4].ToString(),
                        StartTime = DateTime.Parse(r.ItemArray[5].ToString()),
                        EndTime = DateTime.Parse(r.ItemArray[6].ToString()),
                        IdleActivity = r.ItemArray[11].ToString() != "A",//r.ItemArray[7].ToString(),
                        IpAddress = r.ItemArray[8].ToString(),
                        MacAddress = r.ItemArray[9].ToString(),
                        Pid = r.ItemArray[13].ToString(),
                        Osversion = OSVERION,
                        UserID = account//"x.vasquez"
                    };

                    activity = LoadReportMetrics(ActivityId, activity);

                    myReport.Activities.Add(activity);

                }
            }
            return myReport;
        }




        private ActivityReport LoadReportMetrics(String ActivityId, ActivityReport myActivity)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var ds = new DataSet();
                var da = new SQLiteDataAdapter("select MetricTypeId, Value from CollectorMetrics where ActivityId = @ActivityId", cnn);
                da.SelectCommand.Parameters.AddWithValue("@ActivityId", ActivityId);
                var cmd = new SQLiteCommandBuilder(da);

                da.Fill(ds);
                //myActivity.Measurements = new List<MeasurementReport>();
                /*
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    MeasurementReport metric = new MeasurementReport
                    {
                        MeasurementTypeId = r.ItemArray[0].ToString(),
                        Value = r.ItemArray[1].ToString(),
                        AlternativeLabel = ""
                    };

                    //myActivity.Measurements.Add(metric);
                }*/
            }
            return myActivity;
        }

        public void UpdateActivityStatus(ActivityStatus oldStatus, ActivityStatus newStatus, Boolean setLimit = false)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                var cmd = new SQLiteCommand(cnn)
                {
                    CommandText = @"Update CollectorData 
                                       set ServerStatus = @newStatus 
                                     where ServerStatus = @oldStatus"
                };

                if (setLimit)
                {
                    cmd.CommandText += @" and activityId in (SELECT activityId
                                                               FROM CollectorData
                                                              ORDER BY StartTime ASC
                                                              LIMIT "+ LIMIT_QUERY + ")";
                }
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@oldStatus", oldStatus);
                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                cmd.ExecuteNonQuery();
            }
        }

        public AddProcessReportRequest ProcessReportGenerator(String account)
        {
            var myReport = new AddProcessReportRequest
            {
                ProcessesReport = new List<ProcessReport>()
            };
            var generator = new ReportGenerator();
            var OSVERSION = generator.GetOSVersion();
            UpdateProcessStatus(ActivityStatus.Collected, ActivityStatus.Processing, true);//Updating server Status from 0 -> new to 1 -> Processing

            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var ds = new DataSet();

                var da = new SQLiteDataAdapter(@"select ProcessId, 
                                                                      ProcessType, 
                                                                      ProcessName, 
                                                                      WindowsTitle,
                                                                      BrowserURL,
                                                                      IPAddress, 
                                                                      MacAddress,
                                                                      ServerStatus,
                                                                      PID, 
                                                                      CollectedTime
                                                                 from CollectorProcessData
                                                                where ServerStatus = '1'", cnn);
                var cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var ProcessId = r.ItemArray[0].ToString();

                    var process = new ProcessReport
                    {
                        ProcessName = r.ItemArray[2].ToString(),
                        IpAddress = r.ItemArray[5].ToString(),
                        MacAddress = r.ItemArray[6].ToString(),
                        CollectedTime = DateTime.Parse(r.ItemArray[9].ToString()),
                        Osversion = OSVERSION,
                        Pid = r.ItemArray[8].ToString(),
                        UserID = account
                    };

                    process = LoadProcessReportMetrics(ProcessId, process);

                    myReport.ProcessesReport.Add(process);

                }
            }
            return myReport;
        }

        private ProcessReport LoadProcessReportMetrics(String ProcessId, ProcessReport myProcess)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var ds = new DataSet();
                var da = new SQLiteDataAdapter("select MetricTypeId, Value, CollectedTime from CollectorProcessMetrics where ProcessId = @ProcessId", cnn);
                da.SelectCommand.Parameters.AddWithValue("@ProcessId", ProcessId);
                var cmd = new SQLiteCommandBuilder(da);

                da.Fill(ds);
                myProcess.MeasurementReportList = new List<MeasurementReport>();

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var metric = new MeasurementReport
                    {
                        MeasurementTypeId = r.ItemArray[0].ToString(),
                        Value = r.ItemArray[1].ToString(),
                        AlternativeLabel = "",
                        CapturedDate = DateTime.Parse(r.ItemArray[2].ToString()),
                    };

                    myProcess.MeasurementReportList.Add(metric);
                }
            }
            return myProcess;
        }

        public void UpdateProcessStatus(ActivityStatus oldStatus, ActivityStatus newStatus, Boolean setLimit = false)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                var cmd = new SQLiteCommand(cnn)
                {
                    CommandText = @"Update CollectorProcessData 
                                       set ServerStatus = @newStatus 
                                     where ServerStatus = @oldStatus"
                };
                if (setLimit)
                {
                    cmd.CommandText += @" and processID in (SELECT processID
                                                              FROM CollectorProcessData
                                                             ORDER BY CollectedTime ASC
                                                             LIMIT " + LIMIT_QUERY + ")";
                }
                
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@oldStatus", oldStatus);
                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                cmd.ExecuteNonQuery();
            }
        }


        public void CleanDataHistory()
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                var cmd = new SQLiteCommand(cnn)
                {
                    CommandText = @"delete
                                    from collectorMetrics 
                                    where activityId in (
	                                    select activityID 
	                                    from CollectorData 
	                                    where serverStatus = @Status)"
                };
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Status", ActivityStatus.Accepted);
                cmd.ExecuteNonQuery();

                //cmd = new SQLiteCommand(cnn);
                cmd.CommandText = @"Delete from CollectorData 
                                     where ServerStatus = @Status";
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Status", ActivityStatus.Accepted);
                cmd.ExecuteNonQuery();
            }
        }


        public void CleanProcessDataHistory()
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                var cmd = new SQLiteCommand(cnn)
                {
                    CommandText = @"delete
                                    from collectorProcessMetrics 
                                    where processID in (
	                                    select processID 
	                                    from CollectorProcessData 
	                                    where serverStatus = @Status)"
                };
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Status", ActivityStatus.Accepted);
                cmd.ExecuteNonQuery();

                //cmd = new SQLiteCommand(cnn);
                cmd.CommandText = @"Delete from CollectorProcessData 
                                     where ServerStatus = @Status";
                cmd.Prepare();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Status", ActivityStatus.Accepted);
                cmd.ExecuteNonQuery();
            }
        }


        public List<String> LoadGetTopIdleApps()
        {
            var TopApps = new List<String>();
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {

                log.Debug(cnn.ConnectionString);

                var ds = new DataSet();
                var da = new SQLiteDataAdapter(@"select ProcessName
                                                                 from collectorData
                                                                where date(StartTime) = Date('now')
                                                             group by ProcessName
                                                             order by sum(((JulianDay(EndTime)- JulianDay(StartTime)) * 24 * 60)) asc
                                                                limit 3", cnn);

                var cmd = new SQLiteCommandBuilder(da);
                da.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    TopApps.Add(r[0].ToString());
                }
            }
            return TopApps;

        }
    }
}
