using System;
using System.Linq;
using System.Windows;
using AlarmModule;
using AlarmModule.Events;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Client;
using FireMonitor.Views;
using Infrastructure.Common.Windows;
using FireMonitor.ViewModels;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			if (!MutexHelper.IsNew("FireAdministrator"))
			{
				MessageBoxService.ShowWarning("Другой экзэмпляр приложения уже запущен. Приложение будет закрыто");
				Application.Current.Shutdown();
				System.Environment.Exit(1);
				return;
			}

			AppConfigHelper.InitializeAppSettings();
			VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				LoadingService.Show("Чтение конфигурации", 9);
				LoadingService.AddCount(GetModuleCount());
				LoadingService.DoStep("Полчучение списка драйверов с сервера");
				FiresecManager.GetConfiguration();
				if (FiresecManager.Drivers.Count == 0)
					MessageBoxService.Show("Ошибка при получении списка драйверов с сервера");
				LoadingService.DoStep("Проверка состояния системы");
				FiresecManager.GetStates();

				LoadingService.DoStep("Проверка HASP-ключа");
				var operationResult = FiresecManager.FiresecService.CheckHaspPresence();
				if (operationResult.HasError)
					MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
				LoadingService.DoStep("Проверка состояния системы");
				var serverStatus = FiresecManager.FiresecService.GetStatus();
				if (serverStatus != null)
					MessageBoxService.ShowWarning(serverStatus);

				LoadingService.DoStep("Проверка прав пользователя");
				if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login))
				{
					LoadingService.DoStep("Загрузка клиентских настроек");
					ClientSettings.LoadSettings();

					//if (ServiceFactory.AppSettings.ShowOnlyVideo)
					//{
					//    var alarmVideoWather = new AlarmVideoWather();
					//    preLoadWindow.Close();
					//    return;
					//}

					var shell = new MonitorShellViewModel();
					((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
					RunShell(shell);

					FiresecCallbackService.ConfigurationChangedEvent += new Action(OnConfigurationChanged);
				}
				else
				{
					MessageBoxService.Show("Нет прав на работу с программой");
					FiresecManager.Disconnect();
				}
				LoadingService.Close();
			}
			else
				Application.Current.Shutdown();
			MutexHelper.KeepAlive();
		}

		private void OnConfigurationChanged()
		{
			ServiceFactory.Layout.Close();

			FiresecManager.GetConfiguration(false);
			FiresecManager.DeviceStates = FiresecManager.FiresecService.GetStates(true);
			FiresecManager.UpdateStates();

			ServiceFactory.SafeCall(InitializeModules);
		}
	}
}