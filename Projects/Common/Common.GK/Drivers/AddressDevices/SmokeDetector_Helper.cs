using System;
using XFiresecAPI;

namespace Common.GK
{
	public class SmokeDetectorHelper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x61,
				DriverType = XDriverType.SmokeDetector,
				UID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"),
				Name = "Пожарный дымовой извещатель ИП 212-64",
				ShortName = "ИП-64",
				HasZone = true,
                CanPlaceOnPlan = true
			};
			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Info);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму", 0, 18, 5, 20);

			driver.AUParameters.Add(new XAUParameter() { No = 0x82, Name = "Дым" });

			return driver;
		}
	}
}