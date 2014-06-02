using System.Windows.Data;
using FiresecAPI;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.Converters
{
	class ElementZoneTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((ElementZoneType)value).ToDescription();
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}