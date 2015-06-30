﻿using System;
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

		public static bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel)
		{
			try
			{
				WindowBaseView win = new WindowBaseView(windowBaseViewModel);
				windowBaseViewModel.OnLoad();
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
				windowBaseViewModel.OnLoad();
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

		public static bool IsModal(this Window window)
		{
			return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
		}
	}
}