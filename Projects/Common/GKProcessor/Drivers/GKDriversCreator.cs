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
			GKManager.DriversConfiguration.Drivers.Add(GKIndicator_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKRele_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAU_RSR2_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAUIndicator_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_KAU_Shleif_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAU_Mirror_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAU_MirrorItem_Helper.Create());

			var kauShleifDriver = GKManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			var mvpPartDriver = GKManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_MVP_Part);
			foreach (var driver in kauShleifDriver.Children)
			{
				mvpPartDriver.Children.Add(driver);
			}
		}

		static void AddDriverToKau(GKDriver driver)
		{
			GKManager.DriversConfiguration.Drivers.Add(driver);
			var kauShleifDriver = GKManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			kauShleifDriver.Children.Add(driver.DriverType);
		}
	}
}