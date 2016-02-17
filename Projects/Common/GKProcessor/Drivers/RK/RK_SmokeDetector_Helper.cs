using System;
using RubezhAPI.GK;

namespace GKProcessor
{
    public class RK_SmokeDetector_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x1D,
				DriverType = GKDriverType.RK_SmokeDetector,
				UID = new Guid("2685C3BE-FB9B-4D8E-AA57-C66A135B0D76"),
				Name = "Извещатель пожарный дымовой оптико-электронный радиоканальный",
				ShortName = "ИП-RK",
				HasZone = true,
				IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

            GKDriversHelper.AddRadioChanelProperties(driver);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог срабатывания по дыму, дБ/м", 180, 50, 255).Multiplier = 1000;
			GKDriversHelper.AddIntProprety(driver, 4, "Порог запыленности, дБ/м", 200, 50, 255).Multiplier = 1000;

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Задымленность, дБ/м", InternalName = "Smokiness", Multiplier = 1000 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 2, Name = "Запыленность, дБ/м", InternalName = "Dustinness", Multiplier = 1000 });

			return driver;
		}
	}
}
