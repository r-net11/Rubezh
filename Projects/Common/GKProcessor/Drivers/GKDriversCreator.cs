using System.Linq;
using RubezhAPI.GK;
using RubezhAPI;

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
			GKManager.DriversConfiguration.Drivers.Add(GKMirrorDetectorsDevice_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKMirrorPerformersDevice.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKMirrorFightFireZone_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKMirrorFireZone.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKMirrorGuardZone_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKIndicatorsGroup_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKMirrorDirection_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(GKRelaysGroup_Helper.Create());

			AddDriverToKau(RSR2_RM_1_Helper.Create());
			AddDriverToKau(RSR2_AM_1_Helper.Create());
			AddDriverToKau(RSR2_HandDetector_Helper.Create());
			AddDriverToKau(RSR2_SmokeDetector_Helper.Create());
			AddDriverToKau(RSR2_RM_2_Helper.Create());
			AddDriverToKau(RSR2_RM_4_Helper.Create());
			AddDriverToKau(RSR2_AM_4_Group_Helper.Create());
			AddDriverToKau(RSR2_AM_2_Group_Helper.Create());
			AddDriverToKau(RSR2_MDU_Helper.Create());
			AddDriverToKau(RSR2_MDU24_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_MAP4_Helper.Create());
			AddDriverToKau(RSR2_MVK8_Helper.Create());
			AddDriverToKau(RSR2_MAP4_Group_Helper.Create());
			AddDriverToKau(RSR2_MVK8_Group_Helper.Create());
			AddDriverToKau(RSR2_MVK4_Group_Helper.Create());
			AddDriverToKau(RSR2_MVK2_Group_Helper.Create());
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
			AddDriverToKau(RSR2_OPSZ_Group_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_OPKS_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_OPKZ_Helper.Create());
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
			AddDriverToKau(RSR2_ABPC_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_ABShS_Helper.Create());
			AddDriverToKau(RSR2_ABShS_Group_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RSR2_ABTK_Helper.Create());
			AddDriverToKau(RSR2_ABTK_Group_Helper.Create());
			AddDriverToKau(RSR2_HeatDetectorEridan_Helper.Create());
			AddDriverToKau(RSR2_HandDetectorEridan_Helper.Create());
			AddDriverToKau(RSR2_IOLIT_Helper.Create());
			AddDriverToKau(RSR2_MRK_Helper.Create());
			AddDriverToKau(RSR2_Button_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_HandDetector_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_SmokeDetector_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_HeatDetector_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_RM_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_AM_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_OPK_Helper.Create());
			GKManager.DriversConfiguration.Drivers.Add(RK_OPZ_Helper.Create());
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