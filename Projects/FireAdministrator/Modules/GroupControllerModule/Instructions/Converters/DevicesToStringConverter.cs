using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class DevicesToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var devices = value as ICollection<Guid>;
			if (devices.IsNotNullOrEmpty())
			{
				var delimString = ", ";
				var stringBuilder = new StringBuilder();

				foreach (var deviceGuid in devices)
				{
					var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceGuid);
					if (device != null)
					{
						stringBuilder.Append(device.ShortNameAndDottedAddress);
						stringBuilder.Append(delimString);
					}
				}
				var result = stringBuilder.ToString();
				if (result.EndsWith(delimString))
				{
					result = result.Remove(result.Length - delimString.Length);
				}
				return result;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}