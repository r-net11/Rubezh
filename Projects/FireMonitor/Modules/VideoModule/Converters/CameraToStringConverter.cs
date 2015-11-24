using System;
using System.Linq;
using System.Windows.Data;
using RubezhAPI.Models;
using RubezhClient;

namespace VideoModule.Converters
{
	public class CameraToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var camera = value as Camera;
			if (camera != null && ClientManager.SystemConfiguration.Cameras.Any(x => x.Ip == camera.Ip))
				return string.Format("{0} (канал {1}, поток {2})", camera.Ip, camera.ChannelNumber + 1, camera.StreamNo);
			return "<нет>";
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}