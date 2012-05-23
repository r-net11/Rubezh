using System.Linq;
using System.Windows;
using FireAdministrator.ViewModels;
using FireAdministrator.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.MessageBox;
using Infrastructure.Events;
using Infrastructure.Client;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			AppSettingsHelper.InitializeAppSettings();

			if (SingleLaunchHelper.KillRunningProcess("FireAdministrator") == false)
			{
				MessageBox.Show("App is closing");
				Application.Current.Shutdown();
				System.Environment.Exit(1);
			}

			ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new ProgressService(), new ValidationService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			var preLoadWindow = new PreLoadWindow();
			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				preLoadWindow.PreLoadText = "Инициализация компонент...";
				preLoadWindow.Show();

				FiresecManager.GetConfiguration();

				if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
				{
					MessageBoxService.Show("Нет прав на работу с программой");
					FiresecManager.Disconnect();
				}
				else
				{
					var ShellView = new ShellView();
					ServiceFactory.ShellView = ShellView;
					InitializeModules();
					ShellView.Navigation = GetNavigationItems();
					Application.Current.MainWindow = ShellView;
					Application.Current.MainWindow.Show();
				}
				preLoadWindow.Close();
			}
			else
			{
				Application.Current.Shutdown();
				System.Environment.Exit(1);
			}
			SingleLaunchHelper.KeepAlive();

			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);
		}

		void OnConfigurationChanged(object obj)
		{
			InitializeModules();
		}
	}
}