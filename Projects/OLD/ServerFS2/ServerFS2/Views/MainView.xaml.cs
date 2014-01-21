using System;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows;
using ServerFS2.ViewModels;

namespace ServerFS2.Views
{
	public partial class MainView : UserControl
	{
		private Window _window;

		public MainView()
		{
			InitializeComponent();
			CreateNotificationIcon();
		}

		void CreateNotificationIcon()
		{
			NotifyIconService.Start(OnShow, OnClose, OnShowLogs);
		}

		void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_window = Window.GetWindow(this);
			_window.StateChanged += new EventHandler(Window_StateChanged);
			_window.ShowInTaskbar = false;
			_window.Left = SystemParameters.WorkArea.Right;
			_window.Top = SystemParameters.WorkArea.Bottom;
		}

		void OnShow(object sender, EventArgs e)
		{
			_window.Left = SystemParameters.WorkArea.Right - _window.ActualWidth;
			_window.Top = SystemParameters.WorkArea.Bottom - _window.ActualHeight;
			_window.ShowInTaskbar = true;
			_window.WindowState = WindowState.Normal;
			_window.Activate();
		}

		void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить драйвер ОПС Firesec-2?") == MessageBoxResult.Yes)
			{
				_window.Close();
				NotifyIconService.Stop();
				Bootstrapper.Close();
			}
		}

		void OnShowLogs(object sender, EventArgs e)
		{
			var logViewModel = new LogsViewModel();
			DialogService.ShowWindow(logViewModel);
		}

		void Window_StateChanged(object sender, EventArgs e)
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