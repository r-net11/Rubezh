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
                CanPlaceOnPlan = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);

			AddRegim(driver, 0xBB, "Логика работы выхода 1", 1);
			AddRegim(driver, 0xBC, "Логика работы выхода 2", 3);
			AddRegim(driver, 0xBD, "Логика работы выхода 3", 4);
			AddRegim(driver, 0xBE, "Логика работы выхода 4", 2);
			AddRegim(driver, 0xBF, "Логика работы выхода 5", 5);


			var property1 = new XDriverProperty()
			{
				No = 0x8C,
				Name = "Статус МПТ",
				Caption = "Статус МПТ",
				Default = 1,
				Offset = 6
			};
			GKDriversHelper.AddPropertyParameter(property1, "Ведущий", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Ведомый", 2);
			driver.Properties.Add(property1);

			AddControlType(driver, 0x87, "Тип контроля выхода 1");
			AddControlType(driver, 0x88, "Тип контроля выхода 2");
			AddControlType(driver, 0x89, "Тип контроля выхода 3");
			AddControlType(driver, 0x8A, "Тип контроля выхода 4");
			AddControlType(driver, 0x8B, "Тип контроля выхода 5");

			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Масса", 0, 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Давление", 2, 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Двери-Окна", 4, 1);

			GKDriversHelper.AddIntProprety(driver, 0xAB, "время включенного состояния выхода 1", 0, 2, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xAC, "время включенного состояния выхода 2", 0, 2, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xAD, "время включенного состояния выхода 3", 0, 2, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xAE, "время включенного состояния выхода 4", 0, 2, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xAF, "время включенного состояния выхода 5", 0, 2, 0, 255);

			GKDriversHelper.AddIntProprety(driver, 0xB1, "период переключения выхода 1", 0, 1, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xB2, "период переключения выхода 2", 0, 1, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xB3, "период переключения выхода 3", 0, 1, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xB4, "период переключения выхода 4", 0, 1, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xB5, "период переключения выхода 5", 0, 1, 0, 255);

			GKDriversHelper.AddIntProprety(driver, 0xC1, "Время задержки включения выхода 1", 0, 3, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xC2, "Время задержки включения выхода 2", 0, 3, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xC3, "Время задержки включения выхода 3", 0, 3, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xC4, "Время задержки включения выхода 4", 0, 3, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0xC5, "Время задержки включения выхода 5", 0, 60, 0, 255);

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Приоритет запуска", 0,
				"происходит отмена задержки запуска при нарушении датчика «Двери-окна» и рестарт после восстановления датчика «Двери-окна»",
				"не происходит отмена задержки запуска при нарушении датчика «Двери-окна»", 1, 1);

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Блокировка выключения режима «Автоматика включена» при неисправности", 2,
				"режим «Автоматика включена» отключается при неисправности источника питания прибора, при неисправности ШС, при нарушении датчика «Двери-окна»",
				"режим «Автоматика включена» не отключается при неисправности источника питания прибора, при неисправности ШС, при нарушении датчика «Двери-окна»", 1,1);

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Восстановление режима «Автоматика включена»", 4,
				"режим восстанавливается после восстановления датчика «Двери-окна»",
				"режим не восстанавливается после восстановления  датчика «Двери-окна», восстановление возможно по протоколу RSR", 1,2);

			GKDriversHelper.AddPlainEnumProprety(driver, 0xC6, "Состояние  режима «Автоматика включена» после включения питания", 6,
				"после включения питания  режим «Автоматика включена» включен",
				"после включения питания  режим «Автоматика включена» отключен", 1, 1);

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
				Offset = 0
			};
			GKDriversHelper.AddPropertyParameter(property, "Состояние цепи не контролируется", 1);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется только на обрыв", 2);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется только на короткое замыкание", 3);
			GKDriversHelper.AddPropertyParameter(property, "Цепь контролируется на короткое замыкание и на обрыв", 4);
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
				Offset = 4
			};
			GKDriversHelper.AddPropertyParameter(property, "Сирена", 1);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Уходи»", 2);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Не входи»", 3);
			GKDriversHelper.AddPropertyParameter(property, "Табличка «Автоматика отключена»", 4);
			GKDriversHelper.AddPropertyParameter(property, "Выход АУП", 5);
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
				Offset = offset
			};
			GKDriversHelper.AddPropertyParameter(property, "Замкнутое", 1);
			GKDriversHelper.AddPropertyParameter(property, "Разомкнутое", 2);
			driver.Properties.Add(property);
		}
	}
}