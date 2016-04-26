using System.Reflection;
using System.Windows;

namespace Common
{
	public static class EnvironmentParameters
	{
		static EnvironmentParameters()
		{
			var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
			var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

			DpiX = (int)dpiXProperty.GetValue(null, null);
			DpiY = (int)dpiYProperty.GetValue(null, null);
		}

		public static int DpiX { get; private set; }

		public static int DpiY { get; private set; }
	}
}