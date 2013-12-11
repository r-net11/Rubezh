using System;
using XFiresecAPI;

namespace GKProcessor
{
	public class RSR2_HeatDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xDE,
				DriverType = XDriverType.RSR2_HeatDetector,
				UID = new Guid("C0A93D79-9A7F-46AF-A190-855F32759A05"),
				Name = "Пожарный тепловой извещатель ИПТ RSR2",
				ShortName = "ИПТ RSR2",
				HasZone = true,
                IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по температуре, C", 70, 0, 100);
			GKDriversHelper.AddIntProprety(driver, 1, "Порог срабатывания по градиенту температуры, C/мин", 5, 1, 50);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 1, Name = "Температура, C", InternalName = "Temperature" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 2, Name = "Градиент температуры, C/мин", InternalName = "TemperatureDelta" });

			return driver;
		}
	}
}