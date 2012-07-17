using System.Linq;
using System.Windows;
using FireAdministrator.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using System;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			AppSettingsHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new ProgressService(), new ValidationService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect())
				try
				{
					LoadingService.Show("Чтение конфигурации", 4);
					LoadingService.AddCount(GetModuleCount());
					LoadingService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration();
					if (FiresecManager.Drivers.Count == 0)
					{
						MessageBoxService.Show("Ошибка при загрузке конфигурации с сервера");
					}
					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					else
					{
						var shell = new AdministratorShellViewModel();
						((LayoutService)ServiceFactory.Layout).SetMenuViewModel((MenuViewModel)shell.Toolbar);
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

		private void OnConfigurationChanged(object obj)
		{
			InitializeModules();
		}
	}
}