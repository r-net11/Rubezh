using System;
using System.Windows.Data;
using FiresecAPI;

namespace Controls.Converters
{
	public class StateTypeToJournalStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((StateType)value == StateType.Info)
				return "Информация";
			if ((StateType)value == StateType.No)
				return "Прочие";
			return ((StateType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}