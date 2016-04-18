using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_HandDetectorEridan_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x16,
				DriverType = GKDriverType.RSR2_HandDetectorEridan,
				UID = new Guid("528DA6A8-F4DA-4FF6-93B8-2E355F33AFE4"),
				Name = "Извещатель пожарный ручной Эридан",
				ShortName = "ИПРЭ",
				HasZone = true,
				IsPlaceable = true,
				TypeOfBranche = GKDriver.TypesOfBranches.FireDetector
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			return driver;
		}
	}
}