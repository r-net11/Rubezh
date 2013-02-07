using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ClientFS2;
using Infrastructure.Common;
using ClientMonitorFS2.Views;
using ClientMonitorFS2.ViewModels;

namespace ClientMonitorFS2
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			ConfigurationManager.Load();
			//var resourceService = new ResourceService();
			//resourceService.AddResource(new ResourceDescription(typeof(App).Assembly, "DataTemplates/Dictionary.xaml"));
			var mainView = new MainView();
			var mainViewModel = new MainViewModel();
			mainView.DataContext = mainViewModel;
			mainView.Show();
		}
	}
}
