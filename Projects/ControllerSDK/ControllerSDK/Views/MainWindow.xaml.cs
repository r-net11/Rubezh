using System.IO;
using System.Windows;
using ChinaSKDDriver;
using ControllerSDK.ViewModels;

namespace ControllerSDK.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MainViewModel = new MainViewModel();
			DataContext = MainViewModel;

			//Watcher.Run();
			OnConnect(this, null);
		}

		public MainViewModel MainViewModel { get; private set; }

		void OnConnect(object sender, RoutedEventArgs e)
		{
			File.Copy(@"D:\Projects\Projects\ControllerSDK\SDK_DLL\EntranceGuardDemo\Bin\EntranceGuardDemo.dll", @"D:\Projects\Projects\ControllerSDK\ControllerSDK\bin\Debug\EntranceGuardDemo.dll", true);
			MainViewModel.Wrapper = new Wrapper();
			var loginID = MainViewModel.Wrapper.Connect("172.16.6.58", 37777, "admin", "123456");
			_textBox.Text += "LoginID = " + loginID + "\n";
		}

		void OnDisconnect(object sender, RoutedEventArgs e)
		{
			MainViewModel.Wrapper.Disconnect();
		}
	}
}