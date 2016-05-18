using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAPI
{
	public static class DriversRepository
	{
		public static List<GKDriver> Drivers { get; private set; }

		static DriversRepository()
		{
			Drivers = new List<GKDriver>();

			Drivers.Add(GKSystem_Helper.Create());
			Drivers.Add(GK_Helper.Create());
			Drivers.Add(GKIndicator_Helper.Create());
			Drivers.Add(GKRele_Helper.Create());
			Drivers.Add(KAU_RSR2_Helper.Create());
			Drivers.Add(KAUIndicator_Helper.Create());
			Drivers.Add(RSR2_KAU_Shleif_Helper.Create());
			Drivers.Add(KAU_Mirror_Helper.Create());
			Drivers.Add(GKMirrorDetectorsDevice_Helper.Create());
			Drivers.Add(GKMirrorPerformersDevice.Create());
			Drivers.Add(GKMirrorFightFireZone_Helper.Create());
			Drivers.Add(GKMirrorFireZone.Create());
			Drivers.Add(GKMirrorGuardZone_Helper.Create());
			Drivers.Add(GKIndicatorsGroup_Helper.Create());
			Drivers.Add(GKMirrorDirection_Helper.Create());
			Drivers.Add(GKRelaysGroup_Helper.Create());

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
			Drivers.Add(RSR2_MAP4_Helper.Create());
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
		    Drivers.Add(RSR2_OPKS_Helper.Create());
			Drivers.Add(RSR2_OPKZ_Helper.Create());
			Drivers.Add(RSR2_MVP_Part_Helper.Create());

			AddDriverToKau(RSR2_CodeReader_Helper.Create());
			AddDriverToKau(RSR2_CardReader_Helper.Create());
			AddDriverToKau(RSR2_CodeCardReader_Helper.Create());
			AddDriverToKau(RSR2_GuardDetector_Helper.Create());
			AddDriverToKau(RSR2_Valve_KV_Helper.Create());
			AddDriverToKau(RSR2_Valve_KVMV_Helper.Create());
			AddDriverToKau(RSR2_Valve_DU_Helper.Create());
			AddDriverToKau(RSR2_GuardDetectorGuardSound_Helper.Create());
			AddDriverToKau(RSR2_Buz_KV_Helper.Create());
			AddDriverToKau(RSR2_Buz_KVMV_Helper.Create());
			AddDriverToKau(RSR2_Buz_KVDU_Helper.Create());
			AddDriverToKau(RSR2_ABPC_Helper.Create());
		    Drivers.Add(RSR2_ABShS_Helper.Create());
			AddDriverToKau(RSR2_ABShS_Group_Helper.Create());
			Drivers.Add(RSR2_ABTK_Helper.Create());
			AddDriverToKau(RSR2_ABTK_Group_Helper.Create());
			AddDriverToKau(RSR2_HeatDetectorEridan_Helper.Create());
			AddDriverToKau(RSR2_HandDetectorEridan_Helper.Create());
			AddDriverToKau(RSR2_IOLIT_Helper.Create());
			AddDriverToKau(RSR2_IS_Helper.Create());
			AddDriverToKau(RSR2_MRK_Helper.Create());
			AddDriverToKau(RSR2_Button_Helper.Create());
			AddDriverToKau(RSR2_ZOV_Helper.Create());
			AddDriverToKau(RSR2_SCOPA_Helper.Create());
			AddDriverToKau(RSR2_HandGuardDetector_Helper.Create());
			AddDriverToKau(RSR2_KDKR_Helper.Create());
			Drivers.Add(RSR2_KDKR_Part_Helper.Create());
			Drivers.Add(KD_KDZ_Helper.Create());
			Drivers.Add(KD_KDK_Helper.Create());
			Drivers.Add(KD_KDKV_Helper.Create());
			Drivers.Add(KD_KDTD_Helper.Create());

			Drivers.Add(RK_HandDetector_Helper.Create());
			Drivers.Add(RK_SmokeDetector_Helper.Create());
			Drivers.Add(RK_HeatDetector_Helper.Create());
			Drivers.Add(RK_RM_Helper.Create());
			Drivers.Add(RK_AM_Helper.Create());
			Drivers.Add(RK_OPK_Helper.Create());
			Drivers.Add(RK_OPZ_Helper.Create());
			var kauShleifDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			var mvpPartDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_MVP_Part);
			var kdkrPartDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KDKR_Part);
			foreach (var driver in kauShleifDriver.Children)
			{
				mvpPartDriver.Children.Add(driver);
				kdkrPartDriver.Children.Add(driver);
			}
		}

		static void AddDriverToKau(GKDriver driver)
		{
			Drivers.Add(driver);
			var kauShleifDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			kauShleifDriver.Children.Add(driver.DriverType);
		}
	}
}
