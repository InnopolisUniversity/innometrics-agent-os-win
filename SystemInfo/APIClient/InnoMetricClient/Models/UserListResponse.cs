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

    public partial class UserListResponse
    {
        /// <summary>
        /// Initializes a new instance of the UserListResponse class.
        /// </summary>
        public UserListResponse() { }

        /// <summary>
        /// Initializes a new instance of the UserListResponse class.
        /// </summary>
        public UserListResponse(IList<UserResponse> userList = default(IList<UserResponse>))
        {
            UserList = userList;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "userList")]
        public IList<UserResponse> UserList { get; set; }

    }
}
