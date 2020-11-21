﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public partial class MeasurementTypeRequest
    {
        /// <summary>
        /// Initializes a new instance of the MeasurementTypeRequest class.
        /// </summary>
        public MeasurementTypeRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MeasurementTypeRequest class.
        /// </summary>
        public MeasurementTypeRequest(string description = default(string), string label = default(string),
            string operation = default(string), double? scale = default(double?), double? weight = default(double?))
        {
            Description = description;
            Label = label;
            Operation = operation;
            Scale = scale;
            Weight = weight;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

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
        [JsonProperty(PropertyName = "weight")]
        public double? Weight { get; set; }
    }
}