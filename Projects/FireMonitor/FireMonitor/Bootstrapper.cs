using System;
using System.Linq;
using System.Windows;
using AlarmModule;
using AlarmModule.Events;
using Common;
using Infrastructure.Common.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Client;
using FireMonitor.Views;

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

			ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			var preLoadWindow = new PreLoadWindow();
			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				preLoadWindow.PreLoadText = "Инициализация компонент...";
				preLoadWindow.Show();
				FiresecManager.GetConfiguration();
				if (FiresecManager.Drivers.Count == 0)
				{
					MessageBoxService.Show("Ошибка при получении списка драйверов с сервера");
				}
				FiresecManager.GetStates();

				var operationResult = FiresecManager.FiresecService.CheckHaspPresence();
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
				}
				var serverStatus = FiresecManager.FiresecService.GetStatus();
				if (serverStatus != null)
				{
					MessageBoxService.ShowWarning(serverStatus);
				}

				if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login))
				{
					ClientSettings.LoadSettings();

					var shellView = new ShellView();
					ServiceFactory.ShellView = shellView;

					//if (ServiceFactory.AppSettings.ShowOnlyVideo)
					//{
					//    var alarmVideoWather = new AlarmVideoWather();
					//    preLoadWindow.Close();
					//    return;
					//}

					InitializeModules();
					shellView.Navigation = GetNavigationItems();

					App.Current.MainWindow = shellView;
					App.Current.MainWindow.Show();
					FiresecCallbackService.ConfigurationChangedEvent += new Action(OnConfigurationChanged);
				}
				else
				{
					MessageBoxService.Show("Нет прав на работу с программой");
					FiresecManager.Disconnect();
				}

				preLoadWindow.Close();
			}
			else
			{
				Application.Current.Shutdown();
			}

			MutexHelper.KeepAlive();
		}

		void OnConfigurationChanged()
		{
			ServiceFactory.Layout.Close();

			//FiresecManager.FiresecService.StopPing();

			FiresecManager.GetConfiguration(false);
			FiresecManager.DeviceStates = FiresecManager.FiresecService.GetStates(true);
			FiresecManager.UpdateStates();

			//FiresecManager.FiresecService.StartPing();

			ServiceFactory.ShellView.Dispatcher.Invoke(new Action(() => { InitializeModules(); }));
		}
	}
}