using System;
using System.Windows;
using Common;
using FiresecService;
using Infrastructure.Common;
using System.ServiceProcess;
using System.Diagnostics;
using System.Threading;

namespace FiresecServiceRunner
{
	public partial class App : Application
	{
		private const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
		private const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			using (new DoubleLaunchLocker(SignalId, WaitId, true))
				Bootstrapper.Run();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "Исключение при вызове App.CurrentDomain_UnhandledException");
		}
	}
}