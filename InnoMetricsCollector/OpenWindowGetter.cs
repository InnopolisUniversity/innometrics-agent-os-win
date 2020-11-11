using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using InnoMetricsCollector.classes;
using log4net.Appender;
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
            // TODO: this needs redesign; it queries duplicate values from the same processes
            var shellWindow = GetShellWindow();
            var windowHandles = FindWindows()
                .Where(window => window != shellWindow).ToArray();

            var processes = windowHandles.Select(w =>
            {
                GetWindowThreadProcessId(w, out var pid);
                return Process.GetProcessById(pid);
            }).ToArray();

            var windowInfo = windowHandles.Select(GetWindowInfoWithoutProcess).ToArray();
            var processInfo = processes.Select(GetProcessInfo).ToArray();
            Thread.Sleep(1000);

            var now = DateTime.Now;
            var cpuTimes = processes.Select(p => p.TotalProcessorTime);

            foreach (var (pi, time) in processInfo.Zip(cpuTimes, (p, t) => (p, t)))
            {
                var dt = now - pi.MeasurementTime;
                var dpt = time - pi.TotalProcessorTime;
                pi.CpuUsage = dpt.TotalSeconds / dt.TotalSeconds * 100 / LogicalProcessors;
            }

            foreach (var (pi, w) in processInfo.Zip(windowInfo, (p, w) => (p, w)))
            {
                w.ProcessInfo = pi;
            }

            return windowHandles.Zip(windowInfo, (k, v) => (k, v))
                .ToDictionary(t => t.k, t => t.v);
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
            var res = GetWindowInfoWithoutProcess(hWnd);
            GetWindowThreadProcessId(hWnd, out var pid);
            res.ProcessInfo = GetProcessInfo(pid);
            return res;
        }

        private static WindowInfo GetWindowInfoWithoutProcess(HWND hWnd)
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

            
            return new WindowInfo
            {
                WindowTitle = windowTitle,
                IsForeground = GetForegroundWindow() == hWnd,
            };
        }

        private static ProcessInfo GetProcessInfo(int pid) => GetProcessInfo(Process.GetProcessById(pid));

        private static ProcessInfo GetProcessInfo(Process process)
        {
            var mainModule = process.MainModule;

            // TODO: probably not a cheap operation
            //var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            //cpuCounter.NextValue();
            //var cpuUsage = cpuCounter.NextValue();
            var cpuUsage = 0;
            
            return new ProcessInfo
            {
                ProcessName = process.ProcessName,
                ProcessId = process.Id,
                MainModuleName = mainModule.ModuleName,
                MainModulePath = mainModule.FileName,
                MainModuleDescription = mainModule.FileVersionInfo.FileDescription,
                
                StartTime = process.StartTime,
                TotalProcessorTime = process.TotalProcessorTime,
                UserProcessorTime = process.UserProcessorTime,
                
                MeasurementTime = DateTime.Now,
                
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
            public DateTime MeasurementTime { get; set; }
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
