﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIClient.InnoMetricClient.Models
{
    public class ProjectListResponse
    {
        /// <summary>
        ///     Initializes a new instance of the ProjectListResponse class.
        /// </summary>
        public ProjectListResponse()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the ProjectListResponse class.
        /// </summary>
        public ProjectListResponse(IList<ProjectResponse> projectList = default)
        {
            ProjectList = projectList;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "projectList")]
        public IList<ProjectResponse> ProjectList { get; set; }
    }
}