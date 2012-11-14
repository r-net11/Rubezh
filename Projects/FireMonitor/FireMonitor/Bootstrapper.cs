using System;
using System.Linq;
using System.Windows;
using FireMonitor.ViewModels;
using Firesec;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Common.Theme;
using Common.GK;
using Microsoft.Win32;
using Common;
using FiresecAPI;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
            LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect(App.Login, App.Password))
			{
				App.Login = ServiceFactory.LoginService.Login;
				App.Password = ServiceFactory.LoginService.Password;
				try
				{
					LoadingService.Show("Чтение конфигурации", 15);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();
					InitializeFs(false);
                    if (LoadingErrorManager.HasError)
					{
                        MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке драйвера FireSec");
					}
					LoadingService.DoStep("Загрузка конфигурации ГК");
					InitializeGk();

					LoadingService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll(false);
#if RELEASE
                    LoadingService.DoStep("Проверка HASP-ключа");
                    var operationResult = FiresecManager.FiresecDriver.CheckHaspPresence();
                    if (operationResult.HasError)
                        MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
#endif
					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
					{
						LoadingService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();

						var shell = new MonitorShellViewModel();
						((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
						RunShell(shell);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());

						SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
						ServiceFactory.Events.GetEvent<NotifyEvent>().Subscribe(OnNotify);
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					LoadingService.Close();
					GKDBHelper.AddMessage("Вход пользователя в систему");
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.InitializeFs");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return;
			}
			//MutexHelper.KeepAlive();
			ServiceFactory.SubscribeEvents();
			ProgressWatcher.Run();
            FiresecManager.FiresecDriver.FiresecSerializedClient.NativeFiresecClient.StartPingThread();
			ServiceFactory.Events.GetEvent<BootstrapperInitializedEvent>().Publish(null);
		}

		void InitializeFs(bool reconnect = false)
		{
			try
			{
				LoadingService.DoStep("Загрузка конфигурации с сервера");
				FiresecManager.GetConfiguration();

				if (!reconnect)
				{
					LoadingService.DoStep("Инициализация драйвера устройств");
                    var connectionResult = FiresecManager.InitializeFiresecDriver(AppSettingsManager.FS_Address, AppSettingsManager.FS_Port, AppSettingsManager.FS_Login, AppSettingsManager.FS_Password, true);
					if (connectionResult.HasError)
					{
						CloseOnException(connectionResult.Error);
						return;
					}
				}
				LoadingService.DoStep("Синхронизация конфигурации");
				FiresecManager.FiresecDriver.Synchronyze();
				LoadingService.DoStep("Старт мониторинга");
				FiresecManager.FiresecDriver.StartWatcher(true, true);
				if (!reconnect)
				{
					LoadingService.DoStep("Синхронизация журнала событий");
					FiresecManager.SynchrinizeJournal();
				}
			}
			catch (FiresecException e)
			{
				Logger.Error(e, "Bootstrapper.InitializeFs");
				CloseOnException(e.Message);
			}
		}

		void InitializeGk()
		{
			GKDriversCreator.Create();
			XManager.GetConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();
		}

		bool IsRestarting = false;
		void OnConfigurationChanged()
		{
			try
			{
				if (IsRestarting)
					return;
                FiresecManager.FiresecDriver.FiresecSerializedClient.NativeFiresecClient.StopPingThread();
                FiresecManager.FiresecService.SuspendPoll = true;
                LoadingErrorManager.Clear();
				IsRestarting = true;
				ProgressWatcher.Close();
				NativeFiresecClient.IsSuspended = true;
				ApplicationService.Restart();

				LoadingService.Show("Перезагрузка конфигурации", 10);
				LoadingService.AddCount(10);

				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();

				InitializeFs(true);
				InitializeGk();

				InitializeModules();
				ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);

				LoadingService.Close();
			}
			finally
			{
				NativeFiresecClient.IsSuspended = false;
				IsRestarting = false;
                FiresecManager.FiresecDriver.FiresecSerializedClient.NativeFiresecClient.StartPingThread();
                FiresecManager.FiresecService.SuspendPoll = false;
			}
		}

		void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			Application.Current.Shutdown();
		}

		void OnNotify(string message)
		{
			MessageBoxService.Show(message);
		}
	}
}