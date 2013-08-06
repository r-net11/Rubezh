using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace MonitorClientFS2
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(App).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			ServerFS2.Bootstrapper.Run();
			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();
		}
	}
}