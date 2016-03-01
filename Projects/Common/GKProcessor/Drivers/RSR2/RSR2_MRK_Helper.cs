using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RSR2_MRK_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x15,
				DriverType = GKDriverType.RSR2_MRK,
				UID = new Guid("1EF2C05C-B86A-44E5-8F4C-662AF92DAF20"),
				Name = "Модуль радиоканальный",
				ShortName = "МРК-R2",
				IsPlaceable = true
			};

			driver.Children.Add(GKDriverType.RK_HandDetector);
			driver.Children.Add(GKDriverType.RK_SmokeDetector);
			driver.Children.Add(GKDriverType.RK_HeatDetector);
			driver.Children.Add(GKDriverType.RK_RM);
			driver.Children.Add(GKDriverType.RK_AM);
			driver.Children.Add(GKDriverType.RK_OPK);
			driver.Children.Add(GKDriverType.RK_OPZ);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			GKDriversHelper.AddIntProprety(driver, 0, "Число радиоканальных устройств", 0, 0, 32);
			GKDriversHelper.AddIntProprety(driver, 1, "Период опроса, с", 4, 4, 120);
			GKDriversHelper.AddIntProprety(driver, 2, "Повторы/неответы в количестве периодов", 3, 3, 32);
			return driver;
		}
	}
}
