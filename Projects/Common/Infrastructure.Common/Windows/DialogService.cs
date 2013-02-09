using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;

namespace Infrastructure.Common.Windows
{
	public static class DialogService
	{
		public static Window GetActiveWindow()
		{
			if (!Application.Current.Dispatcher.CheckAccess())
				return (Window)Application.Current.Dispatcher.Invoke((Func<Window>)GetActiveWindow);
			var window = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
			return window ?? ApplicationService.ApplicationWindow;
		}

		public static bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel, bool allowsTransparency = true)
		{
			try
			{
				WindowBaseView win = new WindowBaseView(windowBaseViewModel);
				win.AllowsTransparency = allowsTransparency;
				PrepareWindow(windowBaseViewModel);
				bool? result = win.ShowDialog();
				return result.HasValue ? result.Value : false;
			}
			catch (Exception e)
			{
				Logger.Error(e, "DialogService.ShowModalWindow");
			}
			return false;
		}
		public static void ShowWindow(WindowBaseViewModel windowBaseViewModel)
		{
			if (!FindWindowIdentity(windowBaseViewModel))
			{
				var windowBaseView = new WindowBaseView(windowBaseViewModel);
				PrepareWindow(windowBaseViewModel);
				windowBaseView.Show();
			}
		}

		static List<IWindowIdentity> _openedWindows = new List<IWindowIdentity>();
		static bool FindWindowIdentity(WindowBaseViewModel windowBaseViewModel)
		{
			var identityModel = windowBaseViewModel as IWindowIdentity;
			if (identityModel != null)
			{
				WindowBaseViewModel openedWindow = (WindowBaseViewModel)_openedWindows.FirstOrDefault(x => x.Guid == identityModel.Guid);
				if (openedWindow != null && openedWindow.Surface != null)
				{
					openedWindow.Surface.Activate();
					return true;
				}
				windowBaseViewModel.Closed += (s, e) => _openedWindows.Remove((IWindowIdentity)s);
				_openedWindows.Add(identityModel);
			}
			return false;
		}
		static void PrepareWindow(WindowBaseViewModel model)
		{
			SetWindowProperty(model);
			UpdateWindowSize(model);
		}
		static void SetWindowProperty(WindowBaseViewModel windowBaseViewModel)
		{
			windowBaseViewModel.Surface.Owner = GetActiveWindow();
			windowBaseViewModel.Surface.ShowInTaskbar = windowBaseViewModel.Surface.Owner == null;
			windowBaseViewModel.Surface.WindowStartupLocation = windowBaseViewModel.Surface.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
		}
		static void UpdateWindowSize(WindowBaseViewModel windowBaseViewModel)
		{
			try
			{
				var isSaveSize = windowBaseViewModel.GetType().GetCustomAttributes(typeof(SaveSizeAttribute), true).Length > 0;
				if (isSaveSize)
				{
					string key = "WindowRect." + windowBaseViewModel.GetType().AssemblyQualifiedName;
					var windowRect = RegistrySettingsHelper.GetWindowRect(key);
					if (windowRect != null)
					{
						windowBaseViewModel.Surface.Top = windowRect.Top;
						windowBaseViewModel.Surface.Left = windowRect.Left;
						windowBaseViewModel.Surface.Width = windowRect.Width;
						windowBaseViewModel.Surface.Height = windowRect.Height;
						windowBaseViewModel.Surface.WindowStartupLocation = WindowStartupLocation.Manual;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DialogService.UpdateWindowSize");
			}
			windowBaseViewModel.Closed -= SaveWindowSize;
			windowBaseViewModel.Closed += SaveWindowSize;
		}
		static void SaveWindowSize(object sender, EventArgs e)
		{
			WindowBaseViewModel windowBaseViewModel = (WindowBaseViewModel)sender;
			var isSaveSize = windowBaseViewModel.GetType().GetCustomAttributes(typeof(SaveSizeAttribute), true).Length > 0;
			if (isSaveSize)
			{
				string key = "WindowRect." + windowBaseViewModel.GetType().AssemblyQualifiedName;
				var windowRect = new WindowRect()
				{
					Left = windowBaseViewModel.Surface.Left,
					Top = windowBaseViewModel.Surface.Top,
					Width = windowBaseViewModel.Surface.Width,
					Height = windowBaseViewModel.Surface.Height
				};
				RegistrySettingsHelper.SetWindowRect(key, windowRect);
			}
		}

		public static bool IsModal(this Window window)
		{
			return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
		}
	}
}