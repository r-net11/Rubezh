using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace JournalModule.Converters
{
    public class EventsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var events = value as List<StateType>;
            var result = new StringBuilder();

            if (events != null)
            {
                for (var i = 0; i < events.Count; ++i)
                {
                    if (i > 0)
                    {
                        result.Append(" или ");
                    }
                    result.Append(EnumsConverter.StateTypeToEventName(events[i]));
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