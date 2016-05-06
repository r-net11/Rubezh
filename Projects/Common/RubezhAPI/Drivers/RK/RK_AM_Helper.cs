using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RK_AM_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x1B,
				DriverType = GKDriverType.RK_AM,
				UID = new Guid("FE25BE41-EBE9-4469-A176-47F3F4EF7D05"),
				Name = "Метка адресная радиоканальная",
				ShortName = "АМ-РК",
				HasGuardZone = true,
				HasZone = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddRadioChanelProperties(driver);

			var property1 = new GKDriverProperty()
			{
				No = 3,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 1
			};
			var property1Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "Один контакт, нормально замкнутый",
				Value = 0
			};
			var property1Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "Один контакт, нормально разомкнутый",
				Value = 1
			};
			var property1Parameter3 = new GKDriverPropertyParameter()
			{
				Name = "Два контакта, нормально замкнутые",
				Value = 2
			};
			var property1Parameter4 = new GKDriverPropertyParameter()
			{
				Name = "Два контакта, нормально разомкнутые",
				Value = 3
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			property1.Parameters.Add(property1Parameter4);
			driver.Properties.Add(property1);

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

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Сопротивление, Ом", InternalName = "Resistance" });

			return driver;
		}
	}
}