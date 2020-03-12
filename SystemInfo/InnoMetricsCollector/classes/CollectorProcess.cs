using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricsCollector.classes
{
    public class CollectorProcess
    {
        public int ProcessID { get; set; }
        public string ProcessType { get; set; }
        public string ProcessName { get; set; }
        public string WindowsTitle { get; set; }
        public string BrowserUrl { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public List<ProcessMetrics> Measurements { get; set; }        
        public string UserName { get; set; }
        public String Description { get; set; }
        public String Status { get; set; }
        public String mainAppPath { get; set; }
        public String PID { get; set; }

        public CollectorProcess()
        {
            ProcessID = -1;
            Measurements = new List<ProcessMetrics>();
        }
    }
}
