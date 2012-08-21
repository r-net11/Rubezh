using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public static class GKDriversCreator
	{
		public static void Create()
		{
			System.Diagnostics.Trace.WriteLine("RM_2 " + FiresecManager.Drivers.FirstOrDefault(x=>x.DriverType == FiresecAPI.Models.DriverType.RM_2).UID.ToString());
			System.Diagnostics.Trace.WriteLine("RM_3 " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.RM_3).UID.ToString());
			System.Diagnostics.Trace.WriteLine("RM_4 " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.RM_4).UID.ToString());
			System.Diagnostics.Trace.WriteLine("RM_5 " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.RM_5).UID.ToString());
			System.Diagnostics.Trace.WriteLine("AM4 " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.AM4).UID.ToString());
			System.Diagnostics.Trace.WriteLine("AM4_P " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.AM4_P).UID.ToString());
			System.Diagnostics.Trace.WriteLine("AM1_O " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.AM1_O).UID.ToString());
			System.Diagnostics.Trace.WriteLine("AM1_T " + FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == FiresecAPI.Models.DriverType.AM1_T).UID.ToString());

			XManager.DriversConfiguration = new XDriversConfiguration();

			XManager.DriversConfiguration.Drivers.Add(GKSystem_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(GK_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKIndicator_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKLine_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKRele_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKRele_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(KAU_Helper.Create());
			XManager.DriversConfiguration.Drivers.Add(KAUIndicator_Helper.Create());

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
			XManager.DriversConfiguration.Drivers.Add(driver);
			var kauDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
			kauDriver.Children.Add(driver.DriverType);
		}
	}
}