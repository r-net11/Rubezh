using System;
using System.Windows.Data;

namespace GKModule.Converters
{
	public class DescriptorNoToStringConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			ushort descriptorNo = (ushort)value;
			if (descriptorNo > 0)
			{
				return descriptorNo.ToString();
			}
			return "";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}