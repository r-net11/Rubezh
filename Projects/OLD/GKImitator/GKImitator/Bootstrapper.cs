using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using GKImitator.ViewModels;
using System.Windows;

namespace GKImitator
{
	public static class Bootstrapper
	{
		public static void Run()
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			var mainViewModel = new MainViewModel();
			ApplicationService.Run(mainViewModel, false, false);
			Application.Current.MainWindow.BringIntoView();
			Application.Current.MainWindow.Show();
		}
	}
}