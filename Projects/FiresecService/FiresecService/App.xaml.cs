using System;
using System.Windows;
using Controls.MessageBox;
using Common;

namespace FiresecServiceRunner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
			Logger.Info("Firesec Service Startup");
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
		protected override void OnExit(ExitEventArgs e)
		{
			Logger.Info("Firesec Service Exit");
			base.OnExit(e);
		}

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
			Logger.Error((Exception)e.ExceptionObject);
            //MessageBoxService.Show(e.ExceptionObject.ToString());
        }
    }
}