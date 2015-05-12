﻿using System.Linq;
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

			AddDriverToKau(RSR2_RM_1_Helper.Create());
			AddDriverToKau(RSR2_AM_1_Helper.Create());
			AddDriverToKau(RSR2_HandDetector_Helper.Create());
			AddDriverToKau(RSR2_SmokeDetector_Helper.Create());
			AddDriverToKau(RSR2_SmokeDetector2_Helper.Create());
			AddDriverToKau(RSR2_RM_2_Helper.Create());
			AddDriverToKau(RSR2_RM_4_Helper.Create());
			AddDriverToKau(RSR2_AM_4_Group_Helper.Create());
			AddDriverToKau(RSR2_AM_2_Group_Helper.Create());
			AddDriverToKau(RSR2_MDU_Helper.Create());
			AddDriverToKau(RSR2_MDU24_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MAP4_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MVK8_Helper.Create());
			AddDriverToKau(RSR2_MAP4_Group_Helper.Create());
			AddDriverToKau(RSR2_MVK8_Group_Helper.Create());
			AddDriverToKau(RSR2_HeatDetector_Helper.Create());
			AddDriverToKau(RSR2_CombinedDetector_Helper.Create());
			AddDriverToKau(RSR2_DrenazhPump_Helper.Create());
			AddDriverToKau(RSR2_JokeyPump_Helper.Create());
			AddDriverToKau(RSR2_FirePump_Helper.Create());
			AddDriverToKau(RSR2_Shuv_Helper.Create());
			AddDriverToKau(RSR2_OPK_Helper.Create());
			AddDriverToKau(RSR2_OPS_Helper.Create());
			AddDriverToKau(RSR2_OPZ_Helper.Create());
			AddDriverToKau(RSR2_MVP_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MVP_Part_Helper.Create());

			AddDriverToKau(RSR2_CodeReader_Helper.Create());
			AddDriverToKau(RSR2_CardReader_Helper.Create());
			AddDriverToKau(RSR2_GuardDetector_Helper.Create());
			AddDriverToKau(RSR2_Valve_KV_Helper.Create());
			AddDriverToKau(RSR2_Valve_KVMV_Helper.Create());
			AddDriverToKau(RSR2_Valve_DU_Helper.Create());
			AddDriverToKau(RSR2_GuardDetectorGuardSound_Helper.Create());
			AddDriverToKau(RSR2_Buz_KV_Helper.Create());
			AddDriverToKau(RSR2_Buz_KVMV_Helper.Create());
			AddDriverToKau(RSR2_Buz_KVDU_Helper.Create());
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