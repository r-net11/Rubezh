using System;
using System.Windows.Data;
using Infrastructure.Common.Validation;
using Localization.FireAdministrator.Errors;

namespace FireAdministrator.Converters
{
	public class ErrorLevelToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ValidationErrorLevel)value)
			{
				case ValidationErrorLevel.CannotSave:
					return CommonErrors.UnableToSave_Error;

				case ValidationErrorLevel.CannotWrite:
					return CommonErrors.UnableWriteOnDevice_Error;

				case ValidationErrorLevel.Warning:
					return CommonErrors.Warning_Error;

				default:
					return CommonErrors.Unknown_Error;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}