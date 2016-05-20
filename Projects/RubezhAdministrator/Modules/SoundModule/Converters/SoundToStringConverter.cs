using System;
using System.Linq;
using System.Windows.Data;
using SoundsModule.ViewModels;

namespace SoundsModule.Converters
{
	public class SoundToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && RubezhClient.FileHelper.SoundsList.Any(x => x == value.ToString()))
				return value.ToString();
			return SoundViewModel.DefaultName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}