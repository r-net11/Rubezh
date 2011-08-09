using System;
using System.Windows;
using FiresecClient;

namespace FireAdministrator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if ! DEBUG
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

            Bootsrapper bootstrapper = new Bootsrapper();
            bootstrapper.Run();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            FiresecManager.Disconnect();
        }
    }
}