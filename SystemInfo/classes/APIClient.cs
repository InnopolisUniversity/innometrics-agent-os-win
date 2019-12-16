//using InnoMetricsAPI;
//using InnoMetricsAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemInfo.classes
{
    public class APIClient
    {
        public static String getLoginToken(String username, String password)
        {
            string strUri = ConfigurationSettings.AppSettings["backend-uri"];
            String myToken = String.Empty;
            /*
            Uri endpoint = new Uri(strUri);
            InnoMetricsAPIClient client = new InnoMetricsAPIClient(endpoint, new AnonymousCredential());

            AuthRequest req = new AuthRequest
            {
                Email = username,
                Password = password
            };

            var result = (JObject)client.LoginUsingPOST(req);

            if (result != null)
            {
                myToken = result.GetValue("token").ToString();
            }
            */
            return myToken;
        }
    }
}
