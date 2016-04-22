
namespace Infrastructure.Plans
{
	public static class ColorExtentions
	{
		public static System.Windows.Media.Color ToWindowsColor(this RubezhAPI.Color fromColor)
		{
			return System.Windows.Media.Color.FromArgb(fromColor.A, fromColor.R, fromColor.G, fromColor.B);
		}

		public static RubezhAPI.Color ToRubezhColor(this System.Windows.Media.Color fromColor)
		{
			return RubezhAPI.Color.FromArgb(fromColor.A, fromColor.R, fromColor.G, fromColor.B);
		}
	}
}
