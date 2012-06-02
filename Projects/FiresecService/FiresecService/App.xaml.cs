using System;
using System.Windows;
using Common;
using FiresecService;

namespace FiresecServiceRunner
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Bootstrapper.Run();
		}

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
			Logger.Error((Exception)e.ExceptionObject, "Исключение при вызове App.CurrentDomain_UnhandledException");
		}
	}
}