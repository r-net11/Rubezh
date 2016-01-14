using GKImitator.ViewModels;
using GKImitator.Views;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;

namespace GKImitator
{
	public static class Bootstrapper
	{
		public static void Run()
		{
			ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
			ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");

			var mainViewModel = new MainViewModel();
			var mainView = new MainView();
			mainView.DataContext = mainViewModel;
			mainView.Show();

			//ApplicationService.Run(mainViewModel, false, false);
			//Application.Current.MainWindow.BringIntoView();
			//Application.Current.MainWindow.Show();
		}
	}
}