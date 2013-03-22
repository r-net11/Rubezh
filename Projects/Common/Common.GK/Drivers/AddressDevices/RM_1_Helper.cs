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
                IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);

			driver.AvailableCommands.Add(XStateType.TurnOn_InManual);
			driver.AvailableCommands.Add(XStateType.TurnOnNow_InManual);
			driver.AvailableCommands.Add(XStateType.TurnOff_InManual);

			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Конфигурация релейного модуля",
				Caption = "Конфигурация релейного модуля",
				Default = 1,
                IsLowByte = true
			};
			GKDriversHelper.AddPropertyParameter(property1, "Разомкнуто Замкнуто", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Разомкнуто Мерцает", 2);
			GKDriversHelper.AddPropertyParameter(property1, "Замкнуто Разомкнуто", 3);
			GKDriversHelper.AddPropertyParameter(property1, "Замкнуто Мерцает", 4);
			GKDriversHelper.AddPropertyParameter(property1, "Мерцает Разомкнуто", 5);
			GKDriversHelper.AddPropertyParameter(property1, "Мерцает Замкнуто", 6);
			driver.Properties.Add(property1);

			GKDriversHelper.AddIntProprety(driver, 0x83, "Задержка на пуск, с", 0, 0, 0, 255).IsLowByte=true;
			GKDriversHelper.AddIntProprety(driver, 0x83, "Время удержания, с", 8, 0, 0, 255);
			//GKDriversHelper.AddIntProprety(driver, 0x84, "Недокументированный параметр", 0, 1, 1, 1).IsLowByte = true;

			var property2 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип контроля выхода",
				Caption = "Тип контроля выхода",
				Default = 1,
                IsLowByte=true
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