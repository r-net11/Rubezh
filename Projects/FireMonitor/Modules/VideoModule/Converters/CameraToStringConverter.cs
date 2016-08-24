using System;
using System.Linq;
using System.Windows.Data;
using Localization.Video.Common;
using StrazhAPI.Models;
using FiresecClient;

namespace VideoModule.Converters
{
	public class CameraToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var camera = value as Camera;
			if (camera != null && FiresecManager.SystemConfiguration.Cameras.Any(x => x.Ip == camera.Ip))
				return string.Format(CommonResources.Channel,camera.Ip , camera.ChannelNumber + 1);
			return CommonResources.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

	}
}
