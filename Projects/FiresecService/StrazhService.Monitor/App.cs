using System;
using System.Windows;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using KeyGenerator;

namespace StrazhService.Monitor
{
	public class App : Application
	{
		private const string SignalId = "{950CD21B-B2C2-4B6E-A620-F2A5810F609B}";
		private const string WaitId = "{04B5ECAB-FE32-4BC7-A90A-8D619B6C4D74}";

		[STAThread]
		private static void Main()
		{
			var app = new App();
			Current.Resources.MergedDictionaries.Add(LoadComponent(new Uri("Controls;component/Themes/Styles.xaml", UriKind.Relative)) as ResourceDictionary);
			app.Run();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();

			var licenseService = new LicenseManager();

			if (!licenseService.IsValidExistingKey())
			{
				Logger.Error("Лицензия отсутствует");
			}

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
				Bootstrapper.Instance.Run(licenseService);
			}
		}

		private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			Bootstrapper.Instance.Close();
			Current.MainWindow.Close();
			Current.Shutdown();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			
			base.OnExit(e);
		}
	}
}