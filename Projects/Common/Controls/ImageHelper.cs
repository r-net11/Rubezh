using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controls
{
	public static class ImageHelper
	{
		public const string IconPath = "/Controls;component/Images/";
		public const string IconExtension = ".png";

		public static string GetImagePath(string name)
		{
			var path = name;
			if (!path.EndsWith(IconExtension))
				path += IconExtension;
			if (!path.StartsWith(IconPath))
				path = IconPath + path;
			return path;
		}
		public static string ToImagePath(this string name)
		{
			return GetImagePath(name);
		}
	}
}
