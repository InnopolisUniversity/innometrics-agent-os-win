﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System;
using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public class UserRequest
    {
        /// <summary>
        ///     Initializes a new instance of the UserRequest class.
        /// </summary>
        public UserRequest()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the UserRequest class.
        /// </summary>
        public UserRequest(DateTime? confirmedAt = default, string email = default,
            string isactive = default, string name = default, string password = default,
            string surname = default)
        {
            ConfirmedAt = confirmedAt;
            Email = email;
            Isactive = isactive;
            Name = name;
            Password = password;
            Surname = surname;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "confirmed_at")]
        public DateTime? ConfirmedAt { get; set; }

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
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "surname")]
        public string Surname { get; set; }
    }
}