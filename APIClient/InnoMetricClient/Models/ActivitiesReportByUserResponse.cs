﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public partial class ActivitiesReportByUserResponse
    {
        /// <summary>
        /// Initializes a new instance of the ActivitiesReportByUserResponse
        /// class.
        /// </summary>
        public ActivitiesReportByUserResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ActivitiesReportByUserResponse
        /// class.
        /// </summary>
        public ActivitiesReportByUserResponse(
            IList<ActivitiesReportByUser> report = default(IList<ActivitiesReportByUser>))
        {
            Report = report;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "report")]
        public IList<ActivitiesReportByUser> Report { get; set; }
    }
}