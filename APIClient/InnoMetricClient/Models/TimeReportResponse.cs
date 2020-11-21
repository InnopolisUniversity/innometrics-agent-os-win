﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public partial class TimeReportResponse
    {
        /// <summary>
        /// Initializes a new instance of the TimeReportResponse class.
        /// </summary>
        public TimeReportResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TimeReportResponse class.
        /// </summary>
        public TimeReportResponse(IList<TimeReportByUser> report = default(IList<TimeReportByUser>))
        {
            Report = report;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "report")]
        public IList<TimeReportByUser> Report { get; set; }
    }
}