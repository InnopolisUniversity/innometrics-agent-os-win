﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace InnoMetric.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class ActivityReport
    {
        /// <summary>
        /// Initializes a new instance of the ActivityReport class.
        /// </summary>
        public ActivityReport() { }

        /// <summary>
        /// Initializes a new instance of the ActivityReport class.
        /// </summary>
        public ActivityReport(int? activityID = default(int?), string activityType = default(string), string browserTitle = default(string), string browserUrl = default(string), DateTime? endTime = default(DateTime?), string executableName = default(string), bool? idleActivity = default(bool?), string ipAddress = default(string), string macAddress = default(string), DateTime? startTime = default(DateTime?), string userID = default(string))
        {
            ActivityID = activityID;
            ActivityType = activityType;
            BrowserTitle = browserTitle;
            BrowserUrl = browserUrl;
            EndTime = endTime;
            ExecutableName = executableName;
            IdleActivity = idleActivity;
            IpAddress = ipAddress;
            MacAddress = macAddress;
            StartTime = startTime;
            UserID = userID;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activityID")]
        public int? ActivityID { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activityType")]
        public string ActivityType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "browser_title")]
        public string BrowserTitle { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "browser_url")]
        public string BrowserUrl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "end_time")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "executable_name")]
        public string ExecutableName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "idle_activity")]
        public bool? IdleActivity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ip_address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "mac_address")]
        public string MacAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "start_time")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }

    }
}
