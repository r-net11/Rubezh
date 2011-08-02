using System;
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
            var events = value as List<State>;
            StringBuilder result = new StringBuilder();

            if (events != null)
            {
                for (var i = 0; i < events.Count; ++i)
                {
                    if (i > 0)
                    {
                        result.Append(" или ");
                    }
                    result.Append(events[i].EventName);
                }
            }

            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
