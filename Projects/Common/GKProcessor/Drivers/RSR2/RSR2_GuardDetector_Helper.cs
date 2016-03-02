﻿using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_GuardDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x10,
				DriverType = GKDriverType.RSR2_GuardDetector,
				UID = new Guid("501AA41F-248B-4A4E-982A-6BC93505C7A9"),
				Name = "Извещатель охранный инфракрасный R2",
				ShortName = "ИО-ИК R2",
				HasZone = false,
				HasGuardZone = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			driver.AvailableStateBits.Add(GKStateBit.Off);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Уровень в ед АЦП, с", 128, 16, 384);

			return driver;
		}
	}
}