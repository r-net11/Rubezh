using StrazhAPI;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class ColorToSystemColorConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Color)
			{
				var c = (Color)value;
				return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
			}
			var black = System.Drawing.Color.Black;
			return System.Windows.Media.Color.FromArgb(black.A, black.R, black.G, black.B);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is System.Windows.Media.Color)
			{
				var c = (System.Windows.Media.Color)value;
				return new Color { A = c.A, B = c.B, G = c.G, R = c.R };
			}
			var black = System.Drawing.Color.Black;
			return new Color { A = black.A, B = black.B, G = black.G, R = black.R }; 
		}
	}
}