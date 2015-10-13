using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using ControllerSDK.ViewModels;

namespace ControllerSDK.Converters
{
	public class JournalItemTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((JournalItemType) value)
			{
				case JournalItemType.Online:
					return Brushes.Green;
				case JournalItemType.Offline:
					return Brushes.Red;
				default:
					return Brushes.Transparent;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
