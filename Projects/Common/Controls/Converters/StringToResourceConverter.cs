using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class StringToResourceConverter : IValueConverter
	{
		static List<string> values = new List<string>();
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var resourceName = value as string;
			if (resourceName == null)
				return DependencyProperty.UnsetValue;
			var resource = Application.Current.TryFindResource(resourceName);
			if (resource == null)
			{
				if (char.IsUpper(resourceName.First()))
					resource = Application.Current.TryFindResource((value as string).First().ToString().ToLower() + (value as string).Substring(1));
				else
					resource = Application.Current.TryFindResource((value as string).First().ToString().ToUpper() + (value as string).Substring(1));
			}
			if (resource != null)
				return resource;
			
			if(!values.Contains(value.ToString()))
				values.Add(value.ToString());
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
