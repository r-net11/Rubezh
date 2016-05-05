using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_GuardDetectorGuardSound_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x11,
				DriverType = GKDriverType.RSR2_GuardDetectorSound,
				UID = new Guid("38618571-FA63-4a8f-9D39-F16E36567E32"),
				Name = "Извещатель охранный поверхностнозвуковой",
				ShortName = "ИО-ПЗ",
				HasGuardZone = true,
				HasZone = false,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.IntruderDetector
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			driver.AvailableStateBits.Add(GKStateBit.Off);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Уровень НЧ в ед АЦП, с", 16, 16, 384);
			GKDriversHelper.AddIntProprety(driver, 1, "Уровень ВЧ в ед АЦП, с", 16, 16, 384);

			return driver;
		}
	}
}