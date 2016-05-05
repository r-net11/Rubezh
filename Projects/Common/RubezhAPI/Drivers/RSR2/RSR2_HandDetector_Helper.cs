using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_HandDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xD8,
				DriverType = GKDriverType.RSR2_HandDetector,
				UID = new Guid("151881A2-0A39-4609-870F-4A84B2F8A4C8"),
				Name = "Извещатель пожарный ручной электроконтактный адресный",
				ShortName = "ИПР 513-12",
				HasZone = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.FireDetector
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			return driver;
		}
	}
}