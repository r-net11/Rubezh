
namespace Infrastructure.Plans
{
	public static class PointCollectionExtentions
	{
		public static System.Windows.Media.PointCollection ToWindowsPointCollection(this RubezhAPI.PointCollection fromPointCollection)
		{
			return new System.Windows.Media.PointCollection(fromPointCollection);
		}

		public static RubezhAPI.PointCollection ToRubezhPointCollection(this System.Windows.Media.PointCollection fromPointCollection)
		{
			return new RubezhAPI.PointCollection(fromPointCollection);
		}
	}
}
