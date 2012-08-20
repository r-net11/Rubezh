using System.Linq;
using FiresecClient;
using XFiresecAPI;
using System;

namespace Common.GK
{
	public class BUZHelper
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

			CommonHelper.AddIntProprety(driver, 0x84, "Уставка времени хода задвижки", 0, 1, 1, 65535);
			CommonHelper.AddIntProprety(driver, 0x8e, "Время отложенного запуска, с", 0, 0, 0, 255);

			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Открыто»", 0, "концевой выключатель «Открыто» НР", "концевой выключатель «Открыто» НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Открыто/ДУ Открыть", 1, "муфтовый выключатель Открыто/ДУ Открыть НР", "муфтовый выключатель Открыто/ДУ Открыть НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Закрыто»", 2, "концевой выключатель «Закрыто» НР", "концевой выключатель «Закрыто» НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Закрыто/ДУ Закрыть", 3, "муфтовый выключатель Закрыто/ДУ Закрыть НР", "датчик 4 НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Открыть УЗЗ", 4, "кнопка Открыть УЗЗ НР", "кнопка Открыть УЗЗ НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Закрыть УЗЗ", 5, "кнопка Закрыть УЗЗ НР", "кнопка Закрыть УЗЗ НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Стоп УЗЗ", 6, "кнопка Стоп УЗЗ НР", "кнопка Стоп УЗЗ НЗ");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовые выключатели", 9, "муфтовые выключатели есть", "муфтовых выключателей нет");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "датчик уровня", 10, "датчиков уровня нет", "датчики уровня есть");
			CommonHelper.AddPlainEnumProprety(driver, 0x8d, "функция УЗЗ", 11, "функция УЗЗ отключена", "функция УЗЗ включена");

			return driver;
		}
	}
}