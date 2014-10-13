using System;
using FiresecAPI.GK;

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

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по температуре, C", 70, 0, 100);
			GKDriversHelper.AddIntProprety(driver, 1, "Порог срабатывания по градиенту температуры, C/мин", 5, 1, 50);
			GKDriversHelper.AddIntProprety(driver, 2, "Порог срабатывания по дыму, 0.001*дБ/м", 180, 50, 200);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог запыленности, 0.001*дБ/м", 200, 0, 500);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Температура, C", InternalName = "Temperature" });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 2, Name = "Градиент температуры, C/мин", InternalName = "TemperatureDelta" });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 3, Name = "Задымленность, дБ/м", InternalName = "Smokiness", Multiplier = 1000 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 4, Name = "Запыленность, дБ/м", InternalName = "Dustinness", Multiplier = 1000 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 5, Name = "Дата последнего обслуживания", InternalName = "LastServiceTime" });

			return driver;
		}
	}
}