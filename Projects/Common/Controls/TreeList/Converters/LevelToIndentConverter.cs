using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace Controls.TreeList.Converters
{
	internal class LevelToIndentConverter : IValueConverter
	{
		private const double IndentSize = 19.0;

		public object Convert(object obj, Type type, object parameter, CultureInfo culture)
		{
			return new Thickness((int)obj * IndentSize, 0, 0, 0);
		}
		public object ConvertBack(object obj, Type type, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
