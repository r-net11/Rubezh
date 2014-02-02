using System;
using System.Windows;

namespace AutoHosting
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			var view = new View();
			var viewModel = new ViewModel();
			view.DataContext = viewModel;
			view.Show();
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("An unhandled exception occured: " + e.ToString());
		}
	}
}