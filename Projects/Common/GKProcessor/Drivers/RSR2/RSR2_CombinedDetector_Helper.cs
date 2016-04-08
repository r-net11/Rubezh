using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RSR2_CombinedDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xDF,
				DriverType = GKDriverType.RSR2_CombinedDetector,
				UID = new Guid("1CCE48EB-B60B-4E06-8290-A39591CD3DA2"),
				Name = "Извещатель пожарный комбинированный дымовой оптико-электронный тепловой максимально-дифференциальный",
				ShortName = "ИП 212/101-11-PR",
				HasZone = true,
				IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по температуре, °C", 70, 54, 85);
			GKDriversHelper.AddIntProprety(driver, 1, "Порог срабатывания по градиенту температуры, °C/мин", 50, 10, 500).Multiplier = 10;
			GKDriversHelper.AddIntProprety(driver, 2, "Порог срабатывания по дыму, дБ/м", 180, 50, 255).Multiplier = 1000;
			GKDriversHelper.AddIntProprety(driver, 3, "Порог запыленности, дБ/м", 200, 50, 255).Multiplier = 1000;

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 1, Name = "Температура, °C", InternalName = "Temperature", HasNegativeValue = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Градиент температуры, °C/мин", InternalName = "TemperatureDelta", HasNegativeValue = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 3, Name = "Задымленность, дБ/м", InternalName = "Smokiness", Multiplier = 1000 });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 4, Name = "Запыленность, дБ/м", InternalName = "Dustinness", Multiplier = 1000 });

			return driver;
		}
	}
}