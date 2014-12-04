using System;
using System.Windows;
using System.Linq;
using Monitor = System.Windows.Forms.Screen;

namespace Infrastructure.Common.Windows
{
	public static class MonitorHelper
	{
		private static Size? _maxSize = null;
		public static Size MaxSize
		{
			get
			{
				if (!_maxSize.HasValue)
					_maxSize = new Size(Monitor.AllScreens.Max(item => ToRect(item).Width), Monitor.AllScreens.Max(item => ToRect(item).Height));
				return _maxSize.Value;
			}
		}
		public static int Count
		{
			get { return Monitor.AllScreens.Length; }
		}
		public static int PrimaryMonitor
		{
			get { return Array.FindIndex(Monitor.AllScreens, m => m.Primary); }
		}
		public static Rect WorkingArea(int monitorID)
		{
			return ToRect(Monitor.AllScreens[monitorID]);
		}
		public static int FindMonitor(Rect rect)
		{
			//var index = Array.FindIndex(Monitor.AllScreens, m => ToRect(m).Contains(rect));
			var index = Array.FindIndex(Monitor.AllScreens, m => ToRect(m).Contains(rect.TopLeft));
			return index < 0 ? PrimaryMonitor : index;
		}

		private static Rect ToRect(Monitor monitor)
		{
			var area = monitor.WorkingArea;
			return new Rect(area.Left, area.Top, area.Width, area.Height);
		}
	}
}
