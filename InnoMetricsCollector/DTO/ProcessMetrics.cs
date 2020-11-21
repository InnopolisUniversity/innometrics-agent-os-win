using System;

namespace InnoMetricsCollector.DTO
{
    public class ProcessMetrics
    {
        public String MeasurementType { get; set; }
        public String Value { get; set; }
        public DateTime CapturedTime { get; set; }
    }
}