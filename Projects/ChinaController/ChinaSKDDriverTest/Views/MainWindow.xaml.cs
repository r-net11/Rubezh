using System.ComponentModel;
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

			MainViewModel.Wrapper = new Wrapper(Guid.Empty);
			MainViewModel = new MainViewModel();
			DataContext = MainViewModel;
		}

		public MainViewModel MainViewModel { get; private set; }

		protected override void OnClosing(CancelEventArgs e)
		{
			MainViewModel.Wrapper.Disconnect();
			Wrapper.Deinitialize();
			ConnectionSettingsHelper.Set(new ConnectionSettings()
			{
				Address = MainViewModel.ConnectionViewModel.Address,
				Port = MainViewModel.ConnectionViewModel.Port,
				Login = MainViewModel.ConnectionViewModel.Login,
				Password = MainViewModel.ConnectionViewModel.Password
			});

			base.OnClosing(e);
		}
	}
}