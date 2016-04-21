using FiresecAPI.Models;
using Infrastructure.Common.Windows.DataTemplates;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infrastructure.Common.Windows
{
	public static class ApplicationService
	{
		public static event EventHandler Starting;

		public static event CancelEventHandler Closing;

		public static event EventHandler Closed;

		public static event Action ShuttingDown;

		public static Window ApplicationWindow { get; private set; }

		public static bool ApplicationActivated { get; private set; }

		public static User User { get; set; }

		public static Action<FrameworkElement> ApplicationController { get; set; }

		public static ReadOnlyCollection<IModule> Modules { get; private set; }

		public static ILayoutService Layout { get; private set; }

		public static ShellViewModel Shell { get; private set; }

		public static bool IsShuttingDown { get; private set; }

		public static bool IsReportEnabled { get; private set; }

		static ApplicationService()
		{
			ApplicationActivated = false;
			IsShuttingDown = false;
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				IsShuttingDown = true;
				if (ShuttingDown != null)
					ShuttingDown();
			};
		}

		public static void Run(ApplicationViewModel model, bool noBorder, bool? isMaximized)
		{
			var windowBaseView = new WindowBaseView(model);
			if (noBorder)
			{
				windowBaseView.ClearValue(Window.AllowsTransparencyProperty);
				windowBaseView.ClearValue(Window.WindowStyleProperty);
				windowBaseView.ClearValue(Window.BackgroundProperty);
				windowBaseView.SetValue(Window.WindowStyleProperty, WindowStyle.None);
				windowBaseView.SetValue(Window.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0x26, 0x61, 0x99)));
			}
			if (isMaximized.HasValue)
				windowBaseView.SetValue(Window.WindowStateProperty, isMaximized.Value ? WindowState.Maximized : WindowState.Minimized);
			windowBaseView.ContentRendered += (s, e) => ApplicationActivated = true;
			windowBaseView.Closing += win_Closing;
			windowBaseView.Closed += win_Closed;
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
			{
				windowBaseView.Show();
			}
			windowBaseView.Activate();
			model.Run();
			ApplicationWindow = windowBaseView;
		}

		public static FrameworkElement BuildControl(ShellViewModel model)
		{
			model.Header.ShowIconAndTitle = false;
			model.AllowClose = false;
			model.AllowHelp = false;
			model.AllowMaximize = false;
			model.AllowMinimize = false;
			model.Width = double.NaN;
			model.Height = double.NaN;
			var frameworkElement = new ScrollViewer()
			{
				Content = new ContentControl()
				{
					Content = model,
					ContentTemplateSelector = new MulticlientDataTemplateSelector(),
					MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 100,
					MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - 100,
				},
				HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			};
			frameworkElement.SetResourceReference(ScrollViewer.BackgroundProperty, "BaseWindowBackgroundBrush");
			return frameworkElement;
		}

		public static void Run(ShellViewModel model)
		{
			Shell = model;
			Layout = new LayoutService(model);
			if (Starting != null)
				Starting(Layout, EventArgs.Empty);
			if (ApplicationController == null)
				Run((ApplicationViewModel)model, false, true);
			else
			{
				var frameworkElement = BuildControl(model);
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				ApplicationController(frameworkElement);
			}
		}

		public static void ShutDown()
		{
			IsShuttingDown = true;
			Invoke(() =>
				{
					if (Application.Current == null)
						return;
					if (Application.Current.MainWindow != null)
						Application.Current.MainWindow.Close();
					else
					{
						if (Closing != null)
							Closing(null, new CancelEventArgs(false));
						if (Closed != null)
							Closed(null, EventArgs.Empty);
					}
					Application.Current.Shutdown();
				});
		}

		public static bool IsApplicationThread()
		{
			return Application.Current == null ? false : Application.Current.Dispatcher.Thread == Thread.CurrentThread;
		}

		public static void DoEvents()
		{
			if (Application.Current != null)
				DoEvents(Application.Current.Dispatcher);
		}

		public static void DoEvents(Dispatcher dispatcher)
		{
			dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
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

		public static DispatcherOperation BeginInvoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			return Application.Current.Dispatcher.BeginInvoke(priority, action);
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

		public static int GetActiveMonitor(bool shellWindow = false)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
				return (int)Application.Current.Dispatcher.Invoke((Func<bool, int>)GetActiveMonitor, shellWindow);
			var window = shellWindow ? ApplicationWindow : DialogService.GetActiveWindow();

			return window != null ? MonitorHelper.FindMonitor(window.RestoreBounds) : default(int);
		}

		public static void RegisterModules(List<IModule> modules)
		{
			Modules = new ReadOnlyCollection<IModule>(modules);
			IsReportEnabled = Modules.Any(item => item.Name == "Отчёты");
		}

		private static void win_Closing(object sender, CancelEventArgs e)
		{
			if (Closing != null)
				Closing(sender, e);
		}

		private static void win_Closed(object sender, EventArgs e)
		{
			if (Closed != null)
				Closed(sender, e);
		}
	}
}