using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI;
using FiresecClient;

namespace GKModule.Converters
{
	public class DirectionsToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var directionUIDs = value as ICollection<Guid>;
            if (directionUIDs.IsNotNullOrEmpty())
			{
				var delimString = ", ";
				var result = new StringBuilder();

                foreach (var directionUID in directionUIDs)
				{
                    var direction = XManager.Directions.FirstOrDefault(x => x.UID == directionUID);
                    if (direction != null)
					{
                        result.Append(direction.No);
						result.Append(delimString);
					}
				}
                string resultString = result.ToString();
                if (resultString.Length >= delimString.Length)
                    return resultString.Remove(result.Length - delimString.Length);
            }

			return null;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}
