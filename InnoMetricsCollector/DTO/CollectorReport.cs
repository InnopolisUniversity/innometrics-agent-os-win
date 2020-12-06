using System.Collections.Generic;

namespace InnoMetricsCollector.DTO
{
    public class CollectorReport
    {
        public CollectorReport()
        {
            activities = new List<CollectorActivity>();
        }

        public List<CollectorActivity> activities { get; set; }
    }
}