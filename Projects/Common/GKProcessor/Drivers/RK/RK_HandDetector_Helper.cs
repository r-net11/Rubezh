using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RK_HandDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x18,
				DriverType = GKDriverType.RK_HandDetector,
				UID = new Guid("88652A0E-BA4C-4CED-A130-761BED8872A0"),
				Name = "Извещатель пожарный ручной радиоканальный",
				ShortName = "ИПР-RK",
				HasZone = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddRadioChanelProperties(driver);

			return driver;
		}
	}
}