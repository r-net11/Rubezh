using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_MAP4_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE1,
				DriverType = XDriverType.RSR2_MAP4,
				UID = new Guid("42B3C448-2FDD-43D4-AEE0-F173CB8D6CF8"),
				Name = "МАП RSR2",
				ShortName = "МАП RSR2",
				HasZone = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Failure);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire2);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			var property1 = new XDriverProperty()
			{
				No = 0,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 1
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Шлейф дымовых датчиков",
				Value = 0
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Комбинированный шлейф",
				Value = 1
			};
			var property1Parameter3 = new XDriverPropertyParameter()
			{
				Name = "Шлейф тепловых датчиков",
				Value = 2
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			driver.Properties.Add(property1);

			GKDriversHelper.AddIntProprety(driver, 1, "Порог питания, 0.1В", 0, 80, 1, 1000);
			GKDriversHelper.AddIntProprety(driver, 2, "Порог 1, 0.1В", 0, 250, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 2, 0.1В", 0, 750, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 3, 0.1В", 0, 1500, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 4, 0.1В", 0, 4500, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 5, 0.1В", 0, 6000, 1, 10000);

			return driver;
		}
	}
}