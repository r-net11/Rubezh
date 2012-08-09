using System;
using System.ComponentModel;
using System.Windows;
using Common;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace FireAdministrator
{
	public partial class App : Application
	{
		private const string SignalId = "{8599F876-2147-4694-A822-B24E36D7F92F}";
		private const string WaitId = "{07193C2C-CE04-478C-880A-49AB239C6550}";
		Bootstrapper bootstrapper;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

#if DEBUG
			//BindingErrorListener.Listen(m => MessageBox.Show(m));
#endif

			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);

			bootstrapper = new Bootstrapper();
			using (new DoubleLaunchLocker(SignalId, WaitId))
				bootstrapper.Initialize();
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBoxService.ShowException(e.ExceptionObject as Exception);
		}
		private void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			Logger.Info("App.OnExit");
			FiresecManager.Disconnect();
			VideoService.Close();
		}
	}
}