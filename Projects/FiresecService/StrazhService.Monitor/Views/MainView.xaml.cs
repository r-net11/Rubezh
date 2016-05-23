using Infrastructure.Common.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using StrazhService.Monitor.Service;

namespace StrazhService.Monitor.Views
{
	public partial class MainView : UserControl
	{
		private Window _window;

		public MainView()
		{
			InitializeComponent();
			CreateNotificationIcon();
		}

		private void CreateNotificationIcon()
		{
			NotifyIconService.Start(OnShow, OnClose);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_window = Window.GetWindow(this);
			_window.StateChanged += new EventHandler(Window_StateChanged);
			_window.WindowState = WindowState.Minimized;
			_window.ShowInTaskbar = false;
			_window.WindowStyle = WindowStyle.ToolWindow;
			_window.Left = SystemParameters.WorkArea.Right;
			_window.Top = SystemParameters.WorkArea.Bottom;
		}

		private void OnShow(object sender, EventArgs e)
		{
			_window.WindowState = WindowState.Normal;
			_window.Left = SystemParameters.WorkArea.Right - _window.ActualWidth;
			_window.Top = SystemParameters.WorkArea.Bottom - _window.ActualHeight;
			_window.Activate();
		}

		private void OnClose(object sender, EventArgs e)
		{
			_window.Close();
			NotifyIconService.Stop();
			Bootstrapper.Instance.Close();
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			var window = (Window)sender;
			switch (window.WindowState)
			{
				case WindowState.Minimized:
					window.WindowStyle = WindowStyle.ToolWindow;
					window.ShowInTaskbar = false;
					break;

				case WindowState.Normal:
					window.WindowStyle = WindowStyle.None;
					window.ShowInTaskbar = true;
					break;
			}
		}
	}
}