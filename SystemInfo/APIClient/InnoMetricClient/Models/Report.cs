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

    public partial class Report
    {
        /// <summary>
        /// Initializes a new instance of the Report class.
        /// </summary>
        public Report() { }

        /// <summary>
        /// Initializes a new instance of the Report class.
        /// </summary>
        public Report(IList<ActivityReport> activities = default(IList<ActivityReport>))
        {
            Activities = activities;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activities")]
        public IList<ActivityReport> Activities { get; set; }

    }
}
