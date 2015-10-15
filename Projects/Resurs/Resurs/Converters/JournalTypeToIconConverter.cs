using ResursAPI;
using System;
using System.Windows.Data;

namespace Resurs.Converters
{
	public class JournalTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((JournalType)value)
			{
				case JournalType.System:
					return "/Controls;component/StateClassIcons/info.png";
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}