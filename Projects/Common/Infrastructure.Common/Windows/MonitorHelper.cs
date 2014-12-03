using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monitor = System.Windows.Forms.Screen;
using System.Windows;

namespace Infrastructure.Common.Windows
{
    public static class MonitorHelper
    {
        public static int Count
        {
            get { return Monitor.AllScreens.Length; }
        }
        public static int PrimaryMonitor()
        {
            return Array.FindIndex(Monitor.AllScreens, m => m.Primary);
        }
        public static Rect WorkingArea(int index)
        {
            return ToRect(Monitor.AllScreens[index]);
        }
        public static int FindMonitor(Rect rect)
        {
            var index = Array.FindIndex(Monitor.AllScreens, m => ToRect(m).Contains(rect));
            return index < 0 ? PrimaryMonitor() : index;
        }

        private static Rect ToRect(Monitor monitor)
        {
            var area = monitor.WorkingArea;
            return new Rect(area.Left, area.Top, area.Width, area.Height);
        }
    }
}
