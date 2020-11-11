using InnoMetric;
using InnoMetric.Models;
using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            try {
                Uri endpoint = new Uri(strUri);
                InnoMetricClient client = new InnoMetricClient(endpoint, new AnonymousCredential());

                var task = Task.Run(async () => await client.AddReportUsingPOSTWithHttpMessagesAsync(token, report)
                                                            .ConfigureAwait(false));

                var result = task.Result;

                if (result != null)
                {
                    return result.Response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch(Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }
            
            return false;
        }

        public static Boolean SaveProcessReport(AddProcessReportRequest report, String token)
        {
            try
            {
                Uri endpoint = new Uri(strUri);
                InnoMetricClient client = new InnoMetricClient(endpoint, new AnonymousCredential());

                var task = Task.Run(async () => await client.AddProcessReportUsingPOSTWithHttpMessagesAsync(token, report)
                                                            .ConfigureAwait(false));

                var result = task.Result;

                if (result != null)
                {
                    return result.Response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }

            return false;
        }

        public static UserRequest createUser(String name, String surname, String email, String password, String token)
        {
            Uri endpoint = new Uri(strUri);
            InnoMetricClient client = new InnoMetricClient(endpoint, new AnonymousCredential());

            UserRequest req = new UserRequest
            {
                Email = email,
                Name = name,
                Surname = surname,
                Password = password
            };

            var result = (UserRequest)client.CreateUserUsingPOST(req, token);

            return result;
        }
    }
}
