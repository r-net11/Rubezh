using System;
using System.Windows.Data;
using RubezhAPI;
using Infrastructure.Common.Theme;

namespace SettingsModule.Converters
{
	public class ThemeConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Theme)
			{
				return ((Theme)value).ToDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
