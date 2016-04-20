using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using ServerFS2;

namespace MonitorClientFS2
{
	public partial class App : Application
	{
		const string SignalId = "39967D22-39F1-4472-A254-5F575CB8D18B";
		const string WaitId = "5BDAB105-A425-4A8B-8D23-B5A083A4C904";

		protected override void OnStartup(StartupEventArgs e)
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(App).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			ConfigurationManager.Load();

			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				ServerFS2.Bootstrapper.Run(false);
			}
		}
	}
}