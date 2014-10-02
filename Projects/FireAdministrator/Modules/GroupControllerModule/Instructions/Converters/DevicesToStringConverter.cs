using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using FiresecAPI.GK;
using FiresecClient;

namespace GKModule.Converters
{
	public class DevicesToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var deviceUIDs = value as ICollection<Guid>;
			if (deviceUIDs == null)
				return "";
			var devices = new List<GKDevice>();
			foreach (var uid in deviceUIDs)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == uid);
				if (device != null)
					devices.Add(device);
			}
			return GKManager.GetCommaSeparatedDevices(devices);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}