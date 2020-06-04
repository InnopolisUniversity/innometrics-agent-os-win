using System;
using System.Collections.Generic;
using System.Management;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;


using HWND = System.IntPtr;
using System.Linq;

namespace InnoMetricsCollector.classes
{
    class OpenWindowGetter
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region "Import USER32.dll"
        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        #endregion 

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<HWND, Object[]> GetProcessInfo()
        {
            Dictionary<HWND, Object[]> windows = new Dictionary<HWND, Object[]>();
            //Console.WriteLine("Number Of Logical Processors: {0}", Environment.ProcessorCount);
            int LogicalProcessors = Environment.ProcessorCount;
            object processid = String.Empty;
            HWND shellWindow = GetShellWindow();
            try
            {
                ObjectQuery sq = new ObjectQuery("Select * from Win32_Process where SessionId != 0");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);

                int counter = searcher.Get().Count;
                foreach (ManagementObject oReturn in searcher.Get())
                {
                    try
                    {
                        processid = oReturn["ProcessId"];
                        Process process = Process.GetProcessById(Int32.Parse(processid.ToString()));

                        IntPtr hWnd = process.MainWindowHandle;

                        Object[] data = GetProcessData(uint.Parse(processid.ToString()), shellWindow, LogicalProcessors);

                        windows.Add(hWnd, data);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error getting information -> " + ex.Message + ", " + ex.StackTrace);
                    }
                }
            }
            catch
            {
                return null;
            }
            return windows;
        }

        public static IDictionary<String, Object> GetBatteryStatus()
        {
            Dictionary<String, Object> BatteryStatus = new Dictionary<String, Object>();

            ObjectQuery query = new ObjectQuery("Select * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject mo in collection)
            {
                foreach (PropertyData property in mo.Properties)
                {
                    BatteryStatus.Add(property.Name, property.Value);
                    //Console.WriteLine("Property {0}: Value is {1}", property.Name, property.Value);
                }
            }

            return BatteryStatus;
        }
        /// <summary>
        /// Returns the process details given a hWnd pointer
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns>Returns an array of details</returns>
        public static Object[] GetProcessDetails(IntPtr hWnd)
        {
            CollectorActivity newActivity = new CollectorActivity();

            uint processID;
            int LogicalProcessors = Environment.ProcessorCount;

            GetWindowThreadProcessId(hWnd, out processID);
            Object[] data = GetProcessData(processID, IntPtr.Zero, LogicalProcessors);

            return data;

        }


        private static Object[] GetProcessData(uint processID, HWND shellWindow, int LogicalProcessors)
        {
            try {
                Process process = Process.GetProcessById(Int32.Parse(processID.ToString()));

                IntPtr hWnd = process.MainWindowHandle;

                if (shellWindow != IntPtr.Zero)
                    if (hWnd == shellWindow) return null;

                //if (!IsWindowVisible(hWnd)) continue;
                String AppDescription = "";
                int length = GetWindowTextLength(hWnd);
                if (length != 0) {
                    StringBuilder builder = new StringBuilder(length);
                    GetWindowText(hWnd, builder, length + 1);
                    AppDescription = builder.ToString();
                }

                var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
                cpuCounter.NextValue();
                var cpuUsage = cpuCounter.NextValue();

                Double cpuUtilization = Double.Parse(cpuUsage.ToString()) / LogicalProcessors;

                string status;
                if (hWnd == GetForegroundWindow())
                {
                    status = "ACTIVE";
                }
                else
                {
                    status = "IDLE";
                }

                Object[] data = {
                            // 0 Process name
                            process.ProcessName,
                            // 1 Process ID
                            processID.ToString(),//process.Id.ToString(),
                            // 2 Process status
                            status,
                            // 3 Description of the process
                            AppDescription,//builder.ToString(),
                            // 4 exe name
                            process.MainModule.ModuleName,
                            // 5 process starting time
                            process.StartTime,
                            // 6 Total processor time
                            process.TotalProcessorTime,
                            // 7 USer processor time
                            process.UserProcessorTime,
                            // 8 window titte
                            process.MainWindowTitle,
                            // 9 RAM Usage
                            process.WorkingSet64/(1024*1024),
                            // 10 Virtual RAM Usage
                            process.VirtualMemorySize64 /(1024*1024),
                            // 11 CPU usage
                            cpuUtilization, //cpuUsage,
                            //12 Executable path
                            process.MainModule.FileName,
                        };

                return data;
            }
            catch(Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
                return null;
            }
            

        }
    }
}
