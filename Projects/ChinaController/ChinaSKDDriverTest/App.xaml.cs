using System.Windows;
using ControllerSDK.Views;
using ChinaSKDDriver;

namespace ControllerSDK
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			try
			{
				System.IO.File.Copy(@"..\..\..\CPPWrapper\Bin\CPPWrapper.dll", @"CPPWrapper.dll", true);
			}
			catch { }

			//Wrapper.InitializeWatcher();
			var mainWindow = new MainWindow();
			mainWindow.Show();
		}
	}
}