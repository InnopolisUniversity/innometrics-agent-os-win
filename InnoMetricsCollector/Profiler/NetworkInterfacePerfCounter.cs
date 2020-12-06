using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricsCollector.Profiler
{
    public class NetworkInterfacePerfCounter : AbstractPerfCounter<NetworkInterfacePerfCounter.NetworkPerfType>
    {
        private static readonly string NetworkInterfaceCategory = "Network Interface";

        public NetworkInterfacePerfCounter() : base(NetworkInterfaceCategory)
        {
        }

        public new void Initialize(string networkInterface, params NetworkPerfType[] preferredTypes)
        {
            base.Initialize(networkInterface, preferredTypes);
        }

        public enum NetworkPerfType
        {
            BytesReceived,
            BytesSent,
            TotalBytes,
            CurrentBandwidth,
            PacketsReceived,
            PacketsSent,
            TotalPackets
        }

        protected override string CounterTypeToString(NetworkPerfType type)
        {
            switch (type)
            {
                case NetworkPerfType.BytesReceived:
                    return "Bytes Received/sec";
                case NetworkPerfType.BytesSent:
                    return "Bytes Total/sec";
                case NetworkPerfType.TotalBytes:
                    return "Bytes Total/sec";
                case NetworkPerfType.CurrentBandwidth:
                    return "Current Bandwidth";
                case NetworkPerfType.PacketsReceived:
                    return "Packets Received/sec";
                case NetworkPerfType.PacketsSent:
                    return "Packets Sent/sec";
                case NetworkPerfType.TotalPackets:
                    return "Packets/sec";
                default:
                    throw new InvalidOperationException("Unknown type: " + Enum.GetName(typeof(NetworkPerfType), type));
            }
        }
    }
}