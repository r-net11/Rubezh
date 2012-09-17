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

			ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Уставка времени хода задвижки", 0, 1, 1, 65535);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8e, "Время отложенного запуска, с", 0, 0, 0, 255);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8f, "Время удержания запуска, мин", 0, 0, 0, 360);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Открыто»", 0, 
				"0 концевой выключатель «Открыто» НР", 
				"1 концевой выключатель «Открыто» НЗ", 0, 0, 1);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Открыто/ДУ Открыть", 1, 
				"0 муфтовый выключатель Открыто/ДУ Открыть НР", 
				"1 муфтовый выключатель Открыто/ДУ Открыть НЗ", 0, 1, 2);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "концевой выключатель «Закрыто»", 2, 
				"0 концевой выключатель «Закрыто» НР", 
				"1 концевой выключатель «Закрыто» НЗ", 0, 2, 3);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовый выключатель Закрыто/ДУ Закрыть", 3, 
				"0 муфтовый выключатель Закрыто/ДУ Закрыть НР", 
				"1 датчик 4 НЗ", 0, 3, 4);
			
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Открыть УЗЗ", 4, 
				"0 кнопка Открыть УЗЗ НР", 
				"1 кнопка Открыть УЗЗ НЗ", 0, 4, 5);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Закрыть УЗЗ", 5, 
				"0 кнопка Закрыть УЗЗ НР", 
				"1 кнопка Закрыть УЗЗ НЗ", 0, 5, 6);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "кнопка Стоп УЗЗ", 6, 
				"0 кнопка Стоп УЗЗ НР", 
				"1 кнопка Стоп УЗЗ НЗ", 0, 6, 7);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "муфтовые выключатели", 1,
				"0 муфтовых выключателей нет",
				"1 муфтовые выключатели есть", 0, 1, 2, false, true);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "датчик уровня", 2, 
				"0 датчиков уровня нет", 
				"1 датчики уровня есть", 0, 2, 3, false, true);
			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "функция УЗЗ", 3, 
				"0 функция УЗЗ отключена",
				"1 функция УЗЗ включена", 0, 3, 4, false, true);
		}
	}
}