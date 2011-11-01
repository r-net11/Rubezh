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

            var bootstrapper = new Bootsrapper();
            bootstrapper.Initialize();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DialogBox.DialogBox.Show(e.ExceptionObject.ToString());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            FiresecManager.Disconnect();
        }
    }
}