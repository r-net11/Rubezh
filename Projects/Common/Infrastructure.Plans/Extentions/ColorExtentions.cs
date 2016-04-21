
namespace Infrastructure.Plans
{
	public static class ColorExtentions
	{
		public static System.Windows.Media.Color ToWindowsColor(this Common.Color fromColor)
		{
			return System.Windows.Media.Color.FromArgb(fromColor.A, fromColor.R, fromColor.G, fromColor.B);
		}

		public static Common.Color ToRubezhColor(this System.Windows.Media.Color fromColor)
		{
			return Common.Color.FromArgb(fromColor.A, fromColor.R, fromColor.G, fromColor.B);
		}
	}
}
