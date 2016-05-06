using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class KD_KDZ_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF4,
				DriverType = GKDriverType.KD_KDZ,
				UID = new Guid("2E9F2B6A-D707-4007-842C-664D6BCBE9F1"),
				Name = "Замок КД",
				ShortName = "КДЗ",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				IsAutoCreate = true,
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			return driver;
		}
	}
}
