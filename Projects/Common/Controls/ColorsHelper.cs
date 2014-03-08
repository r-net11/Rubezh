using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Controls
{
	public class ColorsHelper
	{
		public static List<Color> AvailableColors
		{
			get
			{
				var availableColors = new List<Color>();
				var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
				var colors = colorProperties.Select(prop => (Color)prop.GetValue(null, null));
				foreach (Color myColor in colors)
				{
					availableColors.Add(myColor);
				}
				availableColors.Remove(Colors.Transparent);
				availableColors.Insert(0, Colors.Transparent);

				return availableColors;
			}
		}
	}
}
