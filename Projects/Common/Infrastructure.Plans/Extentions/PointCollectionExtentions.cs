
namespace Infrastructure.Plans
{
	public static class PointCollectionExtentions
	{
		public static System.Windows.Media.PointCollection ToWindowsPointCollection(this Common.PointCollection fromPointCollection)
		{
			return new System.Windows.Media.PointCollection(fromPointCollection);
		}

		public static Common.PointCollection ToRubezhPointCollection(this System.Windows.Media.PointCollection fromPointCollection)
		{
			return new Common.PointCollection(fromPointCollection);
		}
	}
}
