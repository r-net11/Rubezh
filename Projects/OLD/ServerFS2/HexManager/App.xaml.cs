using System.Windows;
using HexManager.ViewModels;
using HexManager.Views;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;

namespace HexManager
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(App).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();
		}
	}
}