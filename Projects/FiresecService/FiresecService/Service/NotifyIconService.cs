using System;
using System.IO;
using System.Windows;
using Infrastructure.Common.Windows;

namespace FiresecService.Service
{
	public static class NotifyIconService
	{
		private static System.Windows.Forms.NotifyIcon _notifyIcon = null;
		public static void Start(EventHandler onShow, EventHandler onClose)
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
			_notifyIcon = new System.Windows.Forms.NotifyIcon();
			Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/FiresecService;component/Firesec.ico")).Stream;
			_notifyIcon.Icon = new System.Drawing.Icon(iconStream);
			_notifyIcon.Visible = true;

			_notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
			var menuItem1 = new System.Windows.Forms.MenuItem();
			menuItem1.Text = "Показать консоль";
			menuItem1.Click += new EventHandler(onShow);
			_notifyIcon.ContextMenu.MenuItems.Add(menuItem1);

			var menuItem2 = new System.Windows.Forms.MenuItem();
			menuItem2.Text = "Выход";
			menuItem2.Click += new EventHandler(onClose);
			_notifyIcon.ContextMenu.MenuItems.Add(menuItem2);
		}
		public static void Stop()
		{
			if (_notifyIcon != null)
			{
				_notifyIcon.Visible = false;
				_notifyIcon.Dispose();
				_notifyIcon = null;
			}
		}
		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			Stop();
		}
	}
}