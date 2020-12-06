using System;
using System.Diagnostics;

namespace InnoMetricsCollector.Profiler
{
    public class IOPerfCounter : AbstractPerfCounter<IOPerfCounter.IOPerfType>
    {
        public enum IOPerfType
        {
            BytesRead,
            BytesWritten,
            ReadOperations,
            WriteOperations,
            DataOperations,
            DataRate
        }

        private static readonly string ProcessCategory = "Process";

        public IOPerfCounter() : base(ProcessCategory)
        {
        }

        public void Initialize(Process process, params IOPerfType[] preferredTypes)
        {
            base.Initialize(process.ProcessName, preferredTypes);
        }

        protected override string CounterTypeToString(IOPerfType type)
        {
            switch (type)
            {
                case IOPerfType.BytesRead:
                    return "IO Read Bytes/sec";
                case IOPerfType.BytesWritten:
                    return "IO Write Bytes/sec";
                case IOPerfType.ReadOperations:
                    return "IO Read Operations/sec";
                case IOPerfType.WriteOperations:
                    return "IO Write Operations/se";
                case IOPerfType.DataOperations:
                    return "IO Data Operations/sec";
                case IOPerfType.DataRate:
                    return "IO Data Bytes/sec";
                default:
                    throw new InvalidOperationException("Unknown type: " + Enum.GetName(typeof(IOPerfType), type));
            }
        }
    }
}