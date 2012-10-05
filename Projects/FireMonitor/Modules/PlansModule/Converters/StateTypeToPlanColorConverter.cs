using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI;

namespace PlansModule.Converters
{
	public class StateTypeToPlanColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return Brushes.Transparent;
			StateType stateType = (StateType)value;
			switch(stateType)
			{
				case StateType.Fire:
					return Brushes.Red;

				case StateType.Attention:
					return Brushes.Yellow;

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