using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

namespace InstructionsModule.Converters
{
	public class DevicesToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var deviceUIDs = value as ICollection<Guid>;
			if (deviceUIDs.IsNotNullOrEmpty())
			{
				var delimiter = ", ";
				var stringBuilder = new StringBuilder();

				foreach (var deviceUID in deviceUIDs)
				{
					var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (device != null)
					{
						stringBuilder.Append(device.PresentationAddressAndName);
						stringBuilder.Append(delimiter);
					}
				}

				var result = stringBuilder.ToString();
				if (result.EndsWith(delimiter))
					result = result.Remove(result.Length - delimiter.Length);
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