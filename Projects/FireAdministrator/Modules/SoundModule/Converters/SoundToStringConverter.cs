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
			if (value != null &&
				(FiresecClient.FileHelper.SoundsList.Any(ss => ss == value.ToString()) ||
				(FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds != null &&
				 FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(us => us.Name == value.ToString()))))
				return value.ToString();
			return SoundAssignmentViewModel.DefaultName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}