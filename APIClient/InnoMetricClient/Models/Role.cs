﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System;
using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public partial class Role
    {
        /// <summary>
        /// Initializes a new instance of the Role class.
        /// </summary>
        public Role()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Role class.
        /// </summary>
        public Role(string createdby = default(string), DateTime? creationdate = default(DateTime?),
            string description = default(string), string isactive = default(string),
            DateTime? lastupdate = default(DateTime?), string name = default(string), string updateby = default(string))
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