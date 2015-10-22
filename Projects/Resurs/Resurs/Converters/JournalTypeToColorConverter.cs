using ResursAPI;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Resurs.Converters
{
	class JournalTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((JournalType)value)
			{
				case JournalType.AddUser:
				case JournalType.AddDevice:
				case JournalType.AddApartment:
				case JournalType.AddTariff:
					return Brushes.Yellow;
				default:
					return Brushes.Transparent;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}