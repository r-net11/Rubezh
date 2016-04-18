using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class RSR2_SmokeDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xDD,
				DriverType = GKDriverType.RSR2_SmokeDetector,
				UID = new Guid("A50FFA41-B53E-4B3B-ADDF-CDBBA631FEB2"),
				Name = "Извещатель пожарный дымовой оптико-электронный адресно-аналоговый",
				ShortName = "ИП 212-149",
				HasZone = true,
				IsPlaceable = true,
				TypeOfBranche = GKDriver.TypesOfBranches.FireDetector
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0, "Порог срабатывания по дыму, дБ/м", 180, 50, 255).Multiplier = 1000;
			GKDriversHelper.AddIntProprety(driver, 1, "Порог запыленности, дБ/м", 200, 50, 255).Multiplier = 1000;

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Задымленность, дБ/м", InternalName = "Smokiness", Multiplier = 1000 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 2, Name = "Запыленность, дБ/м", InternalName = "Dustinness", Multiplier = 1000 });

			return driver;
		}
	}
}