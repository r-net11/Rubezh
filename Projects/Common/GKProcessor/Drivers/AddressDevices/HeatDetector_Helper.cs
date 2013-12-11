using System;
using XFiresecAPI;

namespace GKProcessor
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
                IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре, C", 70, 54, 85);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x83, Name = "Температура, C", InternalName = "Temperature" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x93, Name = "Дата последнего обслуживания", InternalName = "LastServiceTime" });

			return driver;
		}
	}
}