using System;
using System.Threading;
using System.Windows.Forms;

namespace DataCollectorUI
{
    internal static class Program
    {
        private static readonly string appGuid = "82e37588-392a-4d6e-bb2a-7c33d96950f8";

        /// <summary>
        ///     Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Instance already running", "InnoMetrics data collector", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
        }
    }
}