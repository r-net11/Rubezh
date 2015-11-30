using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using PowerCalculator.ViewModels;
using PowerCalculator.Views;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Services;

namespace PowerCalculator
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			ServiceFactoryBase.ResourceService.AddResource(typeof(App).Assembly, "DataTemplates/Dictionary.xaml");
			ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");
			ThemeHelper.LoadThemeFromRegister();

			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();
		}
	}
}