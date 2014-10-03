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
			GKManager.DriversConfiguration.Drivers.Add(KAU_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAU_RSR2_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAUIndicator_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(KAU_Shleif_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_KAU_Shleif_Helper.Create());

			AddDriverToKau(SmokeDetectorHelper.Create());
			AddDriverToKau(HeatDetector_Helper.Create());
			AddDriverToKau(CombinedDetector_Helper.Create());
			AddDriverToKau(HandDetector_Helper.Create());

			AddDriverToKau(AM_1_Helper.Create());
			AddDriverToKau(AMP_1_Helper.Create());
			AddDriverToKau(AM1_T_Helper.Create());
			AddDriverToKau(Battery_Helper.Create());

			AddDriverToKau(RM_1_Helper.Create());
			AddDriverToKau(MRO_2_Helper.Create());
			AddDriverToKau(MDU_Helper.Create());
			AddDriverToKau(MPT_Helper.Create());
			AddDriverToKau(Valve_Helper.Create());

			AddDriverToKau(FirePump_Helper.Create());
			AddDriverToKau(JockeyPump_Helper.Create());
			AddDriverToKau(DrainagePump_Helper.Create());

			AddDriverToKau(Shu_Helper.Create());
			AddDriverToKau(Shuv_Helper.Create());

			AddDriverToKau(RM_2_Helper.Create());
			AddDriverToKau(RM_3_Helper.Create());
			AddDriverToKau(RM_4_Helper.Create());
			AddDriverToKau(RM_5_Helper.Create());
			AddDriverToKau(AM_4_Helper.Create());
			AddDriverToKau(AMP_4_Helper.Create());

			GKManager.DriversConfiguration.Drivers.Add(RSR2_RM_1_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_AM_1_Helper.Create());
			AddDriverToKau_RSR2(RSR2_HandDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_SmokeDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_SmokeDetector2_Helper.Create());
			AddDriverToKau_RSR2(RSR2_RM_2_Helper.Create());
			AddDriverToKau_RSR2(RSR2_AM_4_Group_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MDU_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MAP4_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MVK8_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MAP4_Group_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MVK8_Group_Helper.Create());
			AddDriverToKau_RSR2(RSR2_HeatDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_CombinedDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_DrenazhPump_Helper.Create());
			AddDriverToKau_RSR2(RSR2_JokeyPump_Helper.Create());
			AddDriverToKau_RSR2(RSR2_FirePump_Helper.Create());
			AddDriverToKau_RSR2(RSR2_OPK_Helper.Create());
			AddDriverToKau_RSR2(RSR2_OPS_Helper.Create());
			AddDriverToKau_RSR2(RSR2_OPZ_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MVP_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MVP_Part_Helper.Create());

			AddDriverToKau_RSR2(RSR2_CodeReader_Helper.Create());
			AddDriverToKau_RSR2(RSR2_GuardDetector_Helper.Create());

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
			var kauShleifDriver = GKManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.KAU_Shleif);
			kauShleifDriver.Children.Add(driver.DriverType);
		}

		static void AddDriverToKau_RSR2(GKDriver driver)
		{
			GKManager.DriversConfiguration.Drivers.Add(driver);
			var kauShleifDriver = GKManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			kauShleifDriver.Children.Add(driver.DriverType);
		}
	}
}