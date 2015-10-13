using System.Windows.Data;
using RubezhAPI;
using Infrastructure.Models;

namespace JournalModule.Converters
{
	public class ArchiveDefaultStateTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((ArchiveDefaultStateType)value).ToDescription();
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}