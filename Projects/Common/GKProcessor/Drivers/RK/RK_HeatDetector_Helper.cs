using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RK_HeatDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x1E,
				DriverType = GKDriverType.RK_HeatDetector,
				UID = new Guid("5D8010FF-B6B8-4A80-9191-211D008BC86B"),
				Name = "Извещатель пожарный тепловой радиоканальный",
				ShortName = "ИПТ-RK",
				HasZone = true,
				IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			GKDriversHelper.AddRadioChanelProperties(driver);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог срабатывания по температуре, °C", 70, 54, 85);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог срабатывания по градиенту температуры, °C/мин", 5, 1, 50).Multiplier = 10;

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Температура, °C", InternalName = "Temperature", HasNegativeValue = true });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 2, Name = "Градиент температуры, °C/мин", InternalName = "TemperatureDelta", HasNegativeValue = true });

			return driver;
		}
	}
}