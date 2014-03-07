using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class DevicesToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var deviceUIDs = value as ICollection<Guid>;
			if (deviceUIDs == null)
				return "";
			var devices = new List<XDevice>();
			foreach (var uid in deviceUIDs)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == uid);
				if (device != null)
					devices.Add(device);
			}
			return XManager.GetCommaSeparatedDevices(devices);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}