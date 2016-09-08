using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Localization.Common.Common;

namespace Common
{
	public static class ColorUtilities
	{
		public static Dictionary<string, Color> KnownColors { get; private set; }

		public static Dictionary<Color, string> ColorNames { get; private set; }

		static ColorUtilities()
		{
			KnownColors = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static).ToDictionary<PropertyInfo, string, Color>(p => p.Name, p => (Color)p.GetValue(null, null));
			ColorNames = new Dictionary<Color, string>()
			{
				{ Colors.AliceBlue, ColorUtilitiesResources.AliceBlue },
				{ Colors.AntiqueWhite, ColorUtilitiesResources.AntiqueWhite },
				//{ Colors.Aqua, ColorUtilitiesResources.Aqua },//Colors.Cyan
				{ Colors.Aquamarine, ColorUtilitiesResources.Aquamarine },
				{ Colors.Azure, ColorUtilitiesResources.Azure },
				{ Colors.Beige, ColorUtilitiesResources.Beige },
				{ Colors.Bisque, ColorUtilitiesResources.Bisque },
				{ Colors.Black, ColorUtilitiesResources.Black },
				{ Colors.BlanchedAlmond, ColorUtilitiesResources.BlanchedAlmond },
				{ Colors.Blue, ColorUtilitiesResources.Blue },
				{ Colors.BlueViolet, ColorUtilitiesResources.BlueViolet },
				{ Colors.Brown, ColorUtilitiesResources.Brown },
				{ Colors.BurlyWood, ColorUtilitiesResources.BurlyWood },
				{ Colors.CadetBlue, ColorUtilitiesResources.CadetBlue },
				{ Colors.Chartreuse, ColorUtilitiesResources.Chartreuse },
				{ Colors.Chocolate, ColorUtilitiesResources.Chocolate },
				{ Colors.Coral, ColorUtilitiesResources.Coral },
				{ Colors.CornflowerBlue, ColorUtilitiesResources.CornflowerBlue },
				{ Colors.Cornsilk, ColorUtilitiesResources.Cornsilk },
				{ Colors.Crimson, ColorUtilitiesResources.Crimson },
				{ Colors.Cyan, ColorUtilitiesResources.Cyan },//Colors.Aqua
				{ Colors.DarkBlue, ColorUtilitiesResources.DarkBlue },
				{ Colors.DarkCyan, ColorUtilitiesResources.DarkCyan },
				{ Colors.DarkGoldenrod, ColorUtilitiesResources.DarkGoldenrod },
				{ Colors.DarkGray, ColorUtilitiesResources.DarkGray },
				{ Colors.DarkGreen, ColorUtilitiesResources.DarkGreen },
				{ Colors.DarkKhaki, ColorUtilitiesResources.DarkKhaki },
				{ Colors.DarkMagenta, ColorUtilitiesResources.DarkMagenta },
				{ Colors.DarkOliveGreen, ColorUtilitiesResources.DarkOliveGreen },
				{ Colors.DarkOrange, ColorUtilitiesResources.DarkOrange },
				{ Colors.DarkOrchid, ColorUtilitiesResources.DarkOrchid },
				{ Colors.DarkRed, ColorUtilitiesResources.DarkRed },
				{ Colors.DarkSalmon, ColorUtilitiesResources.DarkSalmon },
				{ Colors.DarkSeaGreen, ColorUtilitiesResources.DarkSeaGreen },
				{ Colors.DarkSlateBlue, ColorUtilitiesResources.DarkSlateBlue },
				{ Colors.DarkSlateGray, ColorUtilitiesResources.DarkSlateGray },
				{ Colors.DarkTurquoise, ColorUtilitiesResources.DarkTurquoise },
				{ Colors.DarkViolet, ColorUtilitiesResources.DarkViolet },
				{ Colors.DeepPink, ColorUtilitiesResources.DeepPink },
				{ Colors.DeepSkyBlue, ColorUtilitiesResources.DeepSkyBlue },
				{ Colors.DimGray, ColorUtilitiesResources.DimGray },
				{ Colors.DodgerBlue, ColorUtilitiesResources.DodgerBlue },
				{ Colors.Firebrick, ColorUtilitiesResources.Firebrick },
				{ Colors.FloralWhite, ColorUtilitiesResources.FloralWhite },
				{ Colors.ForestGreen, ColorUtilitiesResources.ForestGreen },
				//{ Colors.Fuchsia, ColorUtilitiesResources.Fuchsia },//Colors.Magenta
				{ Colors.Gainsboro, ColorUtilitiesResources.Gainsboro },
				{ Colors.GhostWhite, ColorUtilitiesResources.GhostWhite },
				{ Colors.Gold, ColorUtilitiesResources.Gold },
				{ Colors.Goldenrod, ColorUtilitiesResources.Goldenrod },
				{ Colors.Gray, ColorUtilitiesResources.Gray },
				{ Colors.Green, ColorUtilitiesResources.Green },
				{ Colors.GreenYellow, ColorUtilitiesResources.GreenYellow },
				{ Colors.Honeydew, ColorUtilitiesResources.Honeydew },
				{ Colors.HotPink, ColorUtilitiesResources.HotPink },
				{ Colors.IndianRed, ColorUtilitiesResources.IndianRed },
				{ Colors.Indigo, ColorUtilitiesResources.Indigo },
				{ Colors.Ivory, ColorUtilitiesResources.Ivory },
				{ Colors.Khaki, ColorUtilitiesResources.Khaki },
				{ Colors.Lavender, ColorUtilitiesResources.Lavender },
				{ Colors.LavenderBlush, ColorUtilitiesResources.LavenderBlush },
				{ Colors.LawnGreen, ColorUtilitiesResources.LawnGreen },
				{ Colors.LemonChiffon, ColorUtilitiesResources.LemonChiffon },
				{ Colors.LightBlue, ColorUtilitiesResources.LightBlue },
				{ Colors.LightCoral, ColorUtilitiesResources.LightCoral },
				{ Colors.LightCyan, ColorUtilitiesResources.LightCyan },
				{ Colors.LightGoldenrodYellow, ColorUtilitiesResources.LightGoldenrodYellow },
				{ Colors.LightGray, ColorUtilitiesResources.LightGray },
				{ Colors.LightGreen, ColorUtilitiesResources.LightGreen },
				{ Colors.LightPink, ColorUtilitiesResources.LightPink },
				{ Colors.LightSalmon, ColorUtilitiesResources.LightSalmon },
				{ Colors.LightSeaGreen, ColorUtilitiesResources.LightSeaGreen },
				{ Colors.LightSkyBlue, ColorUtilitiesResources.LightSkyBlue },
				{ Colors.LightSlateGray, ColorUtilitiesResources.LightSlateGray },
				{ Colors.LightSteelBlue, ColorUtilitiesResources.LightSteelBlue },
				{ Colors.LightYellow, ColorUtilitiesResources.LightYellow },
				{ Colors.Lime, ColorUtilitiesResources.Lime },
				{ Colors.LimeGreen, ColorUtilitiesResources.LimeGreen },
				{ Colors.Linen, ColorUtilitiesResources.Linen },
				{ Colors.Magenta, ColorUtilitiesResources.Magenta },//Colors.Fuchsia
				{ Colors.Maroon, ColorUtilitiesResources.Maroon },
				{ Colors.MediumAquamarine, ColorUtilitiesResources.MediumAquamarine },
				{ Colors.MediumBlue, ColorUtilitiesResources.MediumBlue },
				{ Colors.MediumOrchid, ColorUtilitiesResources.MediumOrchid },
				{ Colors.MediumPurple, ColorUtilitiesResources.MediumPurple },
				{ Colors.MediumSeaGreen, ColorUtilitiesResources.MediumSeaGreen },
				{ Colors.MediumSlateBlue, ColorUtilitiesResources.MediumSlateBlue },
				{ Colors.MediumSpringGreen, ColorUtilitiesResources.MediumSpringGreen },
				{ Colors.MediumTurquoise, ColorUtilitiesResources.MediumTurquoise },
				{ Colors.MediumVioletRed, ColorUtilitiesResources.MediumVioletRed },
				{ Colors.MidnightBlue, ColorUtilitiesResources.MidnightBlue },
				{ Colors.MintCream, ColorUtilitiesResources.MintCream },
				{ Colors.MistyRose, ColorUtilitiesResources.MistyRose },
				{ Colors.Moccasin, ColorUtilitiesResources.Moccasin },
				{ Colors.NavajoWhite, ColorUtilitiesResources.NavajoWhite },
				{ Colors.Navy, ColorUtilitiesResources.Navy },
				{ Colors.OldLace, ColorUtilitiesResources.OldLace },
				{ Colors.Olive, ColorUtilitiesResources.Olive },
				{ Colors.OliveDrab, ColorUtilitiesResources.OliveDrab },
				{ Colors.Orange, ColorUtilitiesResources.Orange },
				{ Colors.OrangeRed, ColorUtilitiesResources.OrangeRed },
				{ Colors.Orchid, ColorUtilitiesResources.Orchid },
				{ Colors.PaleGoldenrod, ColorUtilitiesResources.PaleGoldenrod },
				{ Colors.PaleGreen, ColorUtilitiesResources.PaleGreen },
				{ Colors.PaleTurquoise, ColorUtilitiesResources.PaleTurquoise },
				{ Colors.PaleVioletRed, ColorUtilitiesResources.PaleVioletRed },
				{ Colors.PapayaWhip, ColorUtilitiesResources.PapayaWhip },
				{ Colors.PeachPuff, ColorUtilitiesResources.PeachPuff },
				{ Colors.Peru, ColorUtilitiesResources.Peru },
				{ Colors.Pink, ColorUtilitiesResources.Pink },
				{ Colors.Plum, ColorUtilitiesResources.Plum },
				{ Colors.PowderBlue, ColorUtilitiesResources.PowderBlue },
				{ Colors.Purple, ColorUtilitiesResources.Purple },
				{ Colors.Red, ColorUtilitiesResources.Red },
				{ Colors.RosyBrown, ColorUtilitiesResources.RosyBrown },
				{ Colors.RoyalBlue, ColorUtilitiesResources.RoyalBlue },
				{ Colors.SaddleBrown, ColorUtilitiesResources.SaddleBrown },
				{ Colors.Salmon, ColorUtilitiesResources.Salmon },
				{ Colors.SandyBrown, ColorUtilitiesResources.SandyBrown },
				{ Colors.SeaGreen, ColorUtilitiesResources.SeaGreen },
				{ Colors.SeaShell, ColorUtilitiesResources.SeaShell },
				{ Colors.Sienna, ColorUtilitiesResources.Sienna },
				{ Colors.Silver, ColorUtilitiesResources.Silver },
				{ Colors.SkyBlue, ColorUtilitiesResources.SkyBlue },
				{ Colors.SlateBlue, ColorUtilitiesResources.SlateBlue },
				{ Colors.SlateGray, ColorUtilitiesResources.SlateGray },
				{ Colors.Snow, ColorUtilitiesResources.Snow },
				{ Colors.SpringGreen, ColorUtilitiesResources.SpringGreen },
				{ Colors.SteelBlue, ColorUtilitiesResources.SteelBlue },
				{ Colors.Tan, ColorUtilitiesResources.Tan },
				{ Colors.Teal, " Зелено-голубой" },
				{ Colors.Thistle, ColorUtilitiesResources.Thistle },
				{ Colors.Tomato, ColorUtilitiesResources.Tomato },
				{ Colors.Transparent, ColorUtilitiesResources.Transparent },
				{ Colors.Turquoise, ColorUtilitiesResources.Turquoise },
				{ Colors.Violet, ColorUtilitiesResources.Violet },
				{ Colors.Wheat, ColorUtilitiesResources.Wheat },
				{ Colors.White, ColorUtilitiesResources.White },
				{ Colors.WhiteSmoke, ColorUtilitiesResources.WhiteSmoke },
				{ Colors.Yellow, ColorUtilitiesResources.Yellow },
				{ Colors.YellowGreen, ColorUtilitiesResources.YellowGreen },
			};
		}

		public static string GetColorName(this Color color)
		{
			string str = (from kvp in KnownColors where kvp.Value.Equals(color) select kvp.Key).FirstOrDefault<string>();
			if (string.IsNullOrEmpty(str))
				str = color.ToString();
			return str;
		}

		public static string FormatColorString(string stringToFormat, bool isUsingAlphaChannel)
		{
			return !isUsingAlphaChannel && (stringToFormat.Length == 9) ? stringToFormat.Remove(1, 2) : stringToFormat;
		}
	}
}