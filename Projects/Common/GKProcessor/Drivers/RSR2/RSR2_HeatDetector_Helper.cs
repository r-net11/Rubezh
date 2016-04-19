using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RSR2_HeatDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xDE,
				DriverType = GKDriverType.RSR2_HeatDetector,
				UID = new Guid("C0A93D79-9A7F-46AF-A190-855F32759A05"),
				Name = "Извещатель пожарный тепловой",
				ShortName = "ИП 101-52-PR",
				HasZone = true,
				IsPlaceable = true,
				TypeOfBranche = GKDriver.TypesOfBranches.FireDetector
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по температуре, °C", 70, 54, 85);
			GKDriversHelper.AddIntProprety(driver, 1, "Порог срабатывания по градиенту температуры, °C/мин", 50, 10, 500).Multiplier = 10;

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Температура, °C", InternalName = "Temperature", HasNegativeValue = true });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 2, Name = "Градиент температуры, °C/мин", InternalName = "TemperatureDelta", HasNegativeValue = true });

			return driver;
		}
	}
}