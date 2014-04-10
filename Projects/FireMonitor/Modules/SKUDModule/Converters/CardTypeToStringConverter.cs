using System.Windows.Data;
using FiresecAPI;
using Infrastructure.Models;

namespace SKDModule.Converters
{
	public class CardTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((CardType)value).ToDescription();
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}