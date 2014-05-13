using System;
using System.Linq;
using System.Windows.Data;
using FiresecAPI.Models;
using FiresecClient;

namespace VideoModule.Converters
{
	public class CameraToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var camera = value as Camera;
			if (camera != null && FiresecManager.SystemConfiguration.AllCameras.Any(x => x.Address == camera.Address))
				return camera.Address + " (" + (camera.ChannelNumber + 1) + " канал)";
			return "<нет>";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

	}
}
