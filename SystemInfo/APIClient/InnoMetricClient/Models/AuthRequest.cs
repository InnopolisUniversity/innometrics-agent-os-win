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

    public partial class AuthRequest
    {
        /// <summary>
        /// Initializes a new instance of the AuthRequest class.
        /// </summary>
        public AuthRequest() { }

        /// <summary>
        /// Initializes a new instance of the AuthRequest class.
        /// </summary>
        public AuthRequest(string email = default(string), string password = default(string), string projectID = default(string))
        {
            Email = email;
            Password = password;
            ProjectID = projectID;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "projectID")]
        public string ProjectID { get; set; }

    }
}
