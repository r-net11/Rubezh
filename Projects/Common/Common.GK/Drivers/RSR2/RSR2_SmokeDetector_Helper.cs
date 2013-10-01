using System;
using XFiresecAPI;

namespace Common.GK
{
	public class RSR2_SmokeDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xDD,
				DriverType = XDriverType.RSR2_SmokeDetector,
				UID = new Guid("A50FFA41-B53E-4B3B-ADDF-CDBBA631FEB2"),
				Name = "Пожарный дымовой извещатель ИПД RSR2",
				ShortName = "ИПД RSR2",
				HasZone = true,
                IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по дыму, 0.001*дБ/м", 0, 180, 50, 200);
			GKDriversHelper.AddIntProprety(driver, 1, "Порог запыленности, 0.001*дБ/м", 0, 200, 0, 500);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Задымленность, 0.001*дБ/м", InternalName = "Smokiness" });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Запыленность, 0.001*дБ/м", InternalName = "Dustinness" });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Дата последнего обслуживания", InternalName = "LastServiceTime" });

			return driver;
		}
	}
}