using System;

// Required namespaces

namespace SystemInfo
{
    public class DataLine
    {
        // key properties
        public String ProcessName { get; set; }
        public String ProcessId { get; set; }
        public String Status { get; set; }
        public String StartTime { get; set; }
        public String EndTime { get; set; }
        public String MacAddress { get; set; }
        public String IPAddress { get; set; }
        public String Description { get; set; }

        public DataLine()
        {
        }
        public DataLine(String processName, String processId, String processStatus)
        {
            ProcessName = processName;
            ProcessId = processId;
            Status = processStatus;
        }

        public DataLine(String processName, String processId, String processStatus, String stime, String etime)
            : this(processName, processId, processStatus)
        {
            StartTime = stime;
            EndTime = etime;
        }

        public DataLine(String processName, String processId, String processStatus, String stime, String etime,
            String Ip_address, String Mac_address, String Description)
            : this(processName, processId, processStatus, stime, etime)
        {
            IPAddress = Ip_address;
            MacAddress = Mac_address;
            this.Description = Description;
        }
    }
}