using System;
using XFiresecAPI;

namespace GKProcessor
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
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по дыму, 0.001*дБ/м", 800, 50, 1000);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 1, Name = "Дым 1, 0.001*дБ/м" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 2, Name = "Дым 2, 0.001*дБ/м" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 3, Name = "Пыль 1, 0.001*дБ/м" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 4, Name = "Пыль 2, 0.001*дБ/м" });

			return driver;
		}
	}
}