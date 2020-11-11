using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricsCollector.classes
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
