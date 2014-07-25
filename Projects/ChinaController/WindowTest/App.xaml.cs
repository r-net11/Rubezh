using System.Windows;

namespace WindowTest
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

			ChinaSKDDriverNativeApi.NativeWrapper.WRAP_Initialize();
			var mainWindow = new MainWindow();
			mainWindow.Show();
		}
	}
}