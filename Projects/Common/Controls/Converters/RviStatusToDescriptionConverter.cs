using RubezhAPI;
using RubezhAPI.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Controls.Converters
{
	class RviStatusToDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var rviStatus = (RviStatus)value;
			return rviStatus.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}