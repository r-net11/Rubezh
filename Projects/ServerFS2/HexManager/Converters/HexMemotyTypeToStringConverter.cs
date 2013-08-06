using System;
using System.Windows.Data;
using FiresecAPI;
using HexManager.Models;

namespace HexManager.Converters
{
	public class HexMemotyTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (value is HexMemoryType)
			{
				HexMemoryType hexMemoryType = (HexMemoryType)value;
				return hexMemoryType.ToDescription();
			}
			return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			return (HexMemoryType)value;
        }
    }
}