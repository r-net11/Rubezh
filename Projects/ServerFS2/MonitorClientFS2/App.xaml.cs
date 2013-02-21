using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using ServerFS2;

namespace MonitorClientFS2
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			//ConfigurationManager.Load();
			ServerHelper.Initialize();
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