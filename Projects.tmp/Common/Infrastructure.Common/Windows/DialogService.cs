using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Common;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
{
	public static class DialogService
	{
		public static Window GetActiveWindow()
		{
			return null;
		}

		public static bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel)
		{
			return false;
		}

		public static void ShowWindow(WindowBaseViewModel windowBaseViewModel)
		{
			
		}

		static List<IWindowIdentity> _openedWindows = new List<IWindowIdentity>();
		static bool FindWindowIdentity(WindowBaseViewModel windowBaseViewModel)
		{
			return false;
		}

		public static bool IsModal(this Window window)
		{
			return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
		}
	}
}