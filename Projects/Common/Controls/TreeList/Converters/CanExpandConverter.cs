using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Controls.TreeList.Converters
{
	internal class CanExpandConverter : IValueConverter
	{
		public object Convert(object obj, Type type, object parameter, CultureInfo culture)
		{
			return (bool)obj ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object obj, Type type, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
