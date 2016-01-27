using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using RubezhAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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
			
		}

		public static FrameworkElement BuildControl(ShellViewModel model)
		{
			return new FrameworkElement();
		}
		public static void Run(ShellViewModel model)
		{
			
		}
		public static void ShutDown()
		{
			
		}
		public static bool IsApplicationThread()
		{
			return false;
		}
		public static void DoEvents()
		{
			
		}
		public static void DoEvents(Dispatcher dispatcher)
		{
			dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
		}
		public static void Invoke(Action action)
		{
			
		}
		public static void Invoke(Action<string> action, string parameter)
		{
			
		}
		public static DispatcherOperation BeginInvoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			return null;
		}
		public static void CloseAllWindows()
		{
			
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