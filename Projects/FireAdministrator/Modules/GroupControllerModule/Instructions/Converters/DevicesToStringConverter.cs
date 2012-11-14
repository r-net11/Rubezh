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
				var result = new StringBuilder();

				XDevice device = null;
				foreach (var deviceGuid in devices)
				{
					device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceGuid);

					result.Append(device.PresentationAddressAndDriver);
					result.Append(delimString);
				}

				return result.ToString().Remove(result.Length - delimString.Length);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}