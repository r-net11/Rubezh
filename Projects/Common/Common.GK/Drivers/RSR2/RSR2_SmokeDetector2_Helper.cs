using System;
using XFiresecAPI;

namespace Common.GK
{
	public class RSR2_SmokeDetector2_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xD9,
				DriverType = XDriverType.RSR2_SmokeDetector2,
				UID = new Guid("236F4856-5146-4B45-9047-03EAC91652E2"),
				Name = "Пожарный дымовой извещатель ИП2Д RSR2",
				ShortName = "ИП2Д RSR2",
				HasZone = true,
                IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Info);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по дыму, 0.001*дБ/м", 0, 800, 50, 1000);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Дым 1" });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Дым 2" });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Пыль 1" });
			driver.AUParameters.Add(new XAUParameter() { No = 4, Name = "Пыль 2" });

			return driver;
		}
	}
}