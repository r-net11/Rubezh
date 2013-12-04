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
	public class DirectionsToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var directionUIDs = value as ICollection<Guid>;
			if (directionUIDs == null)
				return "";
			var directions = new List<XDirection>();
			foreach (var uid in directionUIDs)
			{
				var direction = XManager.Directions.FirstOrDefault(x => x.UID == uid);
				if (direction != null)
					directions.Add(direction);
			}
			return XManager.GetCommaSeparatedDirections(directions); 
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}
