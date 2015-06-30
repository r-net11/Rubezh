using System;
using System.Windows.Data;
using System.Windows.Media;
using Infrastructure.Common.Validation;

namespace FireAdministrator.Converters
{
	public class ErrorLevelToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ValidationErrorLevel) value)
			{
				case ValidationErrorLevel.CannotSave:
					return Brushes.DarkRed;
				case ValidationErrorLevel.CannotWrite:
					return Brushes.LightPink;
				case ValidationErrorLevel.Warning:
					return Brushes.Wheat;
				default:
					return Brushes.Blue;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}