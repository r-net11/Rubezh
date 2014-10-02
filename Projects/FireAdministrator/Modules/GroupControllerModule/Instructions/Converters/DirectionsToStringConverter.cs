using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI;

namespace GKModule.Converters
{
	public class DirectionsToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var directionUIDs = value as ICollection<Guid>;
			if (directionUIDs == null)
				return "";
			var directions = new List<GKDirection>();
			foreach (var uid in directionUIDs)
			{
				var direction = GKManager.Directions.FirstOrDefault(x => x.UID == uid);
				if (direction != null)
					directions.Add(direction);
			}
			return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(directions));
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}