using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI;

namespace GKModule.Converters
{
	public class ZonesToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var zoneUIDs = value as ICollection<Guid>;
			if (zoneUIDs == null)
				return "";
			var zones = new List<GKZone>();
			foreach (var uid in zoneUIDs)
			{
				var zone = GKManager.Zones.FirstOrDefault(x => x.UID == uid);
				if (zone != null)
					zones.Add(zone);
			}
			return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(zones));
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}