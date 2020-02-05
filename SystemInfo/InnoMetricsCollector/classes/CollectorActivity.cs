using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricsCollector.classes
{
    public class CollectorActivity
    {
        public int ActivityID { get; set; }
        public string ActivityType { get; set; }
        public string BrowserTitle { get; set; }
        public string BrowserUrl { get; set; }
        public string ExecutableName { get; set; }
        public String ProcessId { get; set; }
        public bool IdleActivity { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public List<Metrics> Measurements { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string UserName { get; set; }
        public String Description { get; set; }
        public String Status { get; set; }
        public String mainAppPath { get; set; }

        public CollectorActivity()
        {
            Measurements = new List<Metrics>();
        }
    }
}
