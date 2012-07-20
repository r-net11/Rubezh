using System;
using System.Windows.Data;
using FiresecClient.Validation;

namespace FireAdministrator.Converters
{
	public class ErrorLevelToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ErrorLevel)value)
			{
				case ErrorLevel.CannotSave:
					return "Невозможно сохранить";

				case ErrorLevel.CannotWrite:
					return "Невозможно записать в прибор";

				case ErrorLevel.Warning:
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