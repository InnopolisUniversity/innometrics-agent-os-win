using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using InnoMetricsCollector.classes;

namespace InnoMetricsCollector
{
    public class ReportGenerator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CollectorReport getCurrentProcessReport()
        {
            log.Info("Generation process report...");
            CollectorReport myReport = new CollectorReport();

            IDictionary<IntPtr, Object[]> currentWindows = OpenWindowGetter.GetOpenWindows();
            IDictionary<String, Object> batteryStatus = OpenWindowGetter.GetBatteryStatus();

            // get IP address of host computer
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            // get mac address of host machine
            String firstMacAddress = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault();

            DateTime meausureTime = DateTime.Now;

            foreach (KeyValuePair<IntPtr, Object[]> w in currentWindows)
            {

                // 1 Process name
                // 2 Process ID
                // 3 Process status
                // 4 Description of the process
                // 5 exe name
                // 6 process starting time
                // 7 Total processor time
                // 7 USer processor time

                CollectorActivity myAppActivity = new CollectorActivity
                {
                    ActivityType = "OS",
                    ProcessId = w.Value[1].ToString(),
                    BrowserTitle = "",
                    BrowserUrl = "",
                    ExecutableName = w.Value[4].ToString(),
                    IdleActivity = (w.Value[2].ToString().ToUpper() == "IDLE"),
                    IpAddress = myIP,
                    MacAddress = firstMacAddress,
                    StartTime = DateTime.Parse(w.Value[5].ToString()),
                    EndTime = meausureTime,
                    UserName = "",
                    Description = w.Value[3].ToString(),
                    Status = "0"
                };

                //myAppActivity.Measurements = new List<MeasurementReport>();

                String ChangingStatus = batteryStatus["BatteryStatus"].ToString();
                float EstimatedChargeRemaining = float.Parse(batteryStatus["EstimatedChargeRemaining"].ToString());

                EstimatedChargeRemaining = EstimatedChargeRemaining > 100.0 ? 100 : EstimatedChargeRemaining;

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "EstimatedChargeRemaining",
                    Value = ChangingStatus== "2" ? "-1" : EstimatedChargeRemaining.ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "BatteryStatus",
                    Value = batteryStatus["BatteryStatus"].ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "RAM",
                    Value = w.Value[9].ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "vRAM",
                    Value = w.Value[10].ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "CPU",
                    Value = w.Value[11].ToString()
                });

                myReport.activities.Add(myAppActivity);
            }

            return myReport;
        }
    }
}
