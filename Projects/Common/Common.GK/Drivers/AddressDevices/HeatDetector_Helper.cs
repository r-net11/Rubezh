using System;
using XFiresecAPI;

namespace Common.GK
{
	public class HeatDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x62,
				DriverType = XDriverType.HeatDetector,
				UID = new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c"),
				Name = "Пожарный тепловой извещатель ИП 101-29-A3R1",
				ShortName = "ИП-29",
				HasZone = true,
                CanPlaceOnPlan = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Info);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре, C", 0, 70, 54, 85);
			//GKDriversHelper.AddIntProprety(driver, 0x8C, "Порог срабатывания по градиенту температуры", 0, 100, 0, 255);

			driver.AUParameters.Add(new XAUParameter() { No = 0x83, Name = "Температура" });

			return driver;
		}
	}
}