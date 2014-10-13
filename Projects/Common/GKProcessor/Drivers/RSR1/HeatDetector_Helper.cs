using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class HeatDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x62,
				DriverType = GKDriverType.HeatDetector,
				UID = new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c"),
				Name = "Пожарный тепловой извещатель ИП 101-29-A3R1",
				ShortName = "ИП-29",
				HasZone = true,
				IsPlaceable = true,
				IsIgnored = true,
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре, C", 70, 54, 85);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x83, Name = "Температура, C", InternalName = "Temperature" });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x93, Name = "Дата последнего обслуживания, м.г.", InternalName = "LastServiceTime" });

			return driver;
		}
	}
}