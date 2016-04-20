using System.Windows;
using AdministratorTestClientFS2.ViewModels;
using AdministratorTestClientFS2.Views;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using ServerFS2;

namespace AdministratorTestClientFS2
{
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(App).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			ConfigurationManager.Load();
			USBManager.Initialize();
			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();
		}
	}
}