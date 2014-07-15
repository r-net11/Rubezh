using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_GuardDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xF1,
				DriverType = XDriverType.RSR2_GuardDetector,
				UID = new Guid("501AA41F-248B-4A4E-982A-6BC93505C7A9"),
				Name = "Охранный извещатель RSR2",
				ShortName = "ОИ RSR2",
				HasZone = false,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			return driver;
		}
	}
}