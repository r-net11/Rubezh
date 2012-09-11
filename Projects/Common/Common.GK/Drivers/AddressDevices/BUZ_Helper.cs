using System;
using XFiresecAPI;

namespace Common.GK
{
	public class BUZ_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x71,
				DriverType = XDriverType.Valve,
				UID = new Guid("4935848f-0084-4151-a0c8-3a900e3cb5c5"),
				Name = "Блок управления задвижкой",
				ShortName = "БУЗ",
				MaxAddressOnShleif = 8,
				IsControlDevice = true,
				HasLogic = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Уставка времени хода задвижки", 0, 1, 1, 65535);
			GKDriversHelper.AddIntProprety(driver, 0x8e, "Время отложенного запуска, с", 0, 0, 0, 255);

			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Открыто»", 0, "концевой выключатель «Открыто» НР", "концевой выключатель «Открыто» НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Открыто/ДУ Открыть", 1, "муфтовый выключатель Открыто/ДУ Открыть НР", "муфтовый выключатель Открыто/ДУ Открыть НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Закрыто»", 2, "концевой выключатель «Закрыто» НР", "концевой выключатель «Закрыто» НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Закрыто/ДУ Закрыть", 3, "муфтовый выключатель Закрыто/ДУ Закрыть НР", "датчик 4 НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Открыть УЗЗ", 4, "кнопка Открыть УЗЗ НР", "кнопка Открыть УЗЗ НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Закрыть УЗЗ", 5, "кнопка Закрыть УЗЗ НР", "кнопка Закрыть УЗЗ НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Стоп УЗЗ", 6, "кнопка Стоп УЗЗ НР", "кнопка Стоп УЗЗ НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовые выключатели", 9, "муфтовые выключатели есть", "муфтовых выключателей нет");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "датчик уровня", 10, "датчиков уровня нет", "датчики уровня есть");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "функция УЗЗ", 11, "функция УЗЗ отключена", "функция УЗЗ включена");

			return driver;
		}
	}
}