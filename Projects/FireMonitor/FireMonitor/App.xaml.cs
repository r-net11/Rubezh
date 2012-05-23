using System;
using System.Linq;
using System.Windows;
using Infrastructure.Common.MessageBox;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure;
using Common;

namespace FireMonitor
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Any(item => string.Equals(item, "Integrate", StringComparison.InvariantCultureIgnoreCase)))
			{
				RegistryHelper.Integrate();
				Environment.Exit(1);
			}
			if (e.Args.Any(item => string.Equals(item, "Desintegrate", StringComparison.InvariantCultureIgnoreCase)))
			{
				RegistryHelper.Desintegrate();
				Environment.Exit(1);
			}

#if DEBUG
			//BindingErrorListener.Listen(m => MessageBox.Show(m));
#endif
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
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
			AlarmPlayerHelper.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
			VideoService.Close();
			if (RegistryHelper.IsIntegrated)
				RegistryHelper.ShutDown();
			base.OnExit(e);
		}
	}
}