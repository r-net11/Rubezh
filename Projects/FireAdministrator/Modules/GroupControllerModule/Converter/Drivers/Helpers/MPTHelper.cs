using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converter
{
	public class MPTHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.MPT);

			AddControlType(xDriver, 0x87, "Тип контроля выхода 1");
			AddControlType(xDriver, 0x88, "Тип контроля выхода 2");
			AddControlType(xDriver, 0x89, "Тип контроля выхода 3");
			AddControlType(xDriver, 0x8A, "Тип контроля выхода 4");
			AddControlType(xDriver, 0x8B, "Тип контроля выхода 5");

			AddDetectorState(xDriver, 0x8C, "Нормальное состояние датчика Масса", 0);
			AddDetectorState(xDriver, 0x8C, "Нормальное состояние датчика Давление", 2);
			AddDetectorState(xDriver, 0x8C, "Нормальное состояние датчика Двери-Окна", 4);

			CommonHelper.AddIntProprety(xDriver, 0xAB, "время включенного состояния выхода 1", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xAC, "время включенного состояния выхода 2", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xAD, "время включенного состояния выхода 3", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xAE, "время включенного состояния выхода 4", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xAF, "время включенного состояния выхода 5", 0, 0, 0, 255);

			CommonHelper.AddIntProprety(xDriver, 0xB1, "период переключения выхода 1", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xB2, "период переключения выхода 2", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xB3, "период переключения выхода 3", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xB4, "период переключения выхода 4", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xB5, "период переключения выхода 5", 0, 0, 0, 255);

			CommonHelper.AddIntProprety(xDriver, 0xC1, "Время задержки включения выхода 1", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xC2, "Время задержки включения выхода 2", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xC3, "Время задержки включения выхода 3", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xC4, "Время задержки включения выхода 4", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0xC5, "Время задержки включения выхода 5", 0, 0, 0, 255);

			AddRegim(xDriver, 0xBB, "режим и логика работы выхода 1");
			AddRegim(xDriver, 0xBC, "режим и логика работы выхода 2");
			AddRegim(xDriver, 0xBD, "режим и логика работы выхода 3");
			AddRegim(xDriver, 0xBE, "режим и логика работы выхода 4");
			AddRegim(xDriver, 0xBF, "режим и логика работы выхода 5");

			CommonHelper.AddPlainEnumProprety(xDriver, 0xC6, "Приоритет запуска", 0,
				"происходит отмена задержки запуска при нарушении датчика «Двери-окна» и рестарт после восстановления датчика «Двери-окна»",
				"не происходит отмена задержки запуска при нарушении датчика «Двери-окна»", 1);

			CommonHelper.AddPlainEnumProprety(xDriver, 0xC6, "Блокировка выключения режима «Автоматика включена» при неисправности", 2,
				"режим «Автоматика включена» отключается при неисправности источника питания прибора, при неисправности ШС, при нарушении датчика «Двери-окна»",
				"режим «Автоматика включена» не отключается при неисправности источника питания прибора, при неисправности ШС, при нарушении датчика «Двери-окна»", 1);

			CommonHelper.AddPlainEnumProprety(xDriver, 0xC6, "Восстановление режима «Автоматика включена»", 4,
				"режим восстанавливается после восстановления датчика «Двери-окна»",
				"режим не восстанавливается после восстановления  датчика «Двери-окна», восстановление возможно по протоколу RSR", 1);

			CommonHelper.AddPlainEnumProprety(xDriver, 0xC6, "Состояние  режима «Автоматика включена» после включения питания", 6,
				"после включения питания  режим «Автоматика включена» включен",
				"после включения питания  режим «Автоматика включена» отключен", 1);

			var property1 = new XDriverProperty()
			{
				No = 0x8C,
				Name = "Статус МПТ",
				Caption = "Статус МПТ",
				Default = 1,
				Offset = 6
			};
			CommonHelper.AddPropertyParameter(property1, "Ведущий", 1);
			CommonHelper.AddPropertyParameter(property1, "Ведомый", 2);
			xDriver.Properties.Add(property1);
		}

		static void AddControlType(XDriver xDriver, byte no, string propertyName)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 1,
				Offset = 0
			};
			CommonHelper.AddPropertyParameter(property, "Состояние цепи не контролируется", 1);
			CommonHelper.AddPropertyParameter(property, "Цепь контролируется только на обрыв", 2);
			CommonHelper.AddPropertyParameter(property, "Цепь контролируется только на короткое замыкание", 3);
			CommonHelper.AddPropertyParameter(property, "Цепь контролируется на короткое замыкание и на обрыв", 4);
			xDriver.Properties.Add(property);
		}

		static void AddRegim(XDriver xDriver, byte no, string propertyName)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 1,
				Offset = 0
			};
			CommonHelper.AddPropertyParameter(property, "Сирена", 1);
			CommonHelper.AddPropertyParameter(property, "Табличка «Уходи»", 2);
			CommonHelper.AddPropertyParameter(property, "Табличка «Не входи»", 3);
			CommonHelper.AddPropertyParameter(property, "Табличка «Автоматика отключена»", 4);
			CommonHelper.AddPropertyParameter(property, "Выход АУП", 4);
			xDriver.Properties.Add(property);
		}

		static void AddDetectorState(XDriver xDriver, byte no, string propertyName, byte offset)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 1,
				Offset = offset
			};
			CommonHelper.AddPropertyParameter(property, "Замкнутое", 1);
			CommonHelper.AddPropertyParameter(property, "Разомкнутое", 2);
			xDriver.Properties.Add(property);
		}
	}
}