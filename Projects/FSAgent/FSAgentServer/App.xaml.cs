using System.Windows;
using System;
using Common;
using System.Diagnostics;

namespace FSAgentServer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Bootstrapper.Run();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject as Exception, "App.CurrentDomain_UnhandledException");
            Restart();
        }

        public static void Restart()
        {
			Logger.Error("App.Restart");
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = Application.ResourceAssembly.Location,
            };
            System.Diagnostics.Process.Start(processStartInfo);

			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
        }
    }
}