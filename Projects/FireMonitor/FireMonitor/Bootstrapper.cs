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

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		public bool Initialize()
		{
			var result = true;
			LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect(App.Login, App.Password))
			{
				var userChangedEventArgs = new UserChangedEventArgs
				{
					IsReconnect = false
				};
				ServiceFactory.Events.GetEvent<UserChangedEvent>().Publish(userChangedEventArgs);
				App.Login = ServiceFactory.LoginService.Login;
				App.Password = ServiceFactory.LoginService.Password;
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

		bool IsRestarting = false;

		private void OnConfigurationChanged()
		{
			try
			{
				ServiceFactory.ContentService.Invalidate();
				if (IsRestarting)
					return;
				FiresecManager.FiresecService.SuspendPoll = true;
				if (FiresecManager.FSAgent != null)
					FiresecManager.FSAgent.SuspendPoll = true;
				LoadingErrorManager.Clear();
				IsRestarting = true;
				ProgressWatcher.Close();
				ApplicationService.Restart();

				LoadingService.Show("Перезагрузка конфигурации", "Перезагрузка конфигурации", 10);
				LoadingService.AddCount(10);

				LoadingService.DoStep("Загрузка конфигурации с сервера");
				FiresecManager.GetConfiguration("Monitor/Configuration");

				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();

				BeforeInitialize(false);
				InitializeModules();
				if (ApplicationService.Modules.Any(x => x.Name == "Устройства и Зоны"))
					ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
				else if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Guid.Empty);
				AterInitialize();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Bootstrapper.OnConfigurationChanged");
			}
			finally
			{
				LoadingService.Close();
				IsRestarting = false;
				FiresecManager.FiresecService.SuspendPoll = false;
				if (FiresecManager.FSAgent != null)
					FiresecManager.FSAgent.SuspendPoll = false;
			}
		}

		private void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			Application.Current.Shutdown();
		}
	}
}