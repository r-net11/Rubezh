
namespace Infrastructure.Plans
{
	public static class StretchExtentions
	{
		public static System.Windows.Media.Stretch ToWindowsStretch(this Common.Stretch fromStretch)
		{
			return (System.Windows.Media.Stretch)(int)fromStretch;
		}

		public static Common.Stretch ToRubezhStretch(this System.Windows.Media.Stretch fromStretch)
		{
			return (Common.Stretch)(int)fromStretch;
		}
	}
}
