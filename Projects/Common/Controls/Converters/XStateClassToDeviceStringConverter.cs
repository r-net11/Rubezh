using System;
using System.Windows.Data;
using FiresecAPI;
using XFiresecAPI;

namespace Controls.Converters
{
    public class XStateClassToDeviceStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			XStateClass stateClass = (XStateClass)value;
			if (stateClass == XStateClass.Fire1)
				return "Сработка 1";
			if (stateClass == XStateClass.Fire1)
				return "Сработка 2";
			return stateClass.ToDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (XStateClass)value;
        }
    }
}