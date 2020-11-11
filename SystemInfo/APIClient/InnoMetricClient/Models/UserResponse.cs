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

    public partial class UserResponse
    {
        /// <summary>
        /// Initializes a new instance of the UserResponse class.
        /// </summary>
        public UserResponse() { }

        /// <summary>
        /// Initializes a new instance of the UserResponse class.
        /// </summary>
        public UserResponse(string email = default(string), string isactive = default(string), string name = default(string), string surname = default(string))
        {
            Email = email;
            Isactive = isactive;
            Name = name;
            Surname = surname;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isactive")]
        public string Isactive { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "surname")]
        public string Surname { get; set; }

    }
}
