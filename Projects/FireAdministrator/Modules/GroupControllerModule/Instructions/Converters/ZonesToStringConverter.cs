using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI;
using FiresecClient;

namespace GKModule.Converters
{
	public class ZonesToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var zoneUIDs = value as ICollection<Guid>;
			if (zoneUIDs.IsNotNullOrEmpty())
			{
				var delimString = ", ";
				var result = new StringBuilder();

                foreach (var zoneUID in zoneUIDs)
				{
                    var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
                    if (zone != null)
                    {
                        result.Append(zone.No);
                        result.Append(delimString);
                    }
				}

				return result.ToString().Remove(result.Length - delimString.Length);
			}

			return null;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}