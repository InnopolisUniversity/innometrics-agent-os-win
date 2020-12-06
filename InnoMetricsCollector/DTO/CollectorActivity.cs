using System;
using System.Collections.Generic;

namespace InnoMetricsCollector.DTO
{
    public class CollectorActivity
    {
        public CollectorActivity()
        {
            Measurements = new List<Metrics>();
        }

        public int ActivityID { get; set; }
        public string ActivityType { get; set; }
        public string BrowserTitle { get; set; }
        public string BrowserUrl { get; set; }
        public string ExecutableName { get; set; }
        public string ProcessId { get; set; }
        public bool IdleActivity { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public List<Metrics> Measurements { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string mainAppPath { get; set; }
        public string AppName { get; set; }
    }
}