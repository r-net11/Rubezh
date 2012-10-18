using System;
using System.Linq;
using System.Windows;
using Common.GK;
using FireAdministrator.ViewModels;
using Firesec;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			AppSettingsHelper.InitializeAppSettings();
			LoadStyles();
			ServiceFactory.Initialize(new LayoutService(), new ProgressService(), new ValidationService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect())
				try
				{
					LoadingService.Show("Чтение конфигурации", 4);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();
					InitializeFs();
                    var loadingError = FiresecManager.GetLoadingError();
                    if (!String.IsNullOrEmpty(loadingError))
					{
                        MessageBoxService.ShowWarning(loadingError, "Ошибки при загрузке драйвера FireSec");
					}
					LoadingService.DoStep("Загрузка конфигурации ГК");
					InitializeGk();

                    LoadingService.DoStep("Старт полинга сервера");
                    FiresecManager.StartPoll(true);

					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
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
				}
				catch (Exception ex)
				{
					MessageBoxService.ShowException(ex);
					Application.Current.Shutdown();
				}
			else
				Application.Current.Shutdown();
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);

			MutexHelper.KeepAlive();
		}

		void InitializeFs()
		{
			LoadingService.DoStep("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration();
			LoadingService.DoStep("Загрузка драйвера устройств");
			FiresecManager.InitializeFiresecDriver(ServiceFactory.AppSettings.FS_Address, ServiceFactory.AppSettings.FS_Port, ServiceFactory.AppSettings.FS_Login, ServiceFactory.AppSettings.FS_Password, false);
			LoadingService.DoStep("Синхронизация конфигурации");
			FiresecManager.FiresecDriver.Synchronyze();
			LoadingService.DoStep("Старт мониторинга");
			FiresecManager.FiresecDriver.StartWatcher(false, false);
		}

		void InitializeGk()
		{
			GKDriversCreator.Create();
			XManager.GetConfiguration();
		}

		void OnConfigurationChanged(object obj)
		{
			InitializeModules();
		}

		void LoadStyles()
		{
			ThemeHelper.LoadThemeFromRegister();
		}
	}
}