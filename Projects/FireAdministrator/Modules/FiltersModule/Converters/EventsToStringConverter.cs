using System;
using FiresecAPI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace FiltersModule.Converters
{
	public class EventsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var events = value as List<StateType>;
			if (events.IsNotNullOrEmpty())
			{
				var delimString = " или ";
				var result = new StringBuilder();

				foreach (var event_ in events)
				{
					result.Append(EnumsConverter.StateTypeToEventName(event_));
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