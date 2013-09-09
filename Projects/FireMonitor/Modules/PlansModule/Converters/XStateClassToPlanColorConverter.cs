using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI;
using XFiresecAPI;

namespace PlansModule.Converters
{
	public class XStateClassToPlanColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return Brushes.Transparent;
			XStateClass stateType = (XStateClass)value;
			switch (stateType)
			{
				case XStateClass.Attention:
				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Brushes.Red;

				default:
					return Brushes.Transparent;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}