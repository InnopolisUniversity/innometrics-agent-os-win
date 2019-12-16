namespace InnoMetricsDataSync
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.SrvProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SrvInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SrvProcessInstaller
            // 
            this.SrvProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SrvProcessInstaller.Password = null;
            this.SrvProcessInstaller.Username = null;
            // 
            // SrvInstaller
            // 
            this.SrvInstaller.Description = "it\'s a background process in charge of collecting metrics";
            this.SrvInstaller.DisplayName = "InnoMetricsDataSync";
            this.SrvInstaller.ServiceName = "InnoMetricsDataSync";
            this.SrvInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SrvProcessInstaller,
            this.SrvInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SrvProcessInstaller;
        private System.ServiceProcess.ServiceInstaller SrvInstaller;
    }
}