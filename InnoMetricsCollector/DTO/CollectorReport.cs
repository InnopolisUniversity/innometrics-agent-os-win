using System.Collections.Generic;

namespace InnoMetricsCollector.DTO
{
    public class CollectorReport
    {
        public List<CollectorActivity> activities { get; set; }

        public CollectorReport()
        {
            activities = new List<CollectorActivity>();
        }
    }
}