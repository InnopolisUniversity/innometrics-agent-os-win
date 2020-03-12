using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricsCollector.classes
{
    public class ProcessMetrics
    {
        public String MeasurementType { get; set; }
        public String Value { get; set; }
        public DateTime CapturedTime { get; set; }
    }
}
