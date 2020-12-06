﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System;
using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public class Role
    {
        /// <summary>
        ///     Initializes a new instance of the Role class.
        /// </summary>
        public Role()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the Role class.
        /// </summary>
        public Role(string createdby = default, DateTime? creationdate = default,
            string description = default, string isactive = default,
            DateTime? lastupdate = default, string name = default, string updateby = default)
        {
            Createdby = createdby;
            Creationdate = creationdate;
            Description = description;
            Isactive = isactive;
            Lastupdate = lastupdate;
            Name = name;
            Updateby = updateby;
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
        [JsonProperty(PropertyName = "lastupdate")]
        public DateTime? Lastupdate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "updateby")]
        public string Updateby { get; set; }
    }
}