using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public static class GKDriversCreator
	{
		public static void Create()
		{
			GKManager.DriversConfiguration = new GKDriversConfiguration();

			GKManager.DriversConfiguration.Drivers.Add(GKSystem_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GK_Helper.Create());
		}
	}
}