using System.ComponentModel;

namespace Infrastructure.Common.Theme
{
	public enum Theme
	{
		[DescriptionAttribute("Синяя тема")]
		BlueTheme,
		
		[DescriptionAttribute("Серая тема")]
		GrayTheme,

		[DescriptionAttribute("Голубая тема")]
		SkyblueTheme,

		//[DescriptionAttribute("Светлая тема")]
		//LightTheme,
	}
}