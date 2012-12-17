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
using Infrastructure.Common.BalloonTrayTip;

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
            BalloonHelper.Initialize();

			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				try
				{
					CreateModules();

					LoadingService.Show("Чтение конфигурации", 4);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					BeforeInitialize(true);

					if (LoadingErrorManager.HasError)
					{
						MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке драйвера FireSec");
					}

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

					AterInitialize();

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