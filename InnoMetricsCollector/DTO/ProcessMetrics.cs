using System;

namespace InnoMetricsCollector.DTO
{
    public class ProcessMetrics
    {
        public string MeasurementType { get; set; }
        public string Value { get; set; }
        public DateTime CapturedTime { get; set; }
    }
}