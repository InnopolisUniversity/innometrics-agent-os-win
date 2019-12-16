using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows;

namespace SystemInfo
{
    class ApiOperations
    {
        /**
         * Base Url @string
         */
        private string baseUrl;

        public ApiOperations()
        {
            this.baseUrl = "https://innometric.guru:8120/";
        }

        /**
 * Authenticate user with Web Api Endpoint
 * @param string username
 * @param string password
 */
        public User AuthenticateUser(string username, string password)
        {
            string endpoint = this.baseUrl + "/login";
            string method = "GET";
            string json = JsonConvert.SerializeObject(new
            {
                username = username,
                password = password
            });

            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";
            try
            {
                string response = wc.UploadString(endpoint, method, json);
                MessageBox.Show("Response ---" + response);
                return JsonConvert.DeserializeObject<User>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /**
 * Register User
 * @param  string username
 * @param  string password
 * @param  string firstname
 * @param  string lastname
 * @param  string middlename
 * @param  int    age
 */
        public User RegisterUser(string username, string password, string email,
            string lastname)
        {
            string endpoint = this.baseUrl + "/user";
            string method = "POST";
            string json = JsonConvert.SerializeObject(new
            {
                username = username,
                password = password,
                email = email,
                lastname = lastname
            });

            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";
            try
            {
                string response = wc.UploadString(endpoint, method, json);
                return JsonConvert.DeserializeObject<User>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
