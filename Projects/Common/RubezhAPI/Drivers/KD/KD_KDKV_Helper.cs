using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class KD_KDKV_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF6,
				DriverType = GKDriverType.KD_KDKV,
				UID = new Guid("AE315D83-ECF5-4E75-801B-CB3F6EDE8FB8"),
				Name = "КВ КД",
				ShortName = "КДКВ",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				IsAutoCreate = true,
				MinAddress = 4,
				MaxAddress = 5,
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			return driver;
		}
	}
}
