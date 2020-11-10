using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricsCollector.classes
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
