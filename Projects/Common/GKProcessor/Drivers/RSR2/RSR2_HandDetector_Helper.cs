using System;
using FiresecAPI.GK;

namespace GKProcessor
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
				Name = "ИЗВЕЩАТЕЛЬ ПОЖАРНЫЙ РУЧНОЙ ЭЛЕКТРОКОНТАКТНЫЙ АДРЕСНЫЙ",
				ShortName = "ИПР 513-12",
				HasZone = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			return driver;
		}
	}
}