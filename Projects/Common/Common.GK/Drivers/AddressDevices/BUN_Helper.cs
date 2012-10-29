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

			if (driver.DriverType != XDriverType.Pump)
			{
				GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Логика", 0 + 8, "на входах 1 и 2 два обычных датчика", "на входах 1 и 2 два обычных датчика");
			}

			var property3 = new XDriverProperty()
			{
				No = 0x8b,
				Name = "разрешение функции УЗН",
				Caption = "разрешение функции УЗН",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 3 + 8
			};
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x8b,
				Name = "наличие в прошивке логики работы с УЗН",
				Caption = "наличие в прошивке логики работы с УЗН",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 7 + 8
			};
			driver.Properties.Add(property4);

			if (driver.DriverType == XDriverType.Pump)
			{
				GKDriversHelper.AddBoolProprety(driver, 0x8b, "ЭКМ на выходе насоса", 0);
				GKDriversHelper.AddBoolProprety(driver, 0x8b, "УЗН Старт", 1);
			}
			else
			{
				GKDriversHelper.AddBoolProprety(driver, 0x8b, "ДД/ДУ Пуск", 0);
				GKDriversHelper.AddBoolProprety(driver, 0x8b, "ДД/ДУ Стоп", 1);
			}

			if (driver.DriverType == XDriverType.Pump)
			{
				GKDriversHelper.AddBoolProprety(driver, 0x8b, "УЗН Стоп", 2);
			}

			if (driver.DriverType == XDriverType.Pump)
			{
				GKDriversHelper.AddIntProprety(driver, 0x84, "уставка времени ожидания выхода насоса на режим, с", 0, 3, 3, 30);
			}

			return driver;
		}
	}
}