using System;
using System.Linq;
using System.Windows;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure.Common;

namespace FireMonitor
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Contains("Integrate"))
			{
				RegistryHelper.Integrate();
			}

			if (e.Args.Contains("Desintegrate"))
			{
				RegistryHelper.Desintegrate();
			}

#if ! DEBUG
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

			bootstrapper = new Bootstrapper();
			bootstrapper.Initialize();
		}

		Bootstrapper bootstrapper;

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBoxService.ShowException(e.ExceptionObject as Exception);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			FiresecManager.Disconnect();
			VideoService.Close();
		}
	}
}