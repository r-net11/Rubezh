using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RSR2_HeatDetectorEridan_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x17,
				DriverType = GKDriverType.RSR2_HeatDetectorEridan,
				UID = new Guid("ADC56B8F-0B5D-40E3-933A-9E97F3C8649F"),
				Name = "Извещатель пожарный тепловой Эридан",
				ShortName = "ИПТЭ",
				HasZone = true,
				IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			return driver;
		}
	}
}