using System;
using FiresecAPI.GK;

namespace GKProcessor
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
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);

			AddLogic(driver, 0xBB, "логика работы выхода 1", 1);
			AddLogic(driver, 0xBC, "логика работы выхода 2", 3);
			AddLogic(driver, 0xBD, "логика работы выхода 3", 4);
			AddLogic(driver, 0xBE, "логика работы выхода 4", 2);
			AddLogic(driver, 0xBF, "логика работы выхода 5", 5);

			AddRegim(driver, 0xBB, "режим работы выхода 1", 2);
			AddRegim(driver, 0xBC, "режим работы выхода 2", 6);
			AddRegim(driver, 0xBD, "режим работы выхода 3", 6);
			AddRegim(driver, 0xBE, "режим работы выхода 4", 6);
			AddRegim(driver, 0xBF, "режим работы выхода 5", 10);

			var property1 = GKDriversHelper.AddPlainEnumProprety(driver, 0x8C, 6, "Статус МПТ", "Ведущий", "Ведомый");
			property1.IsLowByte = true;
			property1.CanNotEdit = true;

			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Масса", 0, 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Давление", 2, 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Двери-Окна", 4, 1);

			GKDriversHelper.AddIntProprety(driver, 0xC1, "Задержка включения выхода 1, с", 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC2, "Задержка включения выхода 2, с", 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC3, "Задержка включения выхода 3, с", 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC4, "Задержка включения выхода 4, с", 3, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xC5, "Задержка включения выхода 5, с", 60, 0, 255).IsLowByte = true;

			GKDriversHelper.AddIntProprety(driver, 0xAB, "Время включенного состояния выхода 1, с", 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAC, "Время включенного состояния выхода 2, с", 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAD, "Время включенного состояния выхода 3, с", 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAE, "Время включенного состояния выхода 4, с", 2, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xAF, "Время включенного состояния выхода 5, с", 2, 0, 255).IsLowByte = true;

			GKDriversHelper.AddIntProprety(driver, 0xB1, "Период переключения выхода 1, с", 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB2, "Период переключения выхода 2, с", 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB3, "Период переключения выхода 3, с", 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB4, "Период переключения выхода 4, с", 1, 0, 255).IsLowByte = true;
			GKDriversHelper.AddIntProprety(driver, 0xB5, "Период переключения выхода 5, с", 1, 0, 255).IsLowByte = true;

			AddControlType(driver, 0x87, "Тип контроля выхода 1");
			AddControlType(driver, 0x88, "Тип контроля выхода 2");
			AddControlType(driver, 0x89, "Тип контроля выхода 3");
			AddControlType(driver, 0x8A, "Тип контроля выхода 4");
			AddControlType(driver, 0x8B, "Тип контроля выхода 5");

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, 6, "Приоритет запуска",
				"Происходит останов задержки запуска при открытии дверей или окон(срабатывание датчика «Двери-окна») и рестарт после закрытия дверей и окон",
				"Не происходит останов задержки запуска при срабатывании датчика «Двери-окна»").IsLowByte = true;

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, 0, "Блокировка отключения автоматики",
				"Режим «Автоматика включена» отключается при неисправности источника питания прибора, при неисправности ШС, при срабатывании датчика «Двери-окна»",
				"Режим «Автоматика включена» не отключается при неисправности источника питания прибора, при неисправности ШС, при срабатывании датчика «Двери-окна»").IsLowByte = true;

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, 4, "Восстановление режима «Автоматика включена»",
				"Режим восстанавливается после восстановления датчика «Двери-окна»",
				"Режим не восстанавливается после восстановления  датчика «Двери-окна», восстановление возможно ключем ТМ").IsLowByte = true;

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, 2, "Состояние  режима «Автоматика включена» после включения питания",
				"после включения питания  режим «Автоматика включена» включен",
				"после включения питания  режим «Автоматика включена» отключен").IsLowByte = true;

			return driver;
		}

		private static void AddControlType(XDriver driver, byte no, string propertyName)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 4,
				IsLowByte = true,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType
			};
			GKDriversHelper.AddPropertyParameter(property, "Состояние цепи не контролируется", 1);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется только на обрыв", 2);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется только на короткое замыкание", 3);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется на короткое замыкание и на обрыв", 4);
			driver.Properties.Add(property);
		}

		private static void AddLogic(XDriver driver, byte no, string propertyName, ushort defaultValue = 1)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = defaultValue,
				Mask = 0x0F,
				IsLowByte = true,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType
			};
			GKDriversHelper.AddPropertyParameter(property, "Сирена", 1);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Уходи»", 2);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Не входи»", 3);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Автоматика отключена»", 4);
			GKDriversHelper.AddPropertyParameter(property, "Выход АУП", 5);
			driver.Properties.Add(property);
		}

		private static void AddRegim(XDriver driver, byte no, string propertyName, ushort defaultValue = 1)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = (ushort)(defaultValue << 4),
				Mask = 0xF0,
				IsLowByte = true,
				
			};
			GKDriversHelper.AddPropertyParameter(property, "Не включать", 1 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Включить сразу", 2 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Включить после паузы", 3 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Включить на заданное время", 4 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Включить после паузы на заданное время и выключить", 5 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Переключать постоянно", 6 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Начать переключение после паузы", 7 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Переключать заданное время и оставить включенным", 8 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Начать переключение после паузы, переключать заданное время и оставить включенным", 9 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Переключать заданное время и оставить выключенным", 10 << 4);
			GKDriversHelper.AddPropertyParameter(property, "Начать переключение после паузы, переключать заданное время и оставить выключенным", 11 << 4);
			driver.Properties.Add(property);
		}

		static void AddDetectorState(XDriver driver, byte no, string propertyName, byte offset, ushort defaultValue)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = (ushort)(defaultValue << offset),
				Mask = (short)((1 << offset) + (1 << (offset + 1))),
				IsLowByte = true,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType
			};
			GKDriversHelper.AddPropertyParameter(property, "Замкнутое", 1 << offset);
			GKDriversHelper.AddPropertyParameter(property, "Разомкнутое", 2 << offset);
			driver.Properties.Add(property);
		}
	}
}