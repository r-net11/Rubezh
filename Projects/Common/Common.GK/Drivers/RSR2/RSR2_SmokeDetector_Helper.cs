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
				DriverTypeNo = 0xD9,
				DriverType = XDriverType.RSR2_SmokeDetector,
				UID = new Guid("A50FFA41-B53E-4B3B-ADDF-CDBBA631FEB2"),
				Name = "Пожарный дымовой извещатель ИП 212-64 RSR2",
				ShortName = "ИП-64 RSR2",
				HasZone = true,
                IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Info);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по дыму, 0.01*дБ/м", 0, 18, 5, 20);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Дым 1" });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Дым 2" });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Пыль 1" });
			driver.AUParameters.Add(new XAUParameter() { No = 4, Name = "Пыль 2" });

			return driver;
		}
	}
}