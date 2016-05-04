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
				ShortName = "МРК",
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.RadioChannel
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

			var property3 = new GKDriverProperty()
			{
				No = 3,
				Name = "№ Канала",
				Caption = "№ Канала",
				Default = 0,
				Min = 0,
				Max = 15,
				IsLowByte = true,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType
			};
			driver.Properties.Add(property3);

			var property4 = new GKDriverProperty()
			{
				No = 3,
				Name = "Режим выбора канала",
				Caption = "Режим выбора канала",
				Default = 0,
				IsHieghByte = true,
			};
			GKDriversHelper.AddPropertyParameter(property4, "Ручной", 0);
			GKDriversHelper.AddPropertyParameter(property4, "Автоматический", 1);
			driver.Properties.Add(property4);

			GKDriversHelper.AddIntProprety(driver, 4, "Порог питания, В", 120, 0, 280).Multiplier = 10;
			return driver;
		}
	}
}
