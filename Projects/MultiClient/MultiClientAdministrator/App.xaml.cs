using System.Windows;
using MultiClientAdministrator.ViewModels;
using Infrastructure.Common.Windows;
using System.IO;
using MuliclientAPI;

namespace MultiClientAdministrator
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Bootstrapper.Run();

			var shellView = new ShellView();
			Application.Current.MainWindow = shellView;
			var shellViewModel = new ShellViewModel();
			shellView.DataContext = shellViewModel;

			if (File.Exists("MulticlientConfiguration.xml"))
			{
				var passwordViewModel = new PasswordViewModel();
				DialogService.ShowModalWindow(passwordViewModel);
				shellViewModel.Initialize(passwordViewModel.MulticlientConfiguration);
			}
			else
			{
				shellViewModel.Initialize(new MulticlientConfiguration());
			}
			shellView.Show();
		}
	}
}