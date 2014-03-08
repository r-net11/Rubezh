using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls.Converters
{
	public class ColorToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Color clr = (Color)value;
			var lstKnownColors = GetKnownColors();
			string strColorName = (
			   from c in lstKnownColors
			   where
				  c.Value.A == clr.A &&
				  c.Value.R == clr.R &&
				  c.Value.G == clr.G &&
				  c.Value.B == clr.B
			   select c.Key
			   ).FirstOrDefault();

			return strColorName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static List<KeyValuePair<string, Color>> GetKnownColors()
		{
			List<KeyValuePair<string, Color>> lst = new List<KeyValuePair<string, Color>>();
			Type ColorType = typeof(System.Windows.Media.Colors);
			PropertyInfo[] arrPiColors = ColorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

			foreach (PropertyInfo pi in arrPiColors)
				lst.Add(new KeyValuePair<string, Color>(pi.Name, (Color)pi.GetValue(null, null)));
			return lst;
		}
	}
}
