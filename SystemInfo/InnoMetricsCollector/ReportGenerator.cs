using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using InnoMetricsCollector.classes;
using Microsoft.VisualBasic;

namespace InnoMetricsCollector
{
    public class ReportGenerator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CollectorReport getCurrentProcessReport()
        {
            log.Info("Generation process report...");
            CollectorReport myReport = new CollectorReport();

            //IDictionary<IntPtr, Object[]> currentWindows = OpenWindowGetter.GetOpenWindows();
            IDictionary<IntPtr, Object[]> currentWindows = OpenWindowGetter.GetProcessInfo();
            IDictionary<String, Object> batteryStatus = OpenWindowGetter.GetBatteryStatus();

            // get IP address of host computer
            //string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = getIpAddress();// Dns.GetHostByName(hostName).AddressList[0].ToString();

            // get mac address of host machine
            String firstMacAddress = getMACAddress();
                /*= NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault();*/

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
                    Status = "0",
                    mainAppPath = w.Value[12].ToString(),
                };

                //myAppActivity.Measurements = new List<MeasurementReport>();

                String ChangingStatus = batteryStatus["BatteryStatus"].ToString();
                float EstimatedChargeRemaining = float.Parse(batteryStatus["EstimatedChargeRemaining"].ToString());

                EstimatedChargeRemaining = EstimatedChargeRemaining > 100.0 ? 100 : EstimatedChargeRemaining;

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "1",// "EstimatedChargeRemaining",
                    Value = ChangingStatus== "2" ? "-1" : EstimatedChargeRemaining.ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "2",// "BatteryStatus",
                    Value = batteryStatus["BatteryStatus"].ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "3",// "RAM",
                    Value = w.Value[9].ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "4",// "vRAM",
                    Value = w.Value[10].ToString()
                });

                myAppActivity.Measurements.Add(new Metrics
                {
                    MeasurementType = "5",// "CPU",
                    Value = w.Value[11].ToString()
                });

                myReport.activities.Add(myAppActivity);
            }

            return myReport;
        }

        public String getIpAddress()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            return myIP;
        }

        public String getMACAddress()
        {
            String firstMacAddress = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault();

            var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            var replace = "$1:$2:$3:$4:$5:$6";
            var newformat = Regex.Replace(firstMacAddress, regex, replace);


            return newformat;
        }

        public String getOSVersion()
        {
            return (string)(from x in new ManagementObjectSearcher(
                "SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                            select x.GetPropertyValue("Caption")).FirstOrDefault();
        }
        
        public String getCurrentUser()
        {
            return System.Environment.UserName;
        }
    }
}
