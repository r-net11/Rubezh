using System;
using System.Linq;
using System.Windows;
using AlarmModule.Events;
using FireMonitor.ViewModels;
using Firesec;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Common.GK;

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
					LoadingService.Show("Чтение конфигурации", 15);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();
					InitializeFs(false);
					LoadingService.DoStep("Загрузка конфигурации ГК");
					InitializeGk();
					
					LoadingService.DoStep("Старт полинга сервера");
					FiresecManager.StartPing();

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


						//FiresecCallbackService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
                        SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
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
			ServiceFactory.SubscribeEvents();
		}

		void InitializeFs(bool reconnect = false)
		{
			LoadingService.DoStep("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration();

            if (!reconnect)
            {
                LoadingService.DoStep("Инициализация драйвера устройств");
                FiresecManager.InitializeFiresecDriver(ServiceFactory.AppSettings.FS_Address, ServiceFactory.AppSettings.FS_Port, ServiceFactory.AppSettings.FS_Login, ServiceFactory.AppSettings.FS_Password);
                LoadingService.DoStep("Старт мониторинга");
                FiresecManager.StartWatcher(true, true);
                LoadingService.DoStep("Синхронизация журнала событий");
                FiresecManager.SynchrinizeJournal();
            }

            LoadingService.DoStep("Синхронизация конфигурации");
            FiresecManager.Synchronyze();
		}

		void InitializeGk()
		{
			GKDriversCreator.Create();
			XManager.GetConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();
		}

		void OnConfigurationChanged()
		{
			LoadingService.Show("Перезагрузка конфигурации", 10);
			LoadingService.AddCount(10);

			ApplicationService.CloseAllWindows();
			ServiceFactory.Layout.Close();

			//ServiceFactory.SafeCall(() =>
			//{
                InitializeFs(true);
                InitializeGk();

				//var progressInfoViewModel = new ProgressInfoViewModel();
				//DialogService.ShowWindow(progressInfoViewModel);
				InitializeModules();
				//progressInfoViewModel.Close();
				ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
			//});

            LoadingService.Close();
		}

		void OnNotify(string message)
		{
			MessageBoxService.Show(message);
		}
	}
}