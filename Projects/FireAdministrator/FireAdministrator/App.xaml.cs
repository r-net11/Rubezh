using System;
using System.Windows;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.MessageBox;
using Common;

namespace FireAdministrator
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

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
			base.OnExit(e);
			FiresecManager.Disconnect();
			VideoService.Close();
		}
	}
}