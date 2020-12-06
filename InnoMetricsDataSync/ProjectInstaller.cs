using System.ComponentModel;
using System.Configuration.Install;

namespace InnoMetricsDataSync
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}