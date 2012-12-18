using System.Windows;
using MultiClient.ViewModels;

namespace MultiClient
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Bootstrapper.Run();

			var shellView = new ShellView();
			var shellViewModel = new ShellViewModel();
			shellView.DataContext = shellViewModel;
			shellView.Show();
		}
	}
}