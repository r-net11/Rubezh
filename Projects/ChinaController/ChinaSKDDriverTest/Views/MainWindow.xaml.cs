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
			MainViewModel.Wrapper = new Wrapper();
			MainViewModel = new MainViewModel();
			DataContext = MainViewModel;
			OnConnect(this, null);
		}

		public MainViewModel MainViewModel { get; private set; }

		void OnConnect(object sender, RoutedEventArgs e)
		{
			var loginID = MainViewModel.Wrapper.Connect("172.16.2.53", 37777, "system", "123456");
			_textBox.Text += "LoginID = " + loginID + "\n";
			Wrapper.Start();
		}

		void OnDisconnect(object sender, RoutedEventArgs e)
		{
			Wrapper.Stop();
			var result = MainViewModel.Wrapper.Disconnect();
			_textBox.Text += "result = " + result + "\n";
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Wrapper.Stop();
			MainViewModel.Wrapper.Disconnect();
		}
	}
}