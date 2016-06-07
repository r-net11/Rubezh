using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using StrazhAPI.Journal;

namespace Controls.Converters
{
	public class JournalEventNameTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var eventName = (EventNameAttribute)value.GetType().GetMember(value.ToString())
				.First()
				.GetCustomAttributes(typeof (EventNameAttribute), false)
				.FirstOrDefault();
			if (eventName == null)
				return null;

			return eventName.StateClass.ToIconSource();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}