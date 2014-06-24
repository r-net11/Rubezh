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
			File.Copy(@"D:\Projects\Projects\ControllerSDK\SDK_DLL\EntranceGuardDemo\Bin\EntranceGuardDemo.dll", @"D:\Projects\Projects\ControllerSDK\ControllerSDK\bin\Debug\EntranceGuardDemo.dll", true);
			ChinaSKDDriverNativeApi.NativeWrapper.WRAP_Initialize();
			MainViewModel = new MainViewModel();
			DataContext = MainViewModel;
			OnConnect(this, null);
		}

		public MainViewModel MainViewModel { get; private set; }

		void OnConnect(object sender, RoutedEventArgs e)
		{
			MainViewModel.Wrapper = new Wrapper();
			var loginID = MainViewModel.Wrapper.Connect("172.16.6.58", 37777, "admin", "123456");
			_textBox.Text += "LoginID = " + loginID + "\n";
			MainViewModel.Wrapper.StartWatcher();
		}

		void OnDisconnect(object sender, RoutedEventArgs e)
		{
			MainViewModel.Wrapper.StopWatcher();
			var result = MainViewModel.Wrapper.Disconnect();
			_textBox.Text += "result = " + result + "\n";
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			MainViewModel.Wrapper.StopWatcher();
			MainViewModel.Wrapper.Disconnect();
		}
	}
}