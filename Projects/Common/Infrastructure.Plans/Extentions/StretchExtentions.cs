
using Common;
namespace Infrastructure.Plans
{
	public static class StretchExtentions
	{
		public static System.Windows.Media.Stretch ToWindowsStretch(this Stretch fromStretch)
		{
			return (System.Windows.Media.Stretch)(int)fromStretch;
		}

		public static Stretch ToRubezhStretch(this System.Windows.Media.Stretch fromStretch)
		{
			return (Stretch)(int)fromStretch;
		}
	}
}
