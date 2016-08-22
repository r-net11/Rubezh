using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using Localization.FireAdministrator.Errors;

namespace FireAdministrator
{
	public partial class App : Application
	{
		private const string SignalId = "{134E850D-5534-4BE6-92E1-C0698375678B}";
		private const string WaitId = "{41D511C6-4F6E-442E-8B2C-50BB3FCE6A32}";
		private Bootstrapper _bootstrapper;

		protected override void OnStartup(StartupEventArgs e)
		{

            var culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
			base.OnStartup(e);
			try
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				ApplicationService.Closing += ApplicationService_Closing;
				ThemeHelper.LoadThemeFromRegister();
#if DEBUG
				bool trace = false;
				BindingErrorListener.Listen(m => { if (trace) MessageBox.Show(m); });
#endif
				_bootstrapper = new Bootstrapper();
				_bootstrapper.Initialize();

				if (Current != null && e.Args.Length > 0)
				{
					FileConfigurationHelper.LoadFromFile(e.Args[0]);
				}
			}
			catch (StartupCancellationException)
			{
				ApplicationService.ShutDown();
			}
			finally
			{
				ServiceFactory.StartupService.Close();
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBoxService.ShowException(e.ExceptionObject as Exception);
			if (MessageBoxService.ShowQuestion(CommonErrors.CloseWithException_Error))
			{
				FileConfigurationHelper.SaveToFile();
			}
		}
		private void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
		}

		[STAThread]
		private static void Main()
		{
			ServiceFactory.StartupService.Run();
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}