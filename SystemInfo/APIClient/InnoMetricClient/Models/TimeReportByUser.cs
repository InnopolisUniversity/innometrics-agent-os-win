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

    public partial class TimeReportByUser
    {
        /// <summary>
        /// Initializes a new instance of the TimeReportByUser class.
        /// </summary>
        public TimeReportByUser() { }

        /// <summary>
        /// Initializes a new instance of the TimeReportByUser class.
        /// </summary>
        public TimeReportByUser(string activityDay = default(string), string email = default(string), string timeUsed = default(string))
        {
            ActivityDay = activityDay;
            Email = email;
            TimeUsed = timeUsed;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activity_day")]
        public string ActivityDay { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "time_used")]
        public string TimeUsed { get; set; }

    }
}