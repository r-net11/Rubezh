using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_HandGuardDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xEF,
				DriverType = GKDriverType.RSR2_HandGuardDetector,
				UID = new Guid("BD3A0139-D9FD-4224-B5D6-8772BA9E3760"),
				Name = "Извещатель охранный вскрытия",
				ShortName = "ИОВ",
				HasGuardZone = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.IntruderDetector
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			return driver;
		}
	}
}