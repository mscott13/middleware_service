namespace middleware_service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.installer = new System.ServiceProcess.ServiceProcessInstaller();
            this.middleware_service = new System.ServiceProcess.ServiceInstaller();
            // 
            // installer
            // 
            this.installer.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.installer.Password = null;
            this.installer.Username = null;
            // 
            // middleware_service
            // 
            this.middleware_service.DelayedAutoStart = true;
            this.middleware_service.Description = "Syncronize databases between ASMS and SAGE";
            this.middleware_service.DisplayName = "Middleware Service";
            this.middleware_service.ServiceName = "middleware_service";
            this.middleware_service.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.middleware_service.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.middleware_service_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.installer,
            this.middleware_service});

        }

        #endregion
        public System.ServiceProcess.ServiceProcessInstaller installer;
        public System.ServiceProcess.ServiceInstaller middleware_service;
    }
}