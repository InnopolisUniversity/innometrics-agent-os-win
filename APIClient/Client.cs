using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using APIClient.InnoMetricClient;
using APIClient.InnoMetricClient.Models;
using log4net;

namespace APIClient
{
    public static class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string StrUri = ConfigurationManager.AppSettings["backend-uri"];

        public static string GetLoginToken(string username, string password)
        {
            var endpoint = new Uri(StrUri);
            var client = new InnoMetricClient.InnoMetricClient(endpoint, new AnonymousCredential());


            var req = new AuthRequest
            {
                Email = username,
                Password = password
            };

            dynamic result = client.LoginUsingPOST(req);

            if (result != null)
                return result.token.ToString();

            return "";
        }

        public static bool SaveReport(Report report, string token)
        {
            try
            {
                var endpoint = new Uri(StrUri);
                var client = new InnoMetricClient.InnoMetricClient(endpoint, new AnonymousCredential());

                var task = Task.Run(async () => await client.AddReportUsingPOSTWithHttpMessagesAsync(token, report)
                    .ConfigureAwait(false));

                var result = task.Result;

                if (result != null) return result.Response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Log.Debug($"{ex.Message}, {ex.StackTrace}, {ex.Source}");
            }

            return false;
        }

        public static bool SaveProcessReport(AddProcessReportRequest report, string token)
        {
            try
            {
                var endpoint = new Uri(StrUri);
                var client = new InnoMetricClient.InnoMetricClient(endpoint, new AnonymousCredential());

                var task = Task.Run(async () => await client
                    .AddProcessReportUsingPOSTWithHttpMessagesAsync(token, report)
                    .ConfigureAwait(false));

                var result = task.Result;

                if (result != null) return result.Response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
            }

            return false;
        }

        public static UserRequest CreateUser(string name, string surname, string email, string password, string token)
        {
            var endpoint = new Uri(StrUri);
            var client = new InnoMetricClient.InnoMetricClient(endpoint, new AnonymousCredential());

            var req = new UserRequest
            {
                Email = email,
                Name = name,
                Surname = surname,
                Password = password
            };

            var result = client.CreateUserUsingPOST(req, token);

            return result;
        }
    }
}