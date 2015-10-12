using System;
using System.Windows.Data;
using System.Windows.Media;
using RubezhAPI.GK;

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
				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Brushes.Red;
				case XStateClass.Attention:
					return Brushes.Orange;
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