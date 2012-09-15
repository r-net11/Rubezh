using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace FireMonitor
{
	public partial class App : Application
	{
		private const string SignalId = "{B8150ECC-9433-4535-89AA-5BF6EF631575}";
		private const string WaitId = "{358D5240-9A07-4134-9EAF-8D7A54BCA81F}";
		private Bootstrapper _bootstrapper;

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
			BindingErrorListener.Listen(m => MessageBox.Show(m));
#endif
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
			
			_bootstrapper = new Bootstrapper();
			using (new DoubleLaunchLocker(SignalId, WaitId))
				_bootstrapper.Initialize();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ApplicationService.Invoke(() => MessageBoxService.ShowException(e.ExceptionObject as Exception));
		}
		private void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			foreach (var module in ApplicationService.Modules)
				module.Dispose();
			AlarmPlayerHelper.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
			//VideoService.Close();
			if (RegistryHelper.IsIntegrated)
				RegistryHelper.ShutDown();
		}
	}
}