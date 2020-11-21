using System.Collections.Generic;

namespace InnoMetricsCollector.DTO
{
    public class CollectorProcessReport
    {
        public List<CollectorProcess> processes { get; set; }

        public CollectorProcessReport()
        {
            processes = new List<CollectorProcess>();
        }
    }
}