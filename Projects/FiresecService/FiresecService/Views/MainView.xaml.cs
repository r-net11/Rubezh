using System.Windows.Controls;
using System.IO;
using System.Windows;
using System;
using Infrastructure.Common.Windows;

namespace FiresecService.Views
{
	public partial class MainView : UserControl
	{
		private System.Windows.Forms.NotifyIcon _notifyIcon;
		private Window _window;


		public MainView()
		{
			InitializeComponent();
			//Loaded += new RoutedEventHandler(MainWindow_Loaded);
			CreateNotificationIcon();
		}

		private void CreateNotificationIcon()
		{
			_notifyIcon = new System.Windows.Forms.NotifyIcon();
			Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/FiresecService;component/Firesec.ico")).Stream;
			_notifyIcon.Icon = new System.Drawing.Icon(iconStream);
			_notifyIcon.Visible = true;

			_notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
			var menuItem1 = new System.Windows.Forms.MenuItem();
			menuItem1.Text = "Показать консоль";
			menuItem1.Click += new EventHandler(OnShow);
			_notifyIcon.ContextMenu.MenuItems.Add(menuItem1);

			var menuItem2 = new System.Windows.Forms.MenuItem();
			menuItem2.Text = "Выход";
			menuItem2.Click += new EventHandler(OnClose);
			_notifyIcon.ContextMenu.MenuItems.Add(menuItem2);
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
		}
		private void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить сервер?") == MessageBoxResult.Yes)
			{
				_window.Close();
				_notifyIcon.Visible = false;
				_notifyIcon.Dispose();
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