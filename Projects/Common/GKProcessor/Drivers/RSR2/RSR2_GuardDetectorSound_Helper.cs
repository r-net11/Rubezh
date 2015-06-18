﻿using System;
using FiresecAPI.GK;

namespace GKProcessor
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
				Name = "Извещатель охранный поверхностнозвуковой R2",
				ShortName = "ИО-ПЗ R2",
				IsControlDevice = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Уровень НЧ в ед АЦП, с", 16, 16, 384);
			GKDriversHelper.AddIntProprety(driver, 1, "Уровень ВЧ в ед АЦП, с", 16, 16, 384);

			return driver;
		}
	}
}