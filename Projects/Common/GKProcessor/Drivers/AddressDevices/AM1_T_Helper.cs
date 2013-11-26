using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class AM1_T_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xD2,
				DriverType = XDriverType.AM1_T,
				UID = new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8"),
				Name = "Технологическая адресная метка АМ1-Т",
				ShortName = "АМ1-Т",
				HasZone = false,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			var property1 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 5
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Технологический шлейф с контролем одного нормально-замкнутого контакта",
				Value = 4
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Технологический шлейф с контролем одного нормально-разомкнутого контакта",
				Value = 5
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				IsAUParameter = false,
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				Name = "NormMessage",
				Caption = "Сообщение для нормы",
				StringDefault = "Сообщение для нормы"
			};
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				IsAUParameter = false,
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				Name = "OnMessage",
				Caption = "Сообщение для сработки",
				StringDefault = "Сообщение для сработки"
			};
			driver.Properties.Add(property3);

			return driver;
		}
	}
}