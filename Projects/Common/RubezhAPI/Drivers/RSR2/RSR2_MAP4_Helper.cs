using RubezhAPI.GK;
using System;

namespace RubezhAPI
{
	public static class RSR2_MAP4_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xE1,
				DriverType = GKDriverType.RSR2_MAP4,
				UID = new Guid("42B3C448-2FDD-43D4-AEE0-F173CB8D6CF8"),
				Name = "Метка адресная пожарная",
				ShortName = "АМП",
				HasGuardZone = true,
				HasZone = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.Other
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Failure);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			var property1 = new GKDriverProperty()
			{
				No = 0,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 0
			};
			var property1Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "Шлейф дымовых датчиков",
				Value = 0
			};
			var property1Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "Комбинированный шлейф",
				Value = 1
			};
			var property1Parameter3 = new GKDriverPropertyParameter()
			{
				Name = "Шлейф тепловых датчиков",
				Value = 2
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			driver.Properties.Add(property1);

			GKDriversHelper.AddIntProprety(driver, 1, "Порог питания, В", 80, 0, 280).Multiplier = 10;
			GKDriversHelper.AddIntProprety(driver, 2, "Порог 1", 250, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 2", 750, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 3", 1500, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 4", 4500, 1, 10000);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 5", 6000, 1, 10000);

			var property2 = new GKDriverProperty()
			{
				IsAUParameter = false,
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				Name = "Сообщение для нормы",
				Caption = "Сообщение для нормы",
				StringDefault = "Сообщение для нормы"
			};
			driver.Properties.Add(property2);

			var property3 = new GKDriverProperty()
			{
				IsAUParameter = false,
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				Name = "Сообщение для сработки 1",
				Caption = "Сообщение для сработки 1",
				StringDefault = "Сообщение для сработки 1"
			};
			driver.Properties.Add(property3);

			var property4 = new GKDriverProperty()
			{
				IsAUParameter = false,
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				Name = "Сообщение для сработки 2",
				Caption = "Сообщение для сработки 2",
				StringDefault = "Сообщение для сработки 2"
			};
			driver.Properties.Add(property4);

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 1, Name = "Сопротивление, Ом", InternalName = "Resistance" });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Питание, В", Multiplier = 10 });


			return driver;
		}
	}
}