using System;
using System.Globalization;
using System.Windows.Data;

namespace Controls.Converters
{
	/// <summary>
	/// Convert Level to left margin
	/// Pass a prarameter if you want a unit length other than 19.0.
	/// </summary>
	public class LevelToIndentConverter : IValueConverter
	{
		public object Convert(object o, Type type, object parameter, CultureInfo culture)
		{
			Double indentSize = 0;
			if (parameter != null)
				Double.TryParse(parameter.ToString(), out indentSize);

			return (int)o * indentSize;
		}

		public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

	}

}
