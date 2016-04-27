using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

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
				{ Colors.AliceBlue,             Resources.Language.ColorUtilities.AliceBlue },
				{ Colors.AntiqueWhite,          Resources.Language.ColorUtilities.AntiqueWhite },
				//{ Colors.Aqua, "Морская волна" },				//Colors.Cyan
				{ Colors.Aquamarine,            Resources.Language.ColorUtilities.Aquamarine },
				{ Colors.Azure,                 Resources.Language.ColorUtilities.Azure },
				{ Colors.Beige,                 Resources.Language.ColorUtilities.Beige },
				{ Colors.Bisque,                Resources.Language.ColorUtilities.Bisque },
				{ Colors.Black,                 Resources.Language.ColorUtilities.Black },
				{ Colors.BlanchedAlmond,        Resources.Language.ColorUtilities.BlanchedAlmond },
				{ Colors.Blue,                  Resources.Language.ColorUtilities.Blue },
				{ Colors.BlueViolet,            Resources.Language.ColorUtilities.BlueViolet },
				{ Colors.Brown,                 Resources.Language.ColorUtilities.Brown },
				{ Colors.BurlyWood,             Resources.Language.ColorUtilities.BurlyWood },
				{ Colors.CadetBlue,             Resources.Language.ColorUtilities.CadetBlue },
				{ Colors.Chartreuse,            Resources.Language.ColorUtilities.Chartreuse },
				{ Colors.Chocolate,             Resources.Language.ColorUtilities.Chocolate },
				{ Colors.Coral,                 Resources.Language.ColorUtilities.Coral },
				{ Colors.CornflowerBlue,        Resources.Language.ColorUtilities.CornflowerBlue },
				{ Colors.Cornsilk,              Resources.Language.ColorUtilities.Cornsilk },
				{ Colors.Crimson,               Resources.Language.ColorUtilities.Crimson },
				{ Colors.Cyan,                  Resources.Language.ColorUtilities.Cyan },			//Colors.Aqua
				{ Colors.DarkBlue,              Resources.Language.ColorUtilities.DarkBlue },
				{ Colors.DarkCyan,              Resources.Language.ColorUtilities.DarkCyan },
				{ Colors.DarkGoldenrod,         Resources.Language.ColorUtilities.DarkGoldenrod },
				{ Colors.DarkGray,              Resources.Language.ColorUtilities.DarkGray },
				{ Colors.DarkGreen,             Resources.Language.ColorUtilities.DarkGreen },
				{ Colors.DarkKhaki,             Resources.Language.ColorUtilities.DarkKhaki },
				{ Colors.DarkMagenta,           Resources.Language.ColorUtilities.DarkMagenta },
				{ Colors.DarkOliveGreen,        Resources.Language.ColorUtilities.DarkOliveGreen },
				{ Colors.DarkOrange,            Resources.Language.ColorUtilities.DarkOrange },
				{ Colors.DarkOrchid,            Resources.Language.ColorUtilities.DarkOrchid },
				{ Colors.DarkRed,               Resources.Language.ColorUtilities.DarkRed },
				{ Colors.DarkSalmon,            Resources.Language.ColorUtilities.DarkSalmon },
				{ Colors.DarkSeaGreen,          Resources.Language.ColorUtilities.DarkSeaGreen },
				{ Colors.DarkSlateBlue,         Resources.Language.ColorUtilities.DarkSlateBlue },
				{ Colors.DarkSlateGray,         Resources.Language.ColorUtilities.DarkSlateGray },
				{ Colors.DarkTurquoise,         Resources.Language.ColorUtilities.DarkTurquoise },
				{ Colors.DarkViolet,            Resources.Language.ColorUtilities.DarkViolet },
				{ Colors.DeepPink,              Resources.Language.ColorUtilities.DeepPink },
				{ Colors.DeepSkyBlue,           Resources.Language.ColorUtilities.DeepSkyBlue },
				{ Colors.DimGray,               Resources.Language.ColorUtilities.DimGray },
				{ Colors.DodgerBlue,            Resources.Language.ColorUtilities.DodgerBlue },
				{ Colors.Firebrick,             Resources.Language.ColorUtilities.Firebrick },
				{ Colors.FloralWhite,           Resources.Language.ColorUtilities.FloralWhite },
				{ Colors.ForestGreen,           Resources.Language.ColorUtilities.ForestGreen },
				//{ Colors.Fuchsia, "Розовато-лиловый" },		//Colors.Magenta
				{ Colors.Gainsboro,             Resources.Language.ColorUtilities.Gainsboro },
				{ Colors.GhostWhite,            Resources.Language.ColorUtilities.GhostWhite },
				{ Colors.Gold,                  Resources.Language.ColorUtilities.Gold },
				{ Colors.Goldenrod,             Resources.Language.ColorUtilities.Goldenrod },
				{ Colors.Gray,                  Resources.Language.ColorUtilities.Gray },
				{ Colors.Green,                 Resources.Language.ColorUtilities.Green },
				{ Colors.GreenYellow,           Resources.Language.ColorUtilities.GreenYellow },
				{ Colors.Honeydew,              Resources.Language.ColorUtilities.Honeydew },
				{ Colors.HotPink,               Resources.Language.ColorUtilities.HotPink },
				{ Colors.IndianRed,             Resources.Language.ColorUtilities.IndianRed },
				{ Colors.Indigo,                Resources.Language.ColorUtilities.Indigo },
				{ Colors.Ivory,                 Resources.Language.ColorUtilities.Ivory },
				{ Colors.Khaki,                 Resources.Language.ColorUtilities.Khaki },
				{ Colors.Lavender,              Resources.Language.ColorUtilities.Lavender },
				{ Colors.LavenderBlush,         Resources.Language.ColorUtilities.LavenderBlush },
				{ Colors.LawnGreen,             Resources.Language.ColorUtilities.LawnGreen },
				{ Colors.LemonChiffon,          Resources.Language.ColorUtilities.LemonChiffon },
				{ Colors.LightBlue,             Resources.Language.ColorUtilities.LightBlue },
				{ Colors.LightCoral,            Resources.Language.ColorUtilities.LightCoral },
				{ Colors.LightCyan,             Resources.Language.ColorUtilities.LightCyan },
				{ Colors.LightGoldenrodYellow,  Resources.Language.ColorUtilities.LightGoldenrodYellow },
				{ Colors.LightGray,             Resources.Language.ColorUtilities.LightGray },
				{ Colors.LightGreen,            Resources.Language.ColorUtilities.LightGreen },
				{ Colors.LightPink,             Resources.Language.ColorUtilities.LightPink },
				{ Colors.LightSalmon,           Resources.Language.ColorUtilities.LightSalmon },
				{ Colors.LightSeaGreen,         Resources.Language.ColorUtilities.LightSeaGreen },
				{ Colors.LightSkyBlue,          Resources.Language.ColorUtilities.LightSkyBlue },
				{ Colors.LightSlateGray,        Resources.Language.ColorUtilities.LightSlateGray },
				{ Colors.LightSteelBlue,        Resources.Language.ColorUtilities.LightSteelBlue },
				{ Colors.LightYellow,           Resources.Language.ColorUtilities.LightYellow },
				{ Colors.Lime,                  Resources.Language.ColorUtilities.Lime },
				{ Colors.LimeGreen,             Resources.Language.ColorUtilities.LimeGreen },
				{ Colors.Linen,                 Resources.Language.ColorUtilities.Linen },
				{ Colors.Magenta,               Resources.Language.ColorUtilities.Magenta },
				{ Colors.Maroon,                Resources.Language.ColorUtilities.Maroon },
				{ Colors.MediumAquamarine,      Resources.Language.ColorUtilities.MediumAquamarine },
				{ Colors.MediumBlue,            Resources.Language.ColorUtilities.MediumBlue },
				{ Colors.MediumOrchid,          Resources.Language.ColorUtilities.MediumOrchid },
				{ Colors.MediumPurple,          Resources.Language.ColorUtilities.MediumPurple },
				{ Colors.MediumSeaGreen,        Resources.Language.ColorUtilities.MediumSeaGreen },
				{ Colors.MediumSlateBlue,       Resources.Language.ColorUtilities.MediumSlateBlue },
				{ Colors.MediumSpringGreen,     Resources.Language.ColorUtilities.MediumSpringGreen },
				{ Colors.MediumTurquoise,       Resources.Language.ColorUtilities.MediumTurquoise },
				{ Colors.MediumVioletRed,       Resources.Language.ColorUtilities.MediumVioletRed },
				{ Colors.MidnightBlue,          Resources.Language.ColorUtilities.MidnightBlue },
				{ Colors.MintCream,             Resources.Language.ColorUtilities.MintCream },
				{ Colors.MistyRose,             Resources.Language.ColorUtilities.MistyRose },
				{ Colors.Moccasin,              Resources.Language.ColorUtilities.Moccasin },
				{ Colors.NavajoWhite,           Resources.Language.ColorUtilities.NavajoWhite },
				{ Colors.Navy,                  Resources.Language.ColorUtilities.Navy },
				{ Colors.OldLace,               Resources.Language.ColorUtilities.OldLace },
				{ Colors.Olive,                 Resources.Language.ColorUtilities.Olive },
				{ Colors.OliveDrab,             Resources.Language.ColorUtilities.OliveDrab },
				{ Colors.Orange,                Resources.Language.ColorUtilities.Orange },
				{ Colors.OrangeRed,             Resources.Language.ColorUtilities.OrangeRed },
				{ Colors.Orchid,                Resources.Language.ColorUtilities.Orchid },
				{ Colors.PaleGoldenrod,         Resources.Language.ColorUtilities.PaleGoldenrod },
				{ Colors.PaleGreen,             Resources.Language.ColorUtilities.PaleGreen },
				{ Colors.PaleTurquoise,         Resources.Language.ColorUtilities.PaleTurquoise },
				{ Colors.PaleVioletRed,         Resources.Language.ColorUtilities.PaleVioletRed },
				{ Colors.PapayaWhip,            Resources.Language.ColorUtilities.PapayaWhip },
				{ Colors.PeachPuff,             Resources.Language.ColorUtilities.PeachPuff },
				{ Colors.Peru,                  Resources.Language.ColorUtilities.Peru },
				{ Colors.Pink,                  Resources.Language.ColorUtilities.Pink },
				{ Colors.Plum,                  Resources.Language.ColorUtilities.Plum },
				{ Colors.PowderBlue,            Resources.Language.ColorUtilities.PowderBlue },
				{ Colors.Purple,                Resources.Language.ColorUtilities.Purple },
				{ Colors.Red,                   Resources.Language.ColorUtilities.Red },
				{ Colors.RosyBrown,             Resources.Language.ColorUtilities.RosyBrown },
				{ Colors.RoyalBlue,             Resources.Language.ColorUtilities.RoyalBlue },
				{ Colors.SaddleBrown,           Resources.Language.ColorUtilities.SaddleBrown },
				{ Colors.Salmon,                Resources.Language.ColorUtilities.Salmon },
				{ Colors.SandyBrown,            Resources.Language.ColorUtilities.SandyBrown },
				{ Colors.SeaGreen,              Resources.Language.ColorUtilities.SeaGreen },
				{ Colors.SeaShell,              Resources.Language.ColorUtilities.SeaShell },
				{ Colors.Sienna,                Resources.Language.ColorUtilities.Sienna },
				{ Colors.Silver,                Resources.Language.ColorUtilities.Silver },
				{ Colors.SkyBlue,               Resources.Language.ColorUtilities.SkyBlue },
				{ Colors.SlateBlue,             Resources.Language.ColorUtilities.SlateBlue },
				{ Colors.SlateGray,             Resources.Language.ColorUtilities.SlateGray },
				{ Colors.Snow,                  Resources.Language.ColorUtilities.Snow },
				{ Colors.SpringGreen,           Resources.Language.ColorUtilities.SpringGreen },
				{ Colors.SteelBlue,             Resources.Language.ColorUtilities.SteelBlue },
				{ Colors.Tan,                   Resources.Language.ColorUtilities.Tan },
				{ Colors.Teal,                  Resources.Language.ColorUtilities.Teal },
				{ Colors.Thistle,               Resources.Language.ColorUtilities.Thistle },
				{ Colors.Tomato,                Resources.Language.ColorUtilities.Tomato },
				{ Colors.Transparent,           Resources.Language.ColorUtilities.Transparent },
				{ Colors.Turquoise,             Resources.Language.ColorUtilities.Turquoise },
				{ Colors.Violet,                Resources.Language.ColorUtilities.Violet },
				{ Colors.Wheat,                 Resources.Language.ColorUtilities.Wheat },
				{ Colors.White,                 Resources.Language.ColorUtilities.White },
				{ Colors.WhiteSmoke,            Resources.Language.ColorUtilities.WhiteSmoke },
				{ Colors.Yellow,                Resources.Language.ColorUtilities.Yellow },
				{ Colors.YellowGreen,           Resources.Language.ColorUtilities.YellowGreen },
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