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
				Name = "AU_Delay",
				Caption = "Время хода задвижки, сек",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Default = "180",
				Min = 1,
				Max = 65535,
				LargeValue = true
			};
			driver.Properties.Add(property);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8e, "Задержка включения, сек", "AU_Delay", 0, 0, 0, 250);//0
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8f, "Время удержания запуска, мин", "AU_LaunchDelay", 0, 0, 0, 360);//0

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Открыто»", 0, 
				"нормально-разомкнутый", 
				"нормально-замкнутый", 0, 0, 1);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Закрыто»", 1,
				"нормально-разомкнутый",
				"нормально-замкнутый", 0, 1, 2);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Открыто/ДУ Открыть", 2,
				"нормально-разомкнутый",
				"нормально-замкнутый", 0, 2, 3);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Закрыто/ДУ Закрыть", 3,
				"нормально-разомкнутый",
				"нормально-замкнутый", 0, 3, 4);
			
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Открыть УЗЗ", 4,
				"нормально-разомкнутый",
				"нормально-замкнутый", 0, 4, 5);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Закрыть УЗЗ", 5,
				"нормально-разомкнутый",
				"нормально-замкнутый", 0, 5, 6);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Стоп УЗЗ", 6,
				"нормально-разомкнутый",
				"нормально-замкнутый", 0, 6, 7);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовые выключатели", 1,
				"есть",
				"нет", 0, 1, 2, false, true);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "датчик уровня", 2, 
				"нет", 
				"есть", 0, 2, 3, false, true);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "функция УЗЗ", 3, 
				"отключена",
				"включена", 0, 3, 4, false, true);
		}
	}
}