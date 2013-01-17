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
				Default = "180",
				Min = 1,
				Max = 999,
				LargeValue = true
			};
			driver.Properties.Add(property);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8e, "Задержка включения, сек", "AU_Delay", 0, 0, 0, 250);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8f, "Время удержания запуска, мин", "AU_LaunchDelay", 0, 0, 0, 360);

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

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "функция УЗЗ", 3, 
				"отключена",
				"включена", 0, 3, 4, false, true);

			var additionalSwitcherProperty = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x8d,
				Name = "Дополнительные выключатели",
				Caption = "Дополнительные выключатели",
				Default = "0",
				BitOffset = 0,
				MinBit = 1,
				MaxBit = 4,
				UseMask = false,
				HighByte = true,
			};
			var parameter1 = new DriverPropertyParameter()
			{
				Name = "Нет",
				Value = "0"
			};
			var parameter2 = new DriverPropertyParameter()
			{
				Name = "муфтовые выключатели",
				Value = "1"
			};
			var parameter3 = new DriverPropertyParameter()
			{
				Name = "датчик уровня",
				Value = "2"
			};
			additionalSwitcherProperty.Parameters.Add(parameter1);
			additionalSwitcherProperty.Parameters.Add(parameter2);
			additionalSwitcherProperty.Parameters.Add(parameter3);
			driver.Properties.Add(additionalSwitcherProperty);
		}
	}
}