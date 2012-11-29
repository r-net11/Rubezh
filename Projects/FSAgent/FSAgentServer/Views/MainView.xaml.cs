using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infrastructure.Common.Windows;
using FSAgentServer.ViewModels;

namespace FSAgentServer.Views
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
		}

		private void OnShow(object sender, EventArgs e)
		{
			_window.WindowState = WindowState.Normal;
			_window.Activate();
		}
		private void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить агент?") == MessageBoxResult.Yes)
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