using System;
using System.IO;
using System.Windows;
using FiresecService;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Controls.MessageBox;
using System.Configuration;
using System.Diagnostics;
using Common;

namespace FiresecServiceRunner
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(MainWindow_Loaded);
			_mainView.DataContext = new MainViewModel();
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.ShowInTaskbar = true;
			this.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void OnShow(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Normal;
			this.Visibility = System.Windows.Visibility.Visible;
		}

		private void OnHide(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		void OnClose(object sender, RoutedEventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить сервер?") == MessageBoxResult.Yes)
			{
				Close();
				Application.Current.Shutdown();
				System.Environment.Exit(1);
			}
		}

		void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
				this.DragMove();
		}

		void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			if (this.Width + e.HorizontalChange > 10)
				this.Width += e.HorizontalChange;
			if (this.Height + e.VerticalChange > 10)
				this.Height += e.VerticalChange;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			_notificationIcon.Dispose();
			base.OnClosing(e);
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (this.WindowState == WindowState.Minimized)
			{
				this.ShowInTaskbar = false;
			}
			if (this.WindowState == WindowState.Normal)
			{
				this.ShowInTaskbar = true;
			}
		}
	}
}