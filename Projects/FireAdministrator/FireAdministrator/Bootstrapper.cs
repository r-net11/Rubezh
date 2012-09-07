using System;
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
using Common.GK;
using System.IO;
using System.Windows.Markup;
using Infrastructure.Services;
using Common;

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
					LoadingService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration();
					if (FiresecManager.Drivers.Count == 0)
					{
						MessageBoxService.Show("Ошибка при загрузке конфигурации с сервера");
					}

					LoadingService.DoStep("Загрузка конфигурации ГК с сервера");
					GKDriversCreator.Create();
					XManager.GetConfiguration();

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