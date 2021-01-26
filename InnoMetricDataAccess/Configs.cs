namespace InnoMetricDataAccess
{
    internal class Configs
    {
        public static string GetConnectionString()
        {
//#if DEBUG
            //return @"Data Source=.\CollectorDB.db; Version=3";
//#else
            return @"Data Source=C:\TMP\InnoMetrics\db\CollectorDB.db; Version=3";
//#endif
        }
    }
}