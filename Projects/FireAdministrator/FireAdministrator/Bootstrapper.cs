using System;
using System.Linq;
using System.Windows;
using Common.GK;
using FireAdministrator.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;
using Firesec;

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
					LoadingService.DoStep("Загрузка конфигурации ГК");
					InitializeGk();

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
			LoadingService.DoStep("Остановка Socket Server");
			SocketServerHelper.Stop();
			LoadingService.DoStep("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration();
			LoadingService.DoStep("Загрузка драйвера устройств");
			FiresecManager.InitializeFiresecDriver(ServiceFactory.AppSettings.FS_Address, ServiceFactory.AppSettings.FS_Port, ServiceFactory.AppSettings.FS_Login, ServiceFactory.AppSettings.FS_Password);
			LoadingService.DoStep("Синхронизация конфигурации");
			FiresecManager.Synchronyze();
			LoadingService.DoStep("Старт мониторинга");
			FiresecManager.StatrtWatcher(false);
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
			if (!String.IsNullOrEmpty(ServiceFactory.AppSettings.Theme) && ServiceFactory.AppSettings.Theme != "1")
			{
				var themeName = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/Brushes" + ServiceFactory.AppSettings.Theme + ".xaml";
				Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(themeName) });
			}
		}
	}
}