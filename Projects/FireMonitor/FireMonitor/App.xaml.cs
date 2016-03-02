using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using System.Threading;
using Infrastructure.Client.Startup;
using System.Globalization;

namespace FireMonitor
{
	public partial class App : Application
	{
		private const string SignalId = "{56E74882-C633-45CD-BE94-DCDEE52846BE}";
		private const string WaitId = "{38A602C9-D6F2-424B-B0FA-97E1B133C473}";
		private Bootstrapper _bootstrapper;
		public bool IsClosingOnException { get; private set; }

		public App()
		{
			IsClosingOnException = false;
		}

		protected virtual Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}
		protected override void OnStartup(StartupEventArgs e)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
			base.OnStartup(e);
			try
			{
				if (CheckIntegrateCommandLineArguments(e.Args))
				{
					Shutdown();
					return;
				}

				ApplicationService.Closing += ApplicationService_Closing;
				ApplicationService.Closed += ApplicationService_Closed;
				ThemeHelper.LoadThemeFromRegister();
#if DEBUG
				bool trace = false;
				BindingErrorListener.Listen(m => { if (trace) MessageBox.Show(m); });
#endif
				_bootstrapper = CreateBootstrapper();
				_bootstrapper.InitializeCommandLineArguments(e.Args);
				bool result;
				using (new DoubleLaunchLocker(SignalId, WaitId, true))
					result = _bootstrapper.Initialize();
				if (!result)
				{
					ApplicationService.ShutDown();
					return;
				}
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

				if (GlobalSettingsHelper.GlobalSettings.RunRevisor)
					StartRevisor();
			}
			catch (StartupCancellationException)
			{
				ApplicationService.ShutDown();
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup");
				MessageBoxService.ShowError("Во время загрузки программы произошло исключение. Приложение будет закрыто");
			}
			finally
			{
				ServiceFactory.StartupService.Close();
			}
		}

		void StartRevisor()
		{
			try
			{
				var path = System.Reflection.Assembly.GetExecutingAssembly();
				RegistrySettingsHelper.SetString("FireMonitorPath", path.Location);
				RegistrySettingsHelper.SetBool("FireMonitor.IsRunning", true);
				RegistrySettingsHelper.SetBool("IsException", false);
				var isAutoConnect = RegistrySettingsHelper.GetBool("isAutoConnect");
				if (isAutoConnect)
				{
					RegistrySettingsHelper.SetBool("isAutoConnect", false);
				}
				RevisorLoadHelper.Load();
			}
			catch (Exception e)
			{
				Logger.Error(e, "App.StartRevisor");
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			IsClosingOnException = true;
			Logger.Error(e.ExceptionObject as Exception, "App.CurrentDomain_UnhandledException");
			_bootstrapper.RestartApplication();
			Environment.Exit(0);
		}

		private void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			AlarmPlayerHelper.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
			if (ShellIntegrationHelper.IsIntegrated && !IsClosingOnException)
				ShellIntegrationHelper.ShutDown();
			RegistrySettingsHelper.SetBool("FireMonitor.IsRunning", false);
		}

		private void ApplicationService_Closed(object sender, EventArgs e)
		{
			Current.Shutdown();
		}

		private bool CheckIntegrateCommandLineArguments(string[] args)
		{
			if (args == null || args.Count() != 1) return false;

			switch (args[0])
			{
				case "/integrate":
					ShellIntegrationHelper.Integrate();
					MessageBox.Show("ОЗ интегрирована");
					return true;

				case "/deintegrate":
					ShellIntegrationHelper.Desintegrate();
					MessageBox.Show("ОЗ деинтегрирована");
					return true;
			}
			return false;
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