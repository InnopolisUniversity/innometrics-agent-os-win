using InnoMetric;
using InnoMetric.Models;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APIClient
{
    public class Client
    {
        private static string strUri = ConfigurationManager.AppSettings["backend-uri"];////ConfigurationSettings.AppSettings["backend-uri"];
        public static String getLoginToken(String username, String password)
        {
            String myToken = String.Empty;

            Uri endpoint = new Uri(strUri);
            InnoMetricClient client = new InnoMetricClient(endpoint, new AnonymousCredential());

            AuthRequest req = new AuthRequest
            {
                Email = username,
                Password = password
            };

            var result = (JObject)client.LoginUsingPOST(req);

            var result2 = client.LoginUsingPOST(req);

            if (result != null)
            {
                myToken = result.GetValue("token").ToString();
            }

            return myToken;
        }

        public static Boolean SaveReport(Report report, String token)
        {
            Uri endpoint = new Uri(strUri);
            InnoMetricClient client = new InnoMetricClient(endpoint, new AnonymousCredential());

            var task = Task.Run(async () => await client.AddReportUsingPOSTWithHttpMessagesAsync(token, report)
                                                        .ConfigureAwait(false));

            var result = task.Result;

            if (result != null)
            {
                return result.Response.StatusCode == HttpStatusCode.OK;
            }
            return false;
        }
    }
}
