using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using System.Windows.Controls;
using Infrastructure.Common.Windows.DataTemplates;

namespace Infrastructure.Common.Windows
{
	public static class ApplicationService
	{
		public static event CancelEventHandler Closing;
		public static Window ApplicationWindow { get; private set; }
		public static User User { get; set; }
		public static Action<FrameworkElement> ApplicationController { get; set; }
		public static ReadOnlyCollection<IModule> Modules { get; private set; }
		public static ILayoutService Layout { get; private set; }

		public static void Run(ApplicationViewModel model)
		{
			var windowBaseView = new WindowBaseView(model);
			windowBaseView.Closing += new CancelEventHandler(win_Closing);
			model.Surface.Owner = null;
			model.Surface.ShowInTaskbar = true;
			model.Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			if (Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.MainWindow = windowBaseView;
				Application.Current.MainWindow.Show();
				Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
			}
			else
				windowBaseView.Show();
			ApplicationWindow = windowBaseView;
		}
		public static FrameworkElement BuildControl(ApplicationViewModel model)
		{
			return new ContentControl()
			{
				Content = model,
				//ContentTemplateSelector = new HierarhicalDataTemplateSelector(),
				MinHeight = 100,
				MinWidth = 100,
			};
		}
		public static void Run(ShellViewModel model)
		{
			Layout = new LayoutService(model);
			if (ApplicationController == null)
				Run((ApplicationViewModel)model);
			else
			{
				var frameworkElement = BuildControl(model);
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				ApplicationController(frameworkElement);
			}
		}
		public static void ShutDown()
		{
			Invoke(() =>
				{
					if (Application.Current.MainWindow != null)
						Application.Current.MainWindow.Close();
					Application.Current.Shutdown();
				});
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
		public static void BeginInvoke(Action action)
		{
			Application.Current.Dispatcher.BeginInvoke(action);
		}
		public static void ExecuteThread(Action action)
		{
			var thread = new Thread(() => action());
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		public static void CloseAllWindows()
		{
			var windows = new List<Window>();
			foreach (Window window in Application.Current.Windows)
				if (window != ApplicationWindow)
					windows.Add(window);
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

		public static void Restart()
		{
			if (Restarting != null)
				Restarting();
		}
		public static event Action Restarting;
	}
}