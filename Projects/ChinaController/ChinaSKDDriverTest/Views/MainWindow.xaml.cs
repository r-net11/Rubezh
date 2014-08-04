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
		}

		public MainViewModel MainViewModel { get; private set; }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			MainViewModel.Wrapper.Disconnect();
			Wrapper.Deinitialize();
			//Wrapper.WrapStop();
			//MainViewModel.Wrapper.WrapDisconnect();
		}
	}
}