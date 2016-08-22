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
				{ Colors.AliceBlue, "Бледно-голубой" },
				{ Colors.AntiqueWhite, "Белый антик" },
				//{ Colors.Aqua, "Морская волна" },				//Colors.Cyan
				{ Colors.Aquamarine, "Аквамарин" },
				{ Colors.Azure, "Лазурный" },
				{ Colors.Beige, "Бежевый" },
				{ Colors.Bisque, "Томатный" },
				{ Colors.Black, "Черный" },
				{ Colors.BlanchedAlmond, "Чищенный миндаль" },
				{ Colors.Blue, "Синий" },
				{ Colors.BlueViolet, "Фиолетово-синий" },
				{ Colors.Brown, "Коричневый" },
				{ Colors.BurlyWood, "Желтоватый" },
				{ Colors.CadetBlue, "Серо-синий" },
				{ Colors.Chartreuse, "Шартрез" },
				{ Colors.Chocolate, "Шоколадный" },
				{ Colors.Coral, "Коралловый" },
				{ Colors.CornflowerBlue, "Васильковый" },
				{ Colors.Cornsilk, "Шелковый оттенок" },
				{ Colors.Crimson, "Малиновый" },
				{ Colors.Cyan, "зеленовато-голубой" },			//Colors.Aqua
				{ Colors.DarkBlue, "Темно-синий" },
				{ Colors.DarkCyan, "Темный циан" },
				{ Colors.DarkGoldenrod, "Темно-золотистый" },
				{ Colors.DarkGray, "Темно-серый" },
				{ Colors.DarkGreen, "Темно-зеленый" },
				{ Colors.DarkKhaki, "Темный хаки" },
				{ Colors.DarkMagenta, "Темно-пурпурный" },
				{ Colors.DarkOliveGreen, "Темный оливково-зеленый" },
				{ Colors.DarkOrange, "Темно-оранжевый" },
				{ Colors.DarkOrchid, "Темная орхидея" },
				{ Colors.DarkRed, "Темно-красный" },
				{ Colors.DarkSalmon, "Темный оранжево-розовый" },
				{ Colors.DarkSeaGreen, "Темной морской волны" },
				{ Colors.DarkSlateBlue, "Темный грифельно-синий" },
				{ Colors.DarkSlateGray, "Темный синевато-серый" },
				{ Colors.DarkTurquoise, "Темно-бирюзовый" },
				{ Colors.DarkViolet, "Темно-фиолетовый" },
				{ Colors.DeepPink, "Темно-розовый" },
				{ Colors.DeepSkyBlue, "Насыщенный небесно-голубой" },
				{ Colors.DimGray, "Тускло-серый" },
				{ Colors.DodgerBlue, "Защитно-синий" },
				{ Colors.Firebrick, "Кирпичный" },
				{ Colors.FloralWhite, "Цветочно-белый" },
				{ Colors.ForestGreen, "Цвет лесной зелени" },
				//{ Colors.Fuchsia, "Розовато-лиловый" },		//Colors.Magenta
				{ Colors.Gainsboro, "Светло-голубо-серый" },
				{ Colors.GhostWhite, "Призрачно-белый" },
				{ Colors.Gold, "Золотой" },
				{ Colors.Goldenrod, "Золотистый" },
				{ Colors.Gray, "Серый" },
				{ Colors.Green, "Зелёный" },
				{ Colors.GreenYellow, "Зелено-желтый" },
				{ Colors.Honeydew, "Медовый" },
				{ Colors.HotPink, "Теплый розовый" },
				{ Colors.IndianRed, "Киноварный" },
				{ Colors.Indigo, "Индиго" },
				{ Colors.Ivory, "Слоновой кости" },
				{ Colors.Khaki, "Хаки" },
				{ Colors.Lavender, "Бледно-лиловый" },
				{ Colors.LavenderBlush, "Лавандово-розовый" },
				{ Colors.LawnGreen, "Ярко-зеленый" },
				{ Colors.LemonChiffon, "Лимонный" },
				{ Colors.LightBlue, "Светло-синий" },
				{ Colors.LightCoral, "Коралловый светлый" },
				{ Colors.LightCyan, "Светлый циан" },
				{ Colors.LightGoldenrodYellow, "Светло-желтый золотистый" },
				{ Colors.LightGray, "Светло-серый" },
				{ Colors.LightGreen, "Светло-зеленый" },
				{ Colors.LightPink, "Светло-розовый" },
				{ Colors.LightSalmon, "Светлый оранжево-розовый" },
				{ Colors.LightSeaGreen, "Цвет морской волны, светлый" },
				{ Colors.LightSkyBlue, "Небесно-голубой светлый" },
				{ Colors.LightSlateGray, "Светлый грифельно-синий" },
				{ Colors.LightSteelBlue, "Голубой со стальным оттенком" },
				{ Colors.LightYellow, "Светло-желтый" },
				{ Colors.Lime, "Цвет лайма" },
				{ Colors.LimeGreen, "Лимонно-зеленый" },
				{ Colors.Linen, "Лён" },
				{ Colors.Magenta, "Пурпурный" },			//Colors.Fuchsia
				{ Colors.Maroon, "Темно-бордовый" },
				{ Colors.MediumAquamarine, "Аквамарин" },
				{ Colors.MediumBlue, "Синий" },
				{ Colors.MediumOrchid, "Орхидеи" },
				{ Colors.MediumPurple, "Пурпурный" },
				{ Colors.MediumSeaGreen, "Цвет морской волны" },
				{ Colors.MediumSlateBlue, "Грифельно-синий" },
				{ Colors.MediumSpringGreen, "Весенне-зеленый" },
				{ Colors.MediumTurquoise, "Бирюзовый" },
				{ Colors.MediumVioletRed, "Красно-фиолетовый" },
				{ Colors.MidnightBlue, "Полуночно-синий" },
				{ Colors.MintCream, "Мятная конфетка" },
				{ Colors.MistyRose, "Тускло-розовый" },
				{ Colors.Moccasin, "Мокасин" },
				{ Colors.NavajoWhite, "Белый-навахо" },
				{ Colors.Navy, "Темно-синий" },
				{ Colors.OldLace, "Старое кружево" },
				{ Colors.Olive, "Оливковый" },
				{ Colors.OliveDrab, "Оливковый камуфляжный" },
				{ Colors.Orange, "Оранжевый" },
				{ Colors.OrangeRed, "Оранжево-красный" },
				{ Colors.Orchid, "Орхидея" },
				{ Colors.PaleGoldenrod, "Бледно-золотистый" },
				{ Colors.PaleGreen, "Бледно-зеленый" },
				{ Colors.PaleTurquoise, "Бледно-бирюзовый" },
				{ Colors.PaleVioletRed, "Красно-фиолетовый бледный" },
				{ Colors.PapayaWhip, "Взбитые сливки с папайей" },
				{ Colors.PeachPuff, "Персиковый" },
				{ Colors.Peru, "Перу" },
				{ Colors.Pink, "Розовый" },
				{ Colors.Plum, "Темно-фиолетовый" },
				{ Colors.PowderBlue, "Синий с пороховым оттенком" },
				{ Colors.Purple, "Пурпурный" },
				{ Colors.Red, "Красный" },
				{ Colors.RosyBrown, "Розово-коричневый" },
				{ Colors.RoyalBlue, "Королевский синий" },
				{ Colors.SaddleBrown, "Кожано-коричневый" },
				{ Colors.Salmon, "Оранжево-розовый" },
				{ Colors.SandyBrown, "Песочно-коричневый" },
				{ Colors.SeaGreen, "Цвет морской волны" },
				{ Colors.SeaShell, "Морская раковина" },
				{ Colors.Sienna, "Охра" },
				{ Colors.Silver, "Серебряный" },
				{ Colors.SkyBlue, "Небесно-голубой" },
				{ Colors.SlateBlue, "Грифельно-синий" },
				{ Colors.SlateGray, "Синевато-серый" },
				{ Colors.Snow, "Белоснежный" },
				{ Colors.SpringGreen, "Весенне-зеленый" },
				{ Colors.SteelBlue, "Синий со стальным оттенком" },
				{ Colors.Tan, "Рыжевато-коричневый" },
				{ Colors.Teal, " Зелено-голубой" },
				{ Colors.Thistle, "Цветок чертополоха" },
				{ Colors.Tomato, "Помидор" },
				{ Colors.Transparent, "Прозрачный" },
				{ Colors.Turquoise, "Бирюзовый" },
				{ Colors.Violet, "Фиолетовый" },
				{ Colors.Wheat, "Пшеничный" },
				{ Colors.White, "Белый" },
				{ Colors.WhiteSmoke, "Белый дым" },
				{ Colors.Yellow, "Желтый" },
				{ Colors.YellowGreen, "Желто-зеленый" },
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