using System;
using System.Windows.Data;
using Infrastructure.Common.Windows.Validation;

namespace FireAdministrator.Converters
{
	public class ErrorLevelToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ValidationErrorLevel)value)
			{
				case ValidationErrorLevel.CannotSave:
					return "/Controls;component/Validation/CannotSave.png";

				case ValidationErrorLevel.CannotWrite:
					return "/Controls;component/Validation/CannotWrite.png";

				case ValidationErrorLevel.Warning:
					return "/Controls;component/Validation/Warning.png";

				default:
					return "/Controls;component/Validation/Unknown.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
