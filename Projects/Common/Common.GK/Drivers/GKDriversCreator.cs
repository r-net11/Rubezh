using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public static class GKDriversCreator
	{
		public static void Create()
		{
			XManager.XDriversConfiguration = new XDriversConfiguration();

			XManager.XDriversConfiguration.XDrivers.Add(GKSystem_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(GK_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(GKIndicator_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(GKLine_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(GKRele_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(GKRele_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(KAU_Helper.Create());
			XManager.XDriversConfiguration.XDrivers.Add(KAUIndicator_Helper.Create());

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
		}

		static void AddDriverToKau(XDriver driver)
		{
			XManager.XDriversConfiguration.XDrivers.Add(driver);
			var kauDriver = XManager.XDriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
			kauDriver.Children.Add(driver.DriverType);
		}
	}
}