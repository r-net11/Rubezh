using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows;
using FiresecService.Service;

namespace FiresecService.Views
{
	public partial class MainView : UserControl
	{
		private Window _window;


		public MainView()
		{
			InitializeComponent();
			//Loaded += new RoutedEventHandler(MainWindow_Loaded);
			CreateNotificationIcon();
		}

		private void CreateNotificationIcon()
		{
			NotifyIconService.Start(OnShow, OnClose);
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_window = Window.GetWindow(this);
			_window.ShowInTaskbar = false;
			_window.StateChanged += new EventHandler(Window_StateChanged);
			_window.WindowState = WindowState.Minimized;
		}

		private void OnShow(object sender, EventArgs e)
		{
			_window.WindowState = WindowState.Normal;
		}
		private void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить сервер?") == MessageBoxResult.Yes)
			{
				_window.Close();
				NotifyIconService.Stop();
				Bootstrapper.Close();
			}
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			Window window = (Window)sender;
			switch (window.WindowState)
			{
				case WindowState.Minimized:
					window.ShowInTaskbar = false;
					break;
				case WindowState.Normal:
					window.ShowInTaskbar = true;
					break;
			}
		}
	}
}