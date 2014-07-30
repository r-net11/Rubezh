using System.Windows;
using ChinaSKDDriver;
using ControllerSDK.ViewModels;
using ChinaSKDDriverNativeApi;
using System;
using ChinaSKDDriverAPI;
using System.Diagnostics;

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
			string error;
			var loginID = MainViewModel.Wrapper.Connect("172.16.6.54", 37777, "system", "123456", out error);
			_textBox.Text += "LoginID = " + loginID + " " + error + "\n";
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