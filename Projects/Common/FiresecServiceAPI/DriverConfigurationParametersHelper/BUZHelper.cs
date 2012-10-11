using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class BUZHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.Valve);
			driver.HasConfigurationProperties = true;

			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x84,
				Name = "Время хода задвижки, сек",
				Caption = "Время хода задвижки, сек",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Default = "1",
				Min = 1,
				Max = 65535,
				LargeValue = true
			};
			driver.Properties.Add(property);
			//ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Время хода задвижки, сек", 0, 1, 1, 65535);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8e, "Задержка включения, сек", 0, 0, 0, 250);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8f, "Время удержания запуска, мин", 0, 0, 0, 360);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Открыто»", 0, 
				"0 нормальго-разомкнутый", 
				"1 нормально-замкнутый", 0, 0, 1);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Открыто/ДУ Открыть", 1,
				"0 нормальго-разомкнутый",
				"1 нормально-замкнутый", 0, 1, 2);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Закрыто»", 2,
				"0 нормальго-разомкнутый",
				"1 нормально-замкнутый", 0, 2, 3);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Закрыто/ДУ Закрыть", 3,
				"0 нормальго-разомкнутый",
				"1 нормально-замкнутый", 0, 3, 4);
			
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Открыть УЗЗ", 4,
				"0 нормальго-разомкнутый",
				"1 нормально-замкнутый", 0, 4, 5);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Закрыть УЗЗ", 5,
				"0 нормальго-разомкнутый",
				"1 нормально-замкнутый", 0, 5, 6);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Стоп УЗЗ", 6,
				"0 нормальго-разомкнутый",
				"1 нормально-замкнутый", 0, 6, 7);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовые выключатели", 1,
				"0 нет",
				"1 есть", 0, 1, 2, false, true);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "датчик уровня", 2, 
				"0 нет", 
				"1 есть", 0, 2, 3, false, true);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "функция УЗЗ", 3, 
				"0 отключена",
				"1 включена", 0, 3, 4, false, true);
		}
	}
}