using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.Windows.Threading;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		private string _login;
		private string _password;

		public bool Initialize()
		{
			var result = true;
			LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect(_login, _password))
			{
				var userChangedEventArgs = new UserChangedEventArgs
				{
					IsReconnect = false
				};
				ServiceFactory.Events.GetEvent<UserChangedEvent>().Publish(userChangedEventArgs);
				_login = ServiceFactory.LoginService.Login;
				_password = ServiceFactory.LoginService.Password;
				try
				{
					CreateModules();

					LoadingService.ShowLoading("Чтение конфигурации", 15);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					LoadingService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration("Monitor/Configuration");

					GKDriversCreator.Create();
					BeforeInitialize(true);

					LoadingService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll();

					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
					{
						LoadingService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();
						Notifier.Initialize();

						result = Run();
						SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					LoadingService.Close();

					if (result)
						AterInitialize();

					//MutexHelper.KeepAlive();
					ProgressWatcher.Run();
					if (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost")
					{
						RegistrySettingsHelper.SetBool("isException", true);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.InitializeFs");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return false;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return false;
			}
			return result;
		}
		protected virtual bool Run()
		{
			var result = true;
			var shell = CreateShell();
			((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
			if (!RunShell(shell))
				result = false;
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());
			return result;
		}
		protected virtual ShellViewModel CreateShell()
		{
			return new MonitorShellViewModel();
		}

		protected virtual void OnConfigurationChanged()
		{
			var restartView = new RestartApplicationViewModel();
			var isRestart = DialogService.ShowModalWindow(restartView);
			if (isRestart)
				Restart();
			else
			{
				var timer = new DispatcherTimer();
				timer.Tick += (s, e) =>
				{
					timer.Stop();
					Restart();
				};
				timer.Interval = TimeSpan.FromSeconds(restartView.Total);
				timer.Start();
			}
		}
		private void Restart()
		{
			using (new WaitWrapper())
			{
				ApplicationService.ApplicationWindow.IsEnabled = false;
				ServiceFactory.ContentService.Invalidate();
				FiresecManager.FiresecService.StopPoll();
				if (FiresecManager.FSAgent != null)
					FiresecManager.FSAgent.Stop();
				LoadingErrorManager.Clear();
				ProgressWatcher.Close();
				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();
				ApplicationService.ShutDown();
			}
			RestartApplication();
		}

		public void RestartApplication()
		{
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
				Arguments = GetRestartCommandLineArguments()
			};
			System.Diagnostics.Process.Start(processStartInfo);
		}
		protected virtual string GetRestartCommandLineArguments()
		{
			string commandLineArguments = null;
			if (_login != null && _password != null)
				commandLineArguments = "login='" + _login + "' password='" + _password + "'";
			return commandLineArguments;
		}
		public virtual void InitializeCommandLineArguments(string[] args)
		{
			if (args != null)
			{
				if (args.Count() >= 2)
				{
					foreach (var arg in args)
					{
						if (arg.StartsWith("login='") && arg.EndsWith("'"))
						{
							_login = arg.Replace("login='", "");
							_login = _login.Replace("'", "");
						}
						if (arg.StartsWith("password='") && arg.EndsWith("'"))
						{
							_password = arg.Replace("password='", "");
							_password = _password.Replace("'", "");
						}
					}
				}
			}
		}
	}
}