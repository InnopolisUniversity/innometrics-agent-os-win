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

    public partial class MeasurementTypeResponse
    {
        /// <summary>
        /// Initializes a new instance of the MeasurementTypeResponse class.
        /// </summary>
        public MeasurementTypeResponse() { }

        /// <summary>
        /// Initializes a new instance of the MeasurementTypeResponse class.
        /// </summary>
        public MeasurementTypeResponse(string createdby = default(string), DateTime? creationdate = default(DateTime?), string description = default(string), string isactive = default(string), string label = default(string), DateTime? lastupdate = default(DateTime?), int? measurementtypeid = default(int?), string operation = default(string), double? scale = default(double?), string updateby = default(string), double? weight = default(double?))
        {
            Createdby = createdby;
            Creationdate = creationdate;
            Description = description;
            Isactive = isactive;
            Label = label;
            Lastupdate = lastupdate;
            Measurementtypeid = measurementtypeid;
            Operation = operation;
            Scale = scale;
            Updateby = updateby;
            Weight = weight;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdby")]
        public string Createdby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "creationdate")]
        public DateTime? Creationdate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isactive")]
        public string Isactive { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lastupdate")]
        public DateTime? Lastupdate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "measurementtypeid")]
        public int? Measurementtypeid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "operation")]
        public string Operation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "scale")]
        public double? Scale { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "updateby")]
        public string Updateby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "weight")]
        public double? Weight { get; set; }

    }
}
