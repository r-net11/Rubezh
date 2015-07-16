using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using PowerCalculator.ViewModels;
using PowerCalculator.Views;
using Infrastructure.Common.Theme;

namespace PowerCalculator
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(App).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
			ThemeHelper.LoadThemeFromRegister();

			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();
		}
	}
}