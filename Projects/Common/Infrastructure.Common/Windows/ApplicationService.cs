using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using System.Collections.ObjectModel;

namespace Infrastructure.Common.Windows
{
	public static class ApplicationService
	{
		public static event CancelEventHandler Closing;

		public static Window ApplicationWindow { get; private set; }
		public static User User { get; set; }
		public static ReadOnlyCollection<IModule> Modules { get; private set; }
		public static ILayoutService Layout { get; private set; }

		public static void Run(ApplicationViewModel model)
		{
			WindowBaseView win = new WindowBaseView(model);
			win.Closing += new CancelEventHandler(win_Closing);
			model.Surface.Owner = null;
			model.Surface.ShowInTaskbar = true;
			model.Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			if (Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.MainWindow = win;
				Application.Current.MainWindow.Show();
				Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
			}
			else
				win.Show();
			ApplicationWindow = win;
		}
		public static void Run(ShellViewModel model)
		{
			Layout = new LayoutService(model);
			Run((ApplicationViewModel)model);
		}
		public static void ShutDown()
		{
			if (Application.Current.MainWindow != null)
				Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}
		public static void DoEvents()
		{
			if (Application.Current != null)
				Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
		}
		public static void Invoke(Action action)
		{
			if (Application.Current != null)
			{
				if (Application.Current.Dispatcher.CheckAccess())
					action();
				else
					Application.Current.Dispatcher.Invoke(action);
			}
		}
		public static void CloseAllWindows()
		{
			var windows = new List<Window>();
			foreach (Window win in Application.Current.Windows)
				if (win != ApplicationWindow)
					windows.Add(win);
			foreach (Window window in windows)
				Invoke(() => window.Close());
		}

		public static void RegisterModules(List<IModule> modules)
		{
			Modules = new ReadOnlyCollection<IModule>(modules);
		}

		private static void win_Closing(object sender, CancelEventArgs e)
		{
			if (Closing != null)
				Closing(sender, e);
		}
	}
}