using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class ZonesToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var zoneUIDs = value as ICollection<Guid>;
			if (zoneUIDs == null)
				return "";
			var zones = new List<XZone>();
			foreach (var uid in zoneUIDs)
			{
				var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == uid);
				if (zone != null)
					zones.Add(zone);
			}
			return XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}