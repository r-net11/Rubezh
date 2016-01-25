using System;
using System.Windows.Data;

namespace Controls.Converters
{
	class MultiBooleanConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var andCondition = parameter != null;
			var result = parameter != null;
			foreach (object value in values)
				if (value is bool)
				{
					if (andCondition)
						result = result && (bool) value;
					else
						result = result || (bool)value;
				}
			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}