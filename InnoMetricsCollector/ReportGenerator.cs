using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using InnoMetricsCollector.DTO;
using log4net;

namespace InnoMetricsCollector
{
    public class ReportGenerator
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CollectorProcessReport GetCurrentProcessReport(DateTime measurementTime)
        {
            log.Info("Generation process report...");
            var myReport = new CollectorProcessReport();

            var currentWindows = OpenWindowGetter.GetOpenedWindowsProcessesInfo();
            var batteryStatus = OpenWindowGetter.GetBatteryStatus();

            // get IP address of host computer
            // Retrive the Name of HOST  
            var myIp = GetIpAddress();

            // get mac address of host machine
            var firstMacAddress = GetMACAddress();

            foreach (var w in currentWindows)
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
                // 12 Executable path
                // 13 File name description

                //Khaled:
                // 14 IO Rate
                // 15 NetworkUsage

                var wi = w.Value;
                var pi = w.Value.ProcessInfo;

                var myProcess = new CollectorProcess
                {
                    ProcessName = pi.ProcessName,
                    ProcessType = "OS",
                    WindowsTitle = wi.WindowTitle,
                    BrowserUrl = "",
                    IpAddress = myIp,
                    MacAddress = firstMacAddress,
                    UserName = "",
                    Description = pi.MainModuleDescription,
                    Status = "0",
                    mainAppPath = pi.MainModulePath,
                    PID = pi.ProcessId.ToString(),
                    collectedTime = measurementTime
                };

                if (batteryStatus != null && batteryStatus.Count > 0)
                {
                    var changingStatus = batteryStatus["BatteryStatus"].ToString();
                    var estimatedChargeRemaining = float.Parse(batteryStatus["EstimatedChargeRemaining"].ToString());

                    estimatedChargeRemaining = estimatedChargeRemaining > 100.0 ? 100 : estimatedChargeRemaining;

                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "1", // "EstimatedChargeRemaining",
                        Value = changingStatus == "2" ? "-1" : estimatedChargeRemaining.ToString(),
                        CapturedTime = measurementTime
                    });

                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "2", // "BatteryStatus",
                        Value = batteryStatus["BatteryStatus"].ToString(),
                        CapturedTime = measurementTime
                    });
                }
                else
                {
                    // Default values for desktop computers
                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "1", // "EstimatedChargeRemaining",
                        Value = "-1",
                        CapturedTime = measurementTime
                    });

                    myProcess.Measurements.Add(new ProcessMetrics
                    {
                        MeasurementType = "2", // "BatteryStatus",
                        Value = "-1",
                        CapturedTime = measurementTime
                    });
                }

                myProcess.Measurements.Add(new ProcessMetrics
                {
                    MeasurementType = "3", // "RAM",
                    Value = pi.RamWorkingSetSize.ToString(),
                    CapturedTime = measurementTime
                });

                myProcess.Measurements.Add(new ProcessMetrics
                {
                    MeasurementType = "4", // "vRAM",
                    Value = pi.RamVirtualSize.ToString(),
                    CapturedTime = measurementTime
                });

                myProcess.Measurements.Add(new ProcessMetrics
                {
                    MeasurementType = "5", // "CPU",
                    Value = pi.CpuUsage.ToString(CultureInfo.InvariantCulture),
                    CapturedTime = measurementTime
                });

                myProcess.Measurements.Add(new ProcessMetrics
                {
                    MeasurementType = "7", // "GPU",
                    Value = pi.GpuUsage.ToString(CultureInfo.InvariantCulture),
                    CapturedTime = measurementTime
                });

                myProcess.Measurements.Add(new ProcessMetrics
                {
                    MeasurementType = "14", // "IO",
                    Value = pi.IOUsage.ToString(CultureInfo.InvariantCulture),
                    CapturedTime = measurementTime
                });

                myProcess.Measurements.Add(new ProcessMetrics
                {
                    MeasurementType = "15", // "NetworkUsage",
                    Value = pi.NetworkUsage.ToString(CultureInfo.InvariantCulture),
                    CapturedTime = measurementTime
                });
                myReport.processes.Add(myProcess);
            }


            return myReport;
        }


        public CollectorActivity GetCurrentActivity(IntPtr hWnd, DateTime measurementTime)
        {
            log.Info("Generation activity report...");
            CollectorActivity myActivity = null;
            try
            {
                var wi = OpenWindowGetter.GetWindowInfo(hWnd);
                var pi = wi.ProcessInfo;

                var myIP = GetIpAddress();
                var firstMacAddress = GetMACAddress();

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
                // 12 Executable path
                // 13 File name description

                //CollectorActivity 
                if (wi != null)
                    myActivity = new CollectorActivity
                    {
                        ActivityID = -1,
                        ActivityType = "OS",
                        BrowserTitle = wi.WindowTitle,
                        StartTime = measurementTime,
                        ExecutableName = pi.MainModuleName,
                        IdleActivity = false,
                        ProcessId = pi.ProcessId.ToString(),
                        BrowserUrl = "",
                        IpAddress = myIP,
                        MacAddress = firstMacAddress,
                        UserName = "",
                        Description = wi.WindowTitle,
                        Status = "0",
                        mainAppPath = pi.MainModulePath,
                        AppName = pi.MainModuleDescription
                    };


                //return myActivity;
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                //return null;
            }

            return myActivity;
        }

        public string GetIpAddress()
        {
            var hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            var myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            return myIP;
        }

        public string GetMACAddress()
        {
            var firstMacAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic =>
                    nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            var replace = "$1:$2:$3:$4:$5:$6";
            var newformat = Regex.Replace(firstMacAddress, regex, replace);


            return newformat;
        }

        public string GetOSVersion()
        {
            return (string) (from x in new ManagementObjectSearcher(
                    "SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                select x.GetPropertyValue("Caption")).FirstOrDefault();
        }

        public string GetCurrentUser()
        {
            return Environment.UserName;
        }
    }
}