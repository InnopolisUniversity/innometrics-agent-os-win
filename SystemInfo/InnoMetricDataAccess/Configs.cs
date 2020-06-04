using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace InnoMetricDataAccess
{
    class Configs
    {
        public static String getConnectionString()
        {
#if DEBUG
            //return @"Data Source=.\db\CollectorDB.db; Version=3";
            return @"Data Source=.\CollectorDB.db; Version=3";
#else
            //return @"Data Source=%AppData%\InnoMetrics\db\CollectorDB.db; Version=3";
            return @"Data Source=C:\TMP\InnoMetrics\db\CollectorDB.db; Version=3";
#endif
        }

        //You can either provide User name or SID
        public string GetUserProfilePath(string userName, string userSID = null)
        {
            try
            {
                if (userSID == null)
                {
                    userSID = GetUserSID(userName);
                }

                var keyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\" + userSID;

                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath);
                if (key == null)
                {
                    //handle error
                    return null;
                }

                var profilePath = key.GetValue("ProfileImagePath") as string;

                return profilePath;
            }
            catch
            {
                //handle exception
                return null;
            }
        }

        public string GetUserSID(string userName)
        {
            try
            {
                NTAccount f = new NTAccount(userName);
                SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
                return s.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
