
namespace Infrustructure.Plans
{
	public static class ColorExtentions
	{
		public static System.Windows.Media.Color ToWindowsColor(this StrazhAPI.Color fromColor)
		{
			return System.Windows.Media.Color.FromArgb(fromColor.A, fromColor.R, fromColor.G, fromColor.B);
		}

		public static StrazhAPI.Color ToStruzhColor(this System.Windows.Media.Color fromColor)
		{
			return StrazhAPI.Color.FromArgb(fromColor.A, fromColor.R, fromColor.G, fromColor.B);
		}
	}
}
