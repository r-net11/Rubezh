using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class EnumToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((value == null) || (parameter == null))
			{
				return false;
			}

			return value.ToString().Equals(parameter.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((value == null) || (parameter == null))
			{
				return Binding.DoNothing;
			}

			return (bool)value ? Enum.Parse(targetType, parameter.ToString()) : Binding.DoNothing;
		}
	}
}