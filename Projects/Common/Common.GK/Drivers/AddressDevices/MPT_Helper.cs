using System;
using XFiresecAPI;

namespace Common.GK
{
	public class MPT_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x76,
				DriverType = XDriverType.MPT,
				UID = new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28"),
				Name = "Модуль пожаротушения МПТ-1",
				ShortName = "МПТ-1",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);

			AddLogic(driver, 0xBB, "Логика 1", 1);
			AddLogic(driver, 0xBC, "Логика 2", 2);
			AddLogic(driver, 0xBD, "Логика 3", 3);
			AddLogic(driver, 0xBE, "Логика 4", 4);
			AddLogic(driver, 0xBF, "Логика 5", 5);

			AddRegim(driver, 0xBB, "Режим 1", 1);
			AddRegim(driver, 0xBC, "Режим 2", 2);
			AddRegim(driver, 0xBD, "Режим 3", 3);
			AddRegim(driver, 0xBE, "Режим 4", 4);
			AddRegim(driver, 0xBF, "Режим 5", 5);


			var property1 = new XDriverProperty()
			{
				No = 0x8C,
				Name = "Статус МПТ",
				Caption = "Статус МПТ",
				Default = 1,
				Offset = 6,
				IsLowByte = true
			};
			GKDriversHelper.AddPropertyParameter(property1, "Ведущий", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Ведомый", 2);
			driver.Properties.Add(property1);

			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Масса", 0, 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Давление", 2, 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Двери-Окна", 4, 1);

			GKDriversHelper.AddIntProprety(driver, 0xC1, "Задержка 1", 0, 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC2, "Задержка 2", 0, 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC3, "Задержка 3", 0, 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC4, "Задержка 4", 0, 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC5, "Задержка 5", 0, 3, 0, 255).IsLowByte = true;

			GKDriversHelper.AddIntProprety(driver, 0xAB, "Удержание 1(с)", 0, 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAC, "Удержание 2(с)", 0, 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAD, "Удержание 3(с)", 0, 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAE, "Удержание 4(с)", 0, 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAF, "Удержание 5(с)", 0, 2, 0, 255).IsLowByte = true;

			GKDriversHelper.AddIntProprety(driver, 0xB1, "Период 1(с)", 0, 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB2, "Период 2(с)", 0, 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB3, "Период 3(с)", 0, 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB4, "Период 4(с)", 0, 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB5, "Период 5(с)", 0, 1, 0, 255).IsLowByte = true;

			AddControlType(driver, 0x87, "Контроль 1");
			AddControlType(driver, 0x88, "Контроль 2");
			AddControlType(driver, 0x89, "Контроль 3");
			AddControlType(driver, 0x8A, "Контроль 4");
			AddControlType(driver, 0x8B, "Контроль 5");

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Старт автоматики", 6,
				"Включено",
				"Выключено", 1, 1).IsLowByte = true;

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Пуск автоматики", 0,
				"Отмена пуска",
				"Пуск", 1, 1).IsLowByte = true;

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Восстановление автоматики", 4,
				"Восстановление",
				"Отмена восстановления", 1, 2).IsLowByte = true;

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Неисправность автоматики", 2,
				"Выключено",
				"Включено", 1, 1).IsLowByte = true;

			return driver;
		}

		static void AddControlType(XDriver driver, byte no, string propertyName)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 4,
				Offset = 0,
				IsLowByte = true
			};
			GKDriversHelper.AddPropertyParameter(property, "Состояние цепи не контролируется", 1);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется только на обрыв", 2);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется только на короткое замыкание", 3);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется на короткое замыкание и на обрыв", 4);
			driver.Properties.Add(property);
		}

		static void AddLogic(XDriver driver, byte no, string propertyName, ushort defaultValue = 1)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = defaultValue,
				Mask = 15,
				IsLowByte = true
			};
			GKDriversHelper.AddPropertyParameter(property, "Сирена", 1);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Уходи»", 2);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Не входи»", 3);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Автоматика отключена»", 4);
			GKDriversHelper.AddPropertyParameter(property, "Выход АУП", 5);
			driver.Properties.Add(property);
		}

		static void AddRegim(XDriver driver, byte no, string propertyName, ushort defaultValue = 1)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = defaultValue,
				Offset = 4,
				IsLowByte = true
			};
			GKDriversHelper.AddPropertyParameter(property, "Не включать", 1);
			GKDriversHelper.AddPropertyParameter(property, "Включить сразу", 2);
			GKDriversHelper.AddPropertyParameter(property, "Включить после паузы", 3);
			GKDriversHelper.AddPropertyParameter(property, "Включить на заданное время", 4);
			GKDriversHelper.AddPropertyParameter(property, "Включить после паузы на заданное время и выключить", 5);
			GKDriversHelper.AddPropertyParameter(property, "Переключать постоянно", 6);
			GKDriversHelper.AddPropertyParameter(property, "Начать переключение после паузы", 7);
			GKDriversHelper.AddPropertyParameter(property, "Переключать заданное время и оставить включенным", 8);
			GKDriversHelper.AddPropertyParameter(property, "Начать переключение после паузы, переключать заданное время и оставить включенным", 9);
			GKDriversHelper.AddPropertyParameter(property, "Переключать заданное время и оставить выключенным", 10);
			GKDriversHelper.AddPropertyParameter(property, "Начать переключение после паузы, переключать заданное время и оставить выключенным", 11);
			driver.Properties.Add(property);
		}

		static void AddDetectorState(XDriver driver, byte no, string propertyName, byte offset, ushort defaultValue)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 1,
				Offset = offset,
				Mask = 3,
				IsLowByte = true
			};
			GKDriversHelper.AddPropertyParameter(property, "Замкнутое", 1);
			GKDriversHelper.AddPropertyParameter(property, "Разомкнутое", 2);
			driver.Properties.Add(property);
		}
	}
}