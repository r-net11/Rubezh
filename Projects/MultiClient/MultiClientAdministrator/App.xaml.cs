using System.Windows;
using MultiClient.ViewModels;
using Infrastructure.Common.Windows;

namespace MultiClient
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

			var passwordViewModel = new PasswordViewModel();
			DialogService.ShowModalWindow(passwordViewModel);
			var password = passwordViewModel.Password;

			if (!string.IsNullOrEmpty(password))
			{
				shellViewModel.Initialize(password);
			}
			shellView.Show();
		}
	}
}