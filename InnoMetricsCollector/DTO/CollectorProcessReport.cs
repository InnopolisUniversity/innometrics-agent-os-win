using System.Collections.Generic;

namespace InnoMetricsCollector.DTO
{
    public class CollectorProcessReport
    {
        public CollectorProcessReport()
        {
            processes = new List<CollectorProcess>();
        }

        public List<CollectorProcess> processes { get; set; }
    }
}