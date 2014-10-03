using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class HandDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x55,
				DriverType = GKDriverType.HandDetector,
				UID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a"),
				Name = "Ручной извещатель ИПР513-11",
				ShortName = "ИПР",
				HasZone = true,
				IsPlaceable = true,
				IsIgnored = true,
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			return driver;
		}
	}
}