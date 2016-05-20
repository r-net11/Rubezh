using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Linq;
using System.Windows.Data;

namespace VideoModule.Converters
{
	public class CameraToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var camera = value as Camera;
			if (camera != null && ClientManager.SystemConfiguration.Cameras.Any(x => x.UID == camera.UID))
				return string.Format("{0}. {1}", camera.RviDeviceName, camera.Name);
			return "<нет>";
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}