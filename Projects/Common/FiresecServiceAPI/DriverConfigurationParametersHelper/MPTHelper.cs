using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MPTHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MPT);
			driver.HasConfigurationProperties = true;

			AddControlType(driver, 0x87, "Тип контроля выхода 1");
			AddControlType(driver, 0x88, "Тип контроля выхода 2");
			AddControlType(driver, 0x89, "Тип контроля выхода 3");
			AddControlType(driver, 0x8A, "Тип контроля выхода 4");
			AddControlType(driver, 0x8B, "Тип контроля выхода 5");

			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Масса", 0);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Давление", 2);
			AddDetectorState(driver, 0x8C, "Нормальное состояние датчика Двери-Окна", 4);

			ConfigurationDriverHelper.AddIntProprety(driver, 0xAB, "время включенного состояния выхода 1", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xAC, "время включенного состояния выхода 2", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xAD, "время включенного состояния выхода 3", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xAE, "время включенного состояния выхода 4", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xAF, "время включенного состояния выхода 5", 0, 0, 0, 255, true);

			ConfigurationDriverHelper.AddIntProprety(driver, 0xB1, "период переключения выхода 1", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xB2, "период переключения выхода 2", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xB3, "период переключения выхода 3", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xB4, "период переключения выхода 4", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xB5, "период переключения выхода 5", 0, 0, 0, 255, true);

			ConfigurationDriverHelper.AddIntProprety(driver, 0xC1, "Время задержки включения выхода 1", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xC2, "Время задержки включения выхода 2", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xC3, "Время задержки включения выхода 3", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xC4, "Время задержки включения выхода 4", 0, 0, 0, 255, true);
			ConfigurationDriverHelper.AddIntProprety(driver, 0xC5, "Время задержки включения выхода 5", 0, 0, 0, 255, true);

			AddRegime(driver, 0xBB, "режим работы выхода 1");
			AddRegime(driver, 0xBC, "режим работы выхода 2");
			AddRegime(driver, 0xBD, "режим работы выхода 3");
			AddRegime(driver, 0xBE, "режим работы выхода 4");
			AddRegime(driver, 0xBF, "режим работы выхода 5");

			AddLogic(driver, 0xBB, "логика работы выхода 1");
			AddLogic(driver, 0xBC, "логика работы выхода 2");
			AddLogic(driver, 0xBD, "логика работы выхода 3");
			AddLogic(driver, 0xBE, "логика работы выхода 4");
			AddLogic(driver, 0xBF, "логика работы выхода 5");


			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0xC6, "Приоритет запуска", 0,
				"происходит отмена задержки запуска при нарушении датчика «Двери-окна» и рестарт после восстановления датчика «Двери-окна»",
				"не происходит отмена задержки запуска при нарушении датчика «Двери-окна»", 1, 1, 2, true);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0xC6, "Блокировка выключения режима «Автоматика включена» при неисправности", 2,
				"режим «Автоматика включена» отключается при неисправности источника питания прибора, при неисправности ШС, при нарушении датчика «Двери-окна»",
				"режим «Автоматика включена» не отключается при неисправности источника питания прибора, при неисправности ШС, при нарушении датчика «Двери-окна»", 1, 3, 4, true);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0xC6, "Восстановление режима «Автоматика включена»", 4,
				"режим восстанавливается после восстановления датчика «Двери-окна»",
				"режим не восстанавливается после восстановления  датчика «Двери-окна», восстановление возможно по протоколу RSR", 1, 5, 6, true);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0xC6, "Состояние  режима «Автоматика включена» после включения питания", 6,
				"после включения питания  режим «Автоматика включена» включен",
				"после включения питания  режим «Автоматика включена» отключен", 1, 7, 8, true);

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x8C,
				Name = "Статус МПТ",
				Caption = "Статус МПТ",
				Default = "1",
				BitOffset = 6,
				UseMask = true
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Ведущий", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Ведомый", 2);
			driver.Properties.Add(property1);
		}

		static void AddControlType(Driver driver, byte no, string propertyName)
		{
			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = "1",
				UseMask = true
			};
			ConfigurationDriverHelper.AddPropertyParameter(property, "Состояние цепи не контролируется", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property, "Цепь контролируется только на обрыв", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property, "Цепь контролируется только на короткое замыкание", 3);
			ConfigurationDriverHelper.AddPropertyParameter(property, "Цепь контролируется на короткое замыкание и на обрыв", 4);
			driver.Properties.Add(property);
		}

		static void AddRegime(Driver driver, byte no, string propertyName)
		{
			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = "2",
				MaxBit = 3
			};
			ConfigurationDriverHelper.AddPropertyParameter(property, "1 Сирена", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property, "2 Табличка «Уходи»", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property, "3 Табличка «Не входи»", 3);
			ConfigurationDriverHelper.AddPropertyParameter(property, "4 Табличка «Автоматика отключена»", 4);
			ConfigurationDriverHelper.AddPropertyParameter(property, "5 Выход АУП", 5);
			driver.Properties.Add(property);
		}

		static void AddLogic(Driver driver, byte no, string propertyName)
		{
			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = "1",
				BitOffset = 4
			};
			ConfigurationDriverHelper.AddPropertyParameter(property, "1 Не включать", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property, "2 Включить сразу", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property, "3 Включить после паузы", 3);
			ConfigurationDriverHelper.AddPropertyParameter(property, "4 Включить на заданное время", 4);
			ConfigurationDriverHelper.AddPropertyParameter(property, "5 Включить после паузы на заданное время и выключить", 5);
			ConfigurationDriverHelper.AddPropertyParameter(property, "6 Переключать постоянно", 6);
			ConfigurationDriverHelper.AddPropertyParameter(property, "7 Начать переключение после паузы", 7);
			ConfigurationDriverHelper.AddPropertyParameter(property, "8 Переключать заданное время и оставить выключенным", 8);
			ConfigurationDriverHelper.AddPropertyParameter(property, "9 Начать переключение после паузы, переключать заданное время и оставить выключенным", 9);
			ConfigurationDriverHelper.AddPropertyParameter(property, "10 Переключать заданное время и оставить выключенным", 10);
			ConfigurationDriverHelper.AddPropertyParameter(property, "11 Начать переключение после паузы, переключать заданное время и оставить выключенным", 11);
			driver.Properties.Add(property);
		}

		static void AddDetectorState(Driver driver, byte no, string propertyName, byte offset)
		{
			var property = new DriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = "1",
				BitOffset = offset,
				UseMask = true
			};
			ConfigurationDriverHelper.AddPropertyParameter(property, "Замкнутое", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property, "Разомкнутое", 2);
			driver.Properties.Add(property);
		}
	}
}