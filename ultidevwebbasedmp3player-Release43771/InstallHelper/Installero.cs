using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

using WebServer = CassiniConfiguration;

namespace InstallHelper
{
    [RunInstaller(true)]
    public partial class Installero : System.Configuration.Install.Installer
    {
        static readonly Guid webAppID = new Guid("{F41FA134-8532-4A07-89A2-92209B9C727B}");
        const string appName = "MP3 Music Player by UltiDev";
        const string appDesc = "A simple Flash-based MP3-only (no WMA) music player.";
        const string webAppDir = "WebApp";
        const string hvpnaFile = "UltiDevMP3Player.hvpna";

        const string httpVpnRegPath = @"UltiDev\HttpVPNProxy\Plugins\MyOwnSecureWeb.com\AgentPluginInstallUtility.exe";

        public Installero()
        {
            InitializeComponent();
        }

        private string TargetDir
        {
            get { return this.Context.Parameters["AppLocation"]; }
        }

        private string WebAppDir
        {
            get { return Path.Combine(this.TargetDir, webAppDir); }
        }

        private Guid HttpVpnSKU
        {
            get { return new Guid(this.Context.Parameters["HttpVpnSKU"]); }
        }

        private static string GetBaseUrl(int port)
        {
            return string.Format("http://{0}:{1}/", Environment.MachineName, port);
        }

        private static string HttpVpnRegPath
        {
            get
            {
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                return System.IO.Path.Combine(programFilesPath, httpVpnRegPath);
            }
        }

        private string MakeSkuParm()
        {
            return "/SKU=" + this.HttpVpnSKU.ToString().ToUpper();
        }

        private string MakeAppUrlPart(int port)
        {
            return "/AppUrl=" + GetBaseUrl(port);
        }

        private void RegisterWebAppWithHttpVpn(int port)
        {
            string cmdLineArgs = string.Format("{0} {1}", 
                                    this.MakeSkuParm(), this.MakeAppUrlPart(port));
            
            Process.Start(HttpVpnRegPath, cmdLineArgs);
        }

        private void UpdateHvpnaFile(int port)
        {
            string hvpnaFilePath = Path.Combine(this.TargetDir, hvpnaFile);

            using (StreamWriter writer = new StreamWriter(hvpnaFilePath, false))
            {
                writer.WriteLine(this.MakeSkuParm());
                writer.WriteLine(this.MakeAppUrlPart(port));
            }
        }

        private static bool NeedRegisterWithCassini(Guid appID, string newWebAppFolder, out int port)
        {
            port = 0;

            WebServer.Metabase mb = WebServer.Metabase.Load();
            WebServer.ApplicationEntry appInfo = mb.FindApplication(appID);
            if (appInfo == null)
                // App is not currently registered.
                return true; // tell to register it for the first time on any port.

            port = appInfo.Port;

            DirectoryInfo currentDir = new DirectoryInfo(appInfo.PhysicalPath);
            DirectoryInfo newDir = new DirectoryInfo(newWebAppFolder);
            if (currentDir.FullName.ToLower() == newDir.FullName.ToLower())
                // app is present and is in the same fodler as before upgrade,
                // so no need to register.
                return false;

            // App exists, but was installed into a new fodler, 
            // so it needs to be re-registered, preferrably on the same port as before.
            return true;
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            int port;

            if (NeedRegisterWithCassini(webAppID, this.WebAppDir, out port))
            {
                // Register the app with UltiDev Cassini
                WebServer.ApplicationEntry appInfo = WebServer.Metabase.RegisterApplication(webAppID,
                        appName, port, appDesc, this.WebAppDir, "Default.aspx", false);

                port = appInfo.Port;
            }

            base.Install(stateSaver);

            // Create app registration file to enable future (re)registrations.
            this.UpdateHvpnaFile(port);

            // Register web app with UltiDev HttpVPN Portal.
            this.RegisterWebAppWithHttpVpn(port);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            WebServer.Metabase.UnregisterApplication(webAppID);
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            base.Rollback(savedState);

            WebServer.Metabase.UnregisterApplication(webAppID);
        }
    }
}