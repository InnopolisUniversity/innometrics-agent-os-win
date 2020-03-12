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

        public CollectorProcessReport GetCurrentProcessReport(DateTime measurementTime)
        {
            log.Info("Generation process report...");
            CollectorProcessReport myReport = new CollectorProcessReport();

            IDictionary<IntPtr, Object[]> currentWindows = OpenWindowGetter.GetProcessInfo();
            IDictionary<String, Object> batteryStatus = OpenWindowGetter.GetBatteryStatus();

            // get IP address of host computer
            // Retrive the Name of HOST  
            string myIP = GetIpAddress();

            // get mac address of host machine
            String firstMacAddress = GetMACAddress();

            foreach (KeyValuePair<IntPtr, Object[]> w in currentWindows)
            {

                // 0 Process name
                // 1 Process ID
                // 2 Process status
                // 3 Description of the process
                // 4 exe name
                // 5 process starting time
                // 6 Total processor time
                // 7 USer processor time
                // 8 window titte
                // 9 RAM Usage
                // 10 Virtual RAM Usage
                // 11 CPU usage
                //12 Executable path
                if(w.Value != null)
                {
                    CollectorProcess myProcess = new CollectorProcess
                    {
                        ProcessName = w.Value[4].ToString(),
                        ProcessType = "OS",
                        WindowsTitle = w.Value[8].ToString(),
                        BrowserUrl = "",
                        IpAddress = myIP,
                        MacAddress = firstMacAddress,
                        UserName = "",
                        Description = w.Value[3].ToString(),
                        Status = "0",
                        mainAppPath = w.Value[12].ToString(),
                        PID = w.Value[1].ToString(),
                    };

                    if (batteryStatus != null && batteryStatus.Count > 0)
                    {
                        String ChangingStatus = batteryStatus["BatteryStatus"].ToString();
                        float EstimatedChargeRemaining = float.Parse(batteryStatus["EstimatedChargeRemaining"].ToString());

                        EstimatedChargeRemaining = EstimatedChargeRemaining > 100.0 ? 100 : EstimatedChargeRemaining;

                        myProcess.Measurements.Add(new ProcessMetrics
                        {
                            MeasurementType = "1",// "EstimatedChargeRemaining",
                            Value = ChangingStatus == "2" ? "-1" : EstimatedChargeRemaining.ToString(),
                            CapturedTime = measurementTime

                        });

                        myProcess.Measurements.Add(new ProcessMetrics
                        {
                            MeasurementType = "2",// "BatteryStatus",
                            Value = batteryStatus["BatteryStatus"].ToString(),
                            CapturedTime = measurementTime
                        });

                    }
                    else
                    {

                        // Default values for desktop computers
                        myProcess.Measurements.Add(new ProcessMetrics
                        {
                            MeasurementType = "1",// "EstimatedChargeRemaining",
                            Value = "-1",
                            CapturedTime = measurementTime
                        });

                        myProcess.Measurements.Add(new ProcessMetrics
                        {
                            MeasurementType = "2",// "BatteryStatus",
                            Value = "-1",
                            CapturedTime = measurementTime
                        });
                    }

                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "3",// "RAM",
                        Value = w.Value[9].ToString(),
                        CapturedTime = measurementTime
                    });

                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "4",// "vRAM",
                        Value = w.Value[10].ToString(),
                        CapturedTime = measurementTime
                    });

                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "5",// "CPU",
                        Value = w.Value[11].ToString(),
                        CapturedTime = measurementTime
                    });

                    myReport.processes.Add(myProcess);
                }
                
            }

            return myReport;
        }


        public CollectorActivity GetCurrentActivity(IntPtr hWnd, DateTime measurementTime)
        {
            log.Info("Generation activity report...");
            CollectorActivity myActivity = null;
            try
            {
                Object[] currentWindow = OpenWindowGetter.GetProcessDetails(hWnd);

                String myIP = GetIpAddress();
                String firstMacAddress = GetMACAddress();

                // 0 Process name
                // 1 Process ID
                // 2 Process status
                // 3 Description of the process
                // 4 exe name
                // 5 process starting time
                // 6 Total processor time
                // 7 USer processor time
                // 8 window titte
                // 9 RAM Usage
                // 10 Virtual RAM Usage
                // 11 CPU usage
                //12 Executable path

                //CollectorActivity 
                if(currentWindow != null)
                {
                    myActivity = new CollectorActivity
                    {
                        ActivityID = -1,
                        ActivityType = "OS",
                        BrowserTitle = currentWindow[8].ToString(),
                        StartTime = measurementTime,
                        ExecutableName = currentWindow[4].ToString(),
                        IdleActivity = false,
                        ProcessId = currentWindow[1].ToString(),
                        BrowserUrl = "",
                        IpAddress = myIP,
                        MacAddress = firstMacAddress,
                        UserName = "",
                        Description = currentWindow[3].ToString(),
                        Status = "0",
                        mainAppPath = currentWindow[12].ToString(),
                    };
                }
                   

                //return myActivity;
            }
            catch(Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                //return null;
            }
            return myActivity;


        }

        public String GetIpAddress()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            return myIP;
        }

        public String GetMACAddress()
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

        public String GetOSVersion()
        {
            return (string)(from x in new ManagementObjectSearcher(
                "SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                            select x.GetPropertyValue("Caption")).FirstOrDefault();
        }
        
        public String GetCurrentUser()
        {
            return System.Environment.UserName;
        }
    }
}
