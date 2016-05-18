using System;
using System.Windows.Data;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;

namespace Controls.Converters
{
	public class XSoundTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((SoundType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (SoundType)value;
		}
	}
}