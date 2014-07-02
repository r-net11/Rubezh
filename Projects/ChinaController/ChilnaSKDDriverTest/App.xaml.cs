using System.Windows;
using ControllerSDK.Views;

namespace ControllerSDK
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			System.IO.File.Copy(@"D:\Projects\Projects\ChinaController\CPPWrapper\Bin\CPPWrapper.dll", @"D:\Projects\Projects\ChinaController\ChilnaSKDDriverTest\bin\Debug\CPPWrapper.dll", true);
			ChinaSKDDriverNativeApi.NativeWrapper.WRAP_Initialize();
			var mainWindow = new MainWindow();
			mainWindow.Show();
		}
	}
}