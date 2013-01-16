using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class BUN_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x70,
				DriverType = XDriverType.Pump,
				UID = new Guid("8bff7596-aef4-4bee-9d67-1ae3dc63ca94"),
				Name = "Шкаф управления насосом",
				ShortName = "ШУН",
				IsControlDevice = true,
				HasLogic = true,
				CanPlaceOnPlan = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);

			GKDriversHelper.AddIntProprety(driver, 0x8b, "Максимальное время перезапуска, 0.1 с", 0, 1, 1, 255);
			GKDriversHelper.AddIntProprety(driver, 0x8b, "Время разновременного пуска, с", 0, 1, 1, 255);

			var property3 = new XDriverProperty()
			{
				No = 0x8b,
				Name = "Разрешение функции УЗН",
				Caption = "Разрешение функции УЗН",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 3 + 8
			};
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x8b,
				Name = "Настройка выхода",
				Caption = "Настройка выхода",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
			};
			var parmeter1 = new XDriverPropertyParameter()
			{
				Name = "ЭКМ на выходе насоса",
				Value = 0
			};
			var parmeter2 = new XDriverPropertyParameter()
			{
				Name = "УЗН Старт",
				Value = 1
			};
			var parmeter3 = new XDriverPropertyParameter()
			{
				Name = "УЗН Стоп",
				Value = 2
			};
			property4.Parameters.Add(parmeter1);
			property4.Parameters.Add(parmeter2);
			property4.Parameters.Add(parmeter3);
			driver.Properties.Add(property4);


			return driver;
		}
	}
}