using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infrastructure.Common.Properties;

namespace Infrastructure.Common
{
	public static class WindowsPositionHelper
	{
        private static int MainWindowScreen;
		public static void GetPlacement()
		{
			var leftValue = System.Windows.Application.Current.MainWindow.Left;
			var screens = new List<Screen>(Screen.AllScreens).OrderByDescending(x => x.WorkingArea.Left);
			var screen = screens.FirstOrDefault(x => x.WorkingArea.Left <= leftValue);
			if (screen == null)
				screen = screens.LastOrDefault();
			MainWindowScreen = new List<Screen>(Screen.AllScreens).IndexOf(screen);
			Settings.Default.Save();
		}

		public static void SetPlacement()
		{
			if (Screen.AllScreens.Length <= MainWindowScreen || MainWindowScreen < 0)
				return;
			var screen = Screen.AllScreens[MainWindowScreen];
			System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
			System.Windows.Application.Current.MainWindow.Left = screen.WorkingArea.Left;
			System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
		}
	}
}
