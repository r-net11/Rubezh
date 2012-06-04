using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using Infrastructure.Common.Windows.Views;

namespace Infrastructure.Common.Windows
{
	public static class ApplicationService
	{
		public static void Run(ShellViewModel model)
		{
			WindowBaseView win = new WindowBaseView(model);
			model.Surface.Owner = null;
			model.Surface.ShowInTaskbar = true;
			model.Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Application.Current.MainWindow = win;
			Application.Current.MainWindow.Show();
			Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
		}
		public static void ShutDown()
		{
			if (Application.Current.MainWindow != null)
				Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}
	}
}
