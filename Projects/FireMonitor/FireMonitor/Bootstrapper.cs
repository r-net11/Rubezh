using System;
using System.Linq;
using System.Windows;
using AlarmModule.Events;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			AppConfigHelper.InitializeAppSettings();
			VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect())
				try
				{
					LoadingService.Show("Чтение конфигурации", 9);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration();
					if (FiresecManager.Drivers.Count == 0)
						MessageBoxService.Show("Ошибка при загрузке конфигурации с сервера");
					LoadingService.DoStep("Загрузка состояний с сервера");
					FiresecManager.GetStates();

					//LoadingService.DoStep("Проверка HASP-ключа");
					//var operationResult = FiresecManager.FiresecService.CheckHaspPresence();
					//if (operationResult.HasError)
					//    MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
					//LoadingService.DoStep("Проверка статуса сервера");
					//var serverStatus = FiresecManager.FiresecService.GetStatus();
					//if (serverStatus != null)
					//    MessageBoxService.ShowWarning(serverStatus);

					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login))
					{
						LoadingService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();

						var shell = new MonitorShellViewModel();
						((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
						RunShell(shell);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());


						FiresecCallbackService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
						ServiceFactory.Events.GetEvent<NotifyEvent>().Subscribe(OnNotify);
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					LoadingService.Close();
				}
				catch (Exception ex)
				{
					MessageBoxService.ShowException(ex);
					Application.Current.Shutdown();
				}
			else
				Application.Current.Shutdown();
			MutexHelper.KeepAlive();
		}

		void OnConfigurationChanged()
		{
			ApplicationService.CloseAllWindows();
			ServiceFactory.Layout.Close();

			FiresecManager.GetConfiguration(false);
			FiresecManager.DeviceStates = FiresecManager.FiresecService.GetStates(true);
			FiresecManager.UpdateStates();

			ServiceFactory.SafeCall(() =>
			{
				var progressInfoViewModel = new ProgressInfoViewModel();
				DialogService.ShowWindow(progressInfoViewModel);
				InitializeModules();
				progressInfoViewModel.Close();
				ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
			});
		}

		void OnNotify(string message)
		{
			MessageBoxService.Show(message);
		}
	}
}