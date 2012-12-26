using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using Common.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using Microsoft.Win32;

namespace FireMonitor
{
	public partial class App : Application
	{
		const string SignalId = "{B8150ECC-9433-4535-89AA-5BF6EF631575}";
		const string WaitId = "{358D5240-9A07-4134-9EAF-8D7A54BCA81F}";
		Bootstrapper _bootstrapper;
		public static bool IsClosingOnException = false;
		public static string Login;
		public static string Password;
		public bool IsMulticlient { get; set; }

		public App()
		{
			IsMulticlient = false;
		}

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
#if DEBUG
                AppSettingsManager.AutoConnect = true;
#endif
                InitializeCommandLineArguments(e.Args);
                if (MulticlientHelper.IsMulticlient)
                    MulticlientHelper.Start();

                ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
                ThemeHelper.LoadThemeFromRegister();
#if DEBUG
                bool trace = false;
                BindingErrorListener.Listen(m => { if (trace) MessageBox.Show(m); });
#endif
                _bootstrapper = new Bootstrapper(IsMulticlient);
                using (new DoubleLaunchLocker(SignalId, WaitId, true, !MulticlientHelper.IsMulticlient && !IsMulticlient))
                    _bootstrapper.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "App.OnStartup");
                MessageBoxService.ShowError("Во время загрузки программы произошло исключение. Приложение будет закрыто");
            }

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (!MulticlientHelper.IsMulticlient)
                StartRevisor();
        }

		void StartRevisor()
		{
            try
            {
                var path = System.Reflection.Assembly.GetExecutingAssembly();
                var saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
                if (saveKey != null)
                {
                    saveKey.SetValue("FireMonitorPath", path.Location);
                    saveKey.SetValue("IsException", false);
                    var isAutoConnect = saveKey.GetValue("isAutoConnect");
                    if (isAutoConnect != null)
                        if (isAutoConnect.Equals("True"))
                        {
                            AppSettingsManager.AutoConnect = true;
                            saveKey.SetValue("isAutoConnect", false);
                        }
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

#if RELEASE
                Restart();
#endif
            Application.Current.MainWindow.Close();
            Application.Current.Shutdown();
        }

		void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			GKDBHelper.AddMessage("Выход пользователя из системы");
			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			AlarmPlayerHelper.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
			if (RegistryHelper.IsIntegrated)
			{
				if (!IsClosingOnException)
					RegistryHelper.ShutDown();
			}
		}

		void Restart()
		{
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
						if (arg.StartsWith("regime='") && arg.EndsWith("'"))
						{
							var regime = arg.Replace("regime='", "");
							regime = regime.Replace("'", "");
							if (regime == "multiclient")
							{
								MulticlientHelper.IsMulticlient = true;
							}
						}
						if (arg.StartsWith("ClientId='") && arg.EndsWith("'"))
						{
							MulticlientHelper.MulticlientClientId = arg.Replace("ClientId='", "");
							MulticlientHelper.MulticlientClientId = MulticlientHelper.MulticlientClientId.Replace("'", "");
						}
					}
				}
			}
		}
	}
}