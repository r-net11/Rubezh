using System.Windows;
using ClientFS2;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace MonitorClientFS2
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			ConfigurationManager.Load();
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