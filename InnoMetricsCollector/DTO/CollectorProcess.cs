using System;
using System.Collections.Generic;

namespace InnoMetricsCollector.DTO
{
    public class CollectorProcess
    {
        public CollectorProcess()
        {
            ProcessID = -1;
            Measurements = new List<ProcessMetrics>();
        }

        public int ProcessID { get; set; }
        public string ProcessType { get; set; }
        public string ProcessName { get; set; }
        public string WindowsTitle { get; set; }
        public string BrowserUrl { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public List<ProcessMetrics> Measurements { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string mainAppPath { get; set; }
        public string PID { get; set; }
        public DateTime collectedTime { get; set; }
    }
}