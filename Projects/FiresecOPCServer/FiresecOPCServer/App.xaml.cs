using System.Windows;
using Infrastructure.Common.Windows;
using System.ComponentModel;
using FiresecClient;
using System;
using Common;

namespace FiresecOPCServer
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Bootstrapper.Run();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject as Exception);
        }
    }
}