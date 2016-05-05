using System.Windows;
using ControllerSDK.Views;

namespace ControllerSDK
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var mainWindow = new MainWindow();
			mainWindow.Show();
		}
	}
}