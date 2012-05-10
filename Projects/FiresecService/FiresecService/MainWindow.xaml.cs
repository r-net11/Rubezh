using System;
using System.IO;
using System.Windows;
using Infrastructure.Common.MessageBox;
using FiresecService;

namespace FiresecServiceRunner
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(MainWindow_Loaded);
			CreateNotificationIcon();
		}

		void CreateNotificationIcon()
		{
			var notifyIcon = new System.Windows.Forms.NotifyIcon();
			Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/FiresecService;component/Firesec.ico")).Stream;
			notifyIcon.Icon = new System.Drawing.Icon(iconStream);
			notifyIcon.Visible = true;

			notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
			var menuItem1 = new System.Windows.Forms.MenuItem();
			menuItem1.Text = "Показать консоль";
			menuItem1.Click += new EventHandler(OnShow);
			notifyIcon.ContextMenu.MenuItems.Add(menuItem1);

			var menuItem2 = new System.Windows.Forms.MenuItem();
			menuItem2.Text = "Выход";
			menuItem2.Click += new EventHandler(OnClose);
			notifyIcon.ContextMenu.MenuItems.Add(menuItem2);
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.ShowInTaskbar = true;
			this.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void OnShow(object sender, EventArgs e)
		{
			this.WindowState = WindowState.Normal;
			this.Visibility = System.Windows.Visibility.Visible;
		}

		private void OnHide(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить сервер?") == MessageBoxResult.Yes)
			{
				Close();
				Bootstrapper.Close();
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
			//_notificationIcon.Dispose();
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