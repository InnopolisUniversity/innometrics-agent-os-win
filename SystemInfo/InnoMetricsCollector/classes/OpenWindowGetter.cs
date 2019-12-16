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
        public static IDictionary<HWND, Object[]> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, Object[]> windows = new Dictionary<HWND, Object[]>();

            log.Debug("get the list of opened windows...");

            EnumWindows(delegate (HWND hWnd, int lParam)
            {

                //log.Debug("getting info for a process... -> " + hWnd.ToString() + ", " + lParam.ToString());
                try
                {
                    //log.Debug("-1");
                    if (hWnd == shellWindow) return true;
                    //log.Debug("0");
                    if (!IsWindowVisible(hWnd)) return true;
                    //log.Debug("1");
                    int length = GetWindowTextLength(hWnd);
                    if (length == 0) return true;
                    //log.Debug("2");
                    StringBuilder builder = new StringBuilder(length);
                    GetWindowText(hWnd, builder, length + 1);
                    //log.Debug("3");
                    uint processID;
                    GetWindowThreadProcessId(hWnd, out processID);
                    //log.Debug("4");
                    var process = Process.GetProcessById((int)processID);
                    //log.Debug("getting information for the process -> " + processID);

                    //log.Debug("Process name -> " + process.ProcessName);

                    //log.Debug("RAM usage: " + process.WorkingSet64);
                    //log.Debug("VRAM usage: " + process.VirtualMemorySize64);

                    var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
                    cpuCounter.NextValue();
                    //log.Debug("CPU Usage: " + cpuCounter.NextValue() + "%");
                    //System.Threading.Thread.Sleep(1000);
                    var cpuUsage = cpuCounter.NextValue();
                    //log.Debug("CPU Usage: " + cpuUsage + "%");




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
                    // 1 Process name
                    process.ProcessName,
                    // 2 Process ID
                    processID,//process.Id.ToString(),
                    // 3 Process status
                    status,
                    // 4 Description of the process
                    builder.ToString(),
                    // 5 exe name
                    process.MainModule.ModuleName,
                    // 6 process starting time
                    process.StartTime,
                    // 7 Total processor time
                    process.TotalProcessorTime,
                    // 7 USer processor time
                    process.UserProcessorTime,
                    // 8 window titte
                    process.MainWindowTitle,
                    // 9 RAM Usage
                    process.WorkingSet64,
                    // 10 Virtual RAM Usage
                    process.VirtualMemorySize64,
                    // 11 CPU usage
                    cpuUsage
                    };

                    windows[hWnd] = data;//builder.ToString();
                }
                catch (Exception ex)
                {
                    log.Error("Error getting information -> " + ex.Message + ", " + ex.StackTrace);
                }

                return true;
            }, 0);

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
    }
}
