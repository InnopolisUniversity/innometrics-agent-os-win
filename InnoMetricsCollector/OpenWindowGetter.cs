using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using InnoMetricsCollector.classes;
using HWND = System.IntPtr;

namespace InnoMetricsCollector
{
    class OpenWindowGetter
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int LogicalProcessors = Environment.ProcessorCount;
        
        // TODO: move those into a separate class
        #region "Import USER32.dll"
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        #endregion 

        private static IEnumerable<IntPtr> FindWindows()
        {
            var res = new List<IntPtr>();

            EnumWindows(delegate(IntPtr wnd, IntPtr param)
            {
                if (IsWindowVisible(wnd))
                    res.Add(wnd);
                return true;
            }, IntPtr.Zero);

            return res;
        }
        
        public static IDictionary<HWND, WindowInfo> GetOpenedWindowsProcessesInfo()
        {
            var shellWindow = GetShellWindow();
            return FindWindows()
                .Where(window => window != shellWindow)
                .ToDictionary(window => window, GetWindowInfo);
        }

        // What the battery stuff is even doing here?!
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
        
        public static WindowInfo GetWindowInfo(HWND hWnd)
        {
            var windowTitle = "";
            var length = GetWindowTextLength(hWnd);
            if (length != 0)
            {
                // TODO: fix race condition
                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);
                windowTitle = builder.ToString();
            }

            GetWindowThreadProcessId(hWnd, out var pid);
            
            return new WindowInfo
            {
                WindowTitle = windowTitle,
                IsForeground = GetForegroundWindow() == hWnd,
                ProcessInfo = GetProcessInfo(pid),
            };
        }
        
        public static ProcessInfo GetProcessInfo(int pid)
        {
            var process = Process.GetProcessById(pid);

            var mainModule = process.MainModule;

            // TODO: probably not a cheap operation
            //var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            //cpuCounter.NextValue();
            var cpuUsage = 0.0;//cpuCounter.NextValue();
            
            return new ProcessInfo
            {
                ProcessName = process.ProcessName,
                ProcessId = pid,
                MainModuleName = mainModule.ModuleName,
                MainModulePath = mainModule.FileName,
                MainModuleDescription = mainModule.FileVersionInfo.FileDescription,
                
                StartTime = process.StartTime,
                TotalProcessorTime = process.TotalProcessorTime,
                UserProcessorTime = process.UserProcessorTime,
                
                RamVirtualSize = (int)(process.VirtualMemorySize64 / 1024 / 1024), // B -> MiB
                RamWorkingSetSize = (int)(process.WorkingSet64 / 1024 / 1024),
                
                CpuUsage = cpuUsage,
            };
        }

        public class ProcessInfo
        {
            public string ProcessName { get; set; }
            public int ProcessId { get; set; }
            public string MainModuleName { get; set; }
            public string MainModulePath { get; set; }
            public string MainModuleDescription { get; set; }
            public DateTime StartTime { get; set; }
            public TimeSpan TotalProcessorTime { get; set; }
            public TimeSpan UserProcessorTime { get; set; }
            public int RamWorkingSetSize { get; set; }
            public int RamVirtualSize { get; set; }
            public double CpuUsage { get; set; }
        }

        public class WindowInfo
        {
            public string WindowTitle { get; set; }
            public bool IsForeground { get; set; }
            
            public ProcessInfo ProcessInfo { get; set; }
        }
    }
}
