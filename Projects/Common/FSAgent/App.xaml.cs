using System.Windows;
using FSAgent.Service;
using System;
using Common;
using System.Diagnostics;

namespace FSAgent
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
            Application.Current.MainWindow.Close();
            Application.Current.Shutdown();
        }

        void Restart()
        {
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = Application.ResourceAssembly.Location,
            };
            System.Diagnostics.Process.Start(processStartInfo);
        }
    }
}