using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class CombinedDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x60,
				DriverType = XDriverType.CombinedDetector,
				UID = new Guid("37f13667-bc77-4742-829b-1c43fa404c1f"),
				Name = "Пожарный комбинированный извещатель ИП212/101-64-А2R1",
				ShortName = "ИП-64К",
				HasZone = true,
				IsPlaceable = true,
				IsIgnored = true,
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму, 0.01*дБ/м", 18, 5, 20);
			GKDriversHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре, C", 70, 54, 85);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x82, Name = "Задымленность, 0.001*дБ/м", InternalName = "Smokiness" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x83, Name = "Температура, C", InternalName = "Temperature" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x86, Name = "Запыленность, 0.001*дБ/м", InternalName = "Dustinness" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x87, Name = "Порог запыленности предварительный, 0.001*дБ/м" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x8A, Name = "Порог запыленности критический, 0.001*дБ/м" });

			return driver;
		}
	}
}