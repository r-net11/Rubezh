using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public static class GKDriversCreator
	{
		public static void Create()
		{
			XManager.DriversConfiguration = new XDriversConfiguration();

			XManager.DriversConfiguration.XDrivers.Add(GKSystem_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(GK_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(GKIndicator_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(GKLine_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(GKRele_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(PumpStation_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(KAU_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(KAU_RSR2_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(KAUIndicator_Helper.Create());

			AddDriverToKau(SmokeDetectorHelper.Create());
			AddDriverToKau(HeatDetector_Helper.Create());
			AddDriverToKau(CombinedDetector_Helper.Create());
			AddDriverToKau(HandDetector_Helper.Create());

			AddDriverToKau(AM_1_Helper.Create());
			AddDriverToKau(AMP_1_Helper.Create());
			AddDriverToKau(AM1_T_Helper.Create());
			AddDriverToKau(AM1_O_Helper.Create());

			AddDriverToKau(RM_1_Helper.Create());
			AddDriverToKau(MRO_Helper.Create());
			AddDriverToKau(MRO_2_Helper.Create());
			AddDriverToKau(MDU_Helper.Create());
			AddDriverToKau(MPT_Helper.Create());
			AddDriverToKau(BUZ_Helper.Create());
			AddDriverToKau(BUN_Helper.Create());

			AddDriverToKau(RM_2_Helper.Create());
			AddDriverToKau(RM_3_Helper.Create());
			AddDriverToKau(RM_4_Helper.Create());
			AddDriverToKau(RM_5_Helper.Create());
			AddDriverToKau(AM_4_Helper.Create());
			AddDriverToKau(AMP_4_Helper.Create());

			XManager.DriversConfiguration.XDrivers.Add(RSR2_RM_1_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(RSR2_AM_1_Helper.Create());
			AddDriverToKau_RSR2(RSR2_HandDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_SmokeDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_SmokeDetector2_Helper.Create());
			AddDriverToKau_RSR2(RSR2_RM_2_Helper.Create());
			AddDriverToKau_RSR2(RSR2_AM_4_Group_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MDU_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(RSR2_MAP4_Helper.Create());
			XManager.DriversConfiguration.XDrivers.Add(RSR2_MVK8_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MAP4_Group_Helper.Create());
			AddDriverToKau_RSR2(RSR2_MVK8_Group_Helper.Create());
			AddDriverToKau_RSR2(RSR2_HeatDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_CombinedDetector_Helper.Create());
			AddDriverToKau_RSR2(RSR2_Pump_Helper.Create());
		}

		static void AddDriverToKau(XDriver driver)
		{
			XManager.DriversConfiguration.XDrivers.Add(driver);
			var kauDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
			kauDriver.Children.Add(driver.DriverType);
		}

		static void AddDriverToKau_RSR2(XDriver driver)
		{
			XManager.DriversConfiguration.XDrivers.Add(driver);
			var kauDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU);
			kauDriver.Children.Add(driver.DriverType);
		}
	}
}