using System;
using System.Windows;
using Common;
using Common.GK;
using FireAdministrator.ViewModels;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
            LoadingErrorManager.Clear();
			AppSettingsHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new ProgressService(), new ValidationService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				try
				{
					LoadingService.Show("Чтение конфигурации", 4);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					if (!InitializeFs())
						return;
					if (LoadingErrorManager.HasError)
					{
						MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке драйвера FireSec");
					}

					LoadingService.DoStep("Загрузка конфигурации ГК");
					InitializeGk();

					LoadingService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll(true);

					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Adm_ViewConfig) == false)
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					else
					{
						var shell = new AdministratorShellViewModel();
						ServiceFactory.MenuService = new MenuService((vm) => ((MenuViewModel)shell.Toolbar).ExtendedMenu = vm);
						RunShell(shell);
					}
					LoadingService.Close();

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);
					MutexHelper.KeepAlive();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.Initialize");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return;
			}
		}

		bool InitializeFs()
		{
			try
			{
				LoadingService.DoStep("Загрузка конфигурации с сервера");
				FiresecManager.GetConfiguration();
				LoadingService.DoStep("Загрузка драйвера устройств");
                var connectionResult = FiresecManager.InitializeFiresecDriver(false);
				if (connectionResult.HasError)
				{
					CloseOnException(connectionResult.Error);
					return false;
				}
				LoadingService.DoStep("Синхронизация конфигурации");
				FiresecManager.FiresecDriver.Synchronyze();
				LoadingService.DoStep("Старт мониторинга");
				FiresecManager.FiresecDriver.StartWatcher(false, false);
				FiresecManager.FSAgent.Start();
			}
			catch (FiresecException e)
			{
				Logger.Error(e, "Bootstrapper.InitializeFs");
				CloseOnException(e.Message);
				return false;
			}
			return true;
		}

		void InitializeGk()
		{
			GKDriversCreator.Create();
			XManager.GetConfiguration();
		}

		void OnConfigurationChanged(object obj)
		{
            LoadingErrorManager.Clear();
			InitializeModules();
		}

		void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			if (Application.Current != null)
				Application.Current.Shutdown();
		}
	}
}