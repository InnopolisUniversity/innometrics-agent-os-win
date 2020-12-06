using System.ServiceProcess;

namespace InnoMetricsDataSync
{
    internal static class Program
    {
        /// <summary>
        ///     Punto de entrada principal para la aplicación.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new InnoMetricsDataSync()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}