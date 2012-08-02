using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class BUZHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.Valve);

			CommonHelper.AddIntProprety(xDriver, 0x84, "Уставка времени хода задвижки", 0, 1, 1, 65535);
			CommonHelper.AddIntProprety(xDriver, 0x8e, "Время отложенного запуска, с", 0, 0, 0, 255);

			var property3 = new XDriverProperty()
			{
				No = 0x8f,
				Name = "Время Т2 отложенного запуска, мин",
				Caption = "Время Т2 отложенного запуска, мин",
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Default = 0,
				Min = 1,
				Max = 360
			};
			xDriver.Properties.Add(property3);

			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "концевой выключатель «Открыто»", 0, "концевой выключатель «Открыто» НР", "концевой выключатель «Открыто» НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "муфтовый выключатель Открыто/ДУ Открыть", 1, "муфтовый выключатель Открыто/ДУ Открыть НР", "муфтовый выключатель Открыто/ДУ Открыть НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "концевой выключатель «Закрыто»", 2, "концевой выключатель «Закрыто» НР", "концевой выключатель «Закрыто» НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "муфтовый выключатель Закрыто/ДУ Закрыть", 3, "муфтовый выключатель Закрыто/ДУ Закрыть НР", "датчик 4 НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "кнопка Открыть УЗЗ", 4, "кнопка Открыть УЗЗ НР", "кнопка Открыть УЗЗ НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "кнопка Закрыть УЗЗ", 5, "кнопка Закрыть УЗЗ НР", "кнопка Закрыть УЗЗ НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "кнопка Стоп УЗЗ", 6, "кнопка Стоп УЗЗ НР", "кнопка Стоп УЗЗ НЗ");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "муфтовые выключатели", 9, "муфтовые выключатели есть", "муфтовых выключателей нет");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "датчик уровня", 10, "датчиков уровня нет", "датчики уровня есть");
			CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "функция УЗЗ", 11, "функция УЗЗ отключена", "функция УЗЗ включена");
		}
	}
}