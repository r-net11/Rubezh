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
			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Info);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по дыму, 0.001*дБ/м", 0, 180, 50, 200);
			GKDriversHelper.AddIntProprety(driver, 1, "Порог запыленности, 0.001*дБ/м", 0, 200, 0, 500);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Дым, 0.001*дБ/м" });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Пыль, 0.001*дБ/м" });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Обслуживание, м.г." });

			return driver;
		}
	}
}