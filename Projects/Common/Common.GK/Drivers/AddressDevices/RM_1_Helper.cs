using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RM_1_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x75,
				DriverType = XDriverType.RM_1,
				UID = new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace"),
				Name = "Релейный исполнительный модуль РМ-1",
				ShortName = "РМ-1",
				IsControlDevice = true,
				HasLogic = true,
                CanPlaceOnPlan = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);

			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Конфигурация релейного модуля",
				Caption = "Конфигурация релейного модуля",
				Default = 1
			};
			GKDriversHelper.AddPropertyParameter(property1, "Разомкнуто Замкнуто", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Разомкнуто Мерцает", 2);
			GKDriversHelper.AddPropertyParameter(property1, "Замкнуто Разомкнуто", 3);
			GKDriversHelper.AddPropertyParameter(property1, "Замкнуто Мерцает", 4);
			GKDriversHelper.AddPropertyParameter(property1, "Мерцает Разомкнуто", 5);
			GKDriversHelper.AddPropertyParameter(property1, "Мерцает Замкнуто", 6);
			driver.Properties.Add(property1);

			GKDriversHelper.AddIntProprety(driver, 0x83, "Задержка на пуск", 0, 0, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0x83, "Время удержания", 8, 0, 0, 255);

			var property2 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип контроля выхода",
				Caption = "Тип контроля выхода",
				Default = 1
			};
			GKDriversHelper.AddPropertyParameter(property2, "Состояние цепи не контролируется", 1);
			GKDriversHelper.AddPropertyParameter(property2, "Цепь контролируется только на обрыв", 2);
			GKDriversHelper.AddPropertyParameter(property2, "Цепь контролируется только на короткое замыкание", 3);
			GKDriversHelper.AddPropertyParameter(property2, "Цепь контролируется на короткое замыкание и на обрыв", 4);
			driver.Properties.Add(property2);

			return driver;
		}
	}
}