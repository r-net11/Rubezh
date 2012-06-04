using System.Linq;
using System.Windows;
using FireAdministrator.ViewModels;
using FireAdministrator.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure.Client;
using Infrastructure.Common.Windows.Views;
using Infrastructure.Common.Windows.ViewModels;
using System;
using Infrastructure.Common.Windows;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			//if (!MutexHelper.IsNew("FireAdministrator"))
			//{
			//    MessageBoxService.ShowWarning("Другой экзэмпляр приложения уже запущен. Приложение будет закрыто");
			//    Application.Current.Shutdown();
			//    System.Environment.Exit(1);
			//    return;
			//}

			AppSettingsHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new ProgressService(), new ValidationService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				var preLoadWindow = new Infrastructure.Common.Windows.ViewModels.ProgressViewModel() { Title = "Инициализация компонент..." };
				DialogService.ShowWindow(preLoadWindow);

				FiresecManager.GetConfiguration();
				if (FiresecManager.Drivers.Count == 0)
				{
					MessageBoxService.Show("Ошибка при получении списка драйверов с сервера");
				}

				if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
				{
					MessageBoxService.Show("Нет прав на работу с программой");
					FiresecManager.Disconnect();
				}
				else
				{
					var shell = new AdministratorShellViewModel();
					shell.NavigationItems = GetNavigationItems();
					//ServiceFactory.ShellView = shell.Surface;
					InitializeModules();
					FireAdministrator.Views.ShellView view = new Views.ShellView();
					view.Show();
					ApplicationService.Run(shell);
				}
				preLoadWindow.ForceClose();
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
	}
}