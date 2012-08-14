using System;
using System.Windows.Data;
using Infrastructure.Common.Validation;

namespace FireAdministrator.Converters
{
	public class ErrorLevelToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ValidationErrorLevel)value)
			{
				case ValidationErrorLevel.CannotSave:
					return "Невозможно сохранить";

				case ValidationErrorLevel.CannotWrite:
					return "Невозможно записать в прибор";

				case ValidationErrorLevel.Warning:
					return "Предупреждение";

				default:
					return "Неизвестно";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}