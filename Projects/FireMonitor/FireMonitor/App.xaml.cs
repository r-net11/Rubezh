using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using GKProcessor;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using MuliclientAPI;

namespace FireMonitor
{
	public partial class App : Application
	{
		const string SignalId = "{B8150ECC-9433-4535-89AA-5BF6EF631575}";
		const string WaitId = "{358D5240-9A07-4134-9EAF-8D7A54BCA81F}";
		Bootstrapper Bootstrapper;
		public static bool IsClosingOnException = false;
		public static string Login;
		public static string Password;
		public static bool IsMulticlient { get; private set; }
		public static string MulticlientId { get; private set; }

		public void SetMulticlientData(MulticlientData multiclientData)
		{
			IsMulticlient = true;
			MulticlientId = multiclientData.Id;
			Login = multiclientData.Login;
			Password = multiclientData.Password;
			ConnectionSettingsManager.RemoteAddress = multiclientData.Address;
			ConnectionSettingsManager.RemotePort = multiclientData.Port;
		}

		public App()
		{
			IsMulticlient = false;
			PatchManager.Patch();
		}

		protected virtual Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			try
			{
				if (CheckIntegrateCommandLineArguments(e.Args))
				{
					Shutdown();
					return;
				}
				InitializeCommandLineArguments(e.Args);

				ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
				ThemeHelper.LoadThemeFromRegister();
#if DEBUG
				bool trace = false;
				BindingErrorListener.Listen(m => { if (trace) MessageBox.Show(m); });
#endif
				Bootstrapper = CreateBootstrapper();
				var result = true;
				using (new DoubleLaunchLocker(SignalId, WaitId, true, !IsMulticlient))
				{
					result = Bootstrapper.Initialize();
				}
				if (!result)
				{
					ApplicationService.ShutDown();
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup");
				MessageBoxService.ShowError("Во время загрузки программы произошло исключение. Приложение будет закрыто");
			}

			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			if (GlobalSettingsHelper.GlobalSettings.RunRevisor)
				StartRevisor();
			if (IsMulticlient)
				MulticlientController.Current.SuscribeMulticlientStateChanged();
		}

		void StartRevisor()
		{
#if DEBUG
			return;
#endif
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
			Restart();
			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}
		void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			AlarmPlayerHelper.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
			if (ShellIntegrationHelper.IsIntegrated)
			{
				if (!IsClosingOnException)
					ShellIntegrationHelper.ShutDown();
			}
			RegistrySettingsHelper.SetBool("FireMonitor.IsRunning", false);

			Environment.Exit(0);
		}

		void Restart()
		{
#if DEBUG
			return;
#endif
			string commandLineArguments = null;
			if (Login != null && Password != null)
			{
				commandLineArguments = "login='" + Login + "' password='" + Password + "'";
			}
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
				Arguments = commandLineArguments
			};
			System.Diagnostics.Process.Start(processStartInfo);
		}

		void InitializeCommandLineArguments(string[] args)
		{
			if (args != null)
			{
				if (args.Count() >= 2)
				{
					foreach (var arg in args)
					{
						if (arg.StartsWith("login='") && arg.EndsWith("'"))
						{
							Login = arg.Replace("login='", "");
							Login = Login.Replace("'", "");
						}
						if (arg.StartsWith("password='") && arg.EndsWith("'"))
						{
							Password = arg.Replace("password='", "");
							Password = Password.Replace("'", "");
						}
					}
				}
			}
		}
		bool CheckIntegrateCommandLineArguments(string[] args)
		{
			if (args != null)
			{
				if (args.Count() == 1)
				{
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
				}
			}
			return false;
		}
	}
}