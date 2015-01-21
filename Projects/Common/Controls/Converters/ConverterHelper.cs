using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Controls.Converters
{
	public static class ConverterHelper
	{
		static List<string> values = new List<string>();
		public static object GetResource(object value)
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

			if (!values.Contains(value.ToString()))
				values.Add(value.ToString());
			return DependencyProperty.UnsetValue;
		}
	}
}
