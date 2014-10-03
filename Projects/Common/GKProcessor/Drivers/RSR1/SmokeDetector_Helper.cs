using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class SmokeDetectorHelper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x61,
				DriverType = GKDriverType.SmokeDetector,
				UID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"),
				Name = "Пожарный дымовой извещатель ИП 212-64",
				ShortName = "ИП-64",
				HasZone = true,
				IsPlaceable = true,
				IsIgnored = true,
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму, 0.01*дБ/м", 18, 5, 20);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x82, Name = "Задымленность, 0.001*дБ/м", InternalName = "Smokiness" });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x91, Name = "Запыленность, 0.001*дБ/м", InternalName = "Dustinness" });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x93, Name = "Дата последнего обслуживания", InternalName = "LastServiceTime" });

			return driver;
		}
	}
}