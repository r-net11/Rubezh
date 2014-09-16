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
				DriverTypeNo = 0x10,
				DriverType = XDriverType.RSR2_GuardDetector,
				UID = new Guid("501AA41F-248B-4A4E-982A-6BC93505C7A9"),
				Name = "Извещатель охранный инфракрасный RSR2",
				ShortName = "ИО-ИК RSR2",
				IsControlDevice = true,
				HasZone = false,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Уровень в ед АЦП, с", 128, 128, 384);

			return driver;
		}
	}
}