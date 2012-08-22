using System.Collections.Generic;

namespace FiresecAPI.Models
{
	public static class RMHelper
	{
		public static void Create(Driver driver)
		{
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x82,
				Name = "Конфигурация релейного модуля",
				Caption = "Конфигурация релейного модуля",
				Default = "1",
				AlternativePareterNames = new List<string>() { "Стоп", "Пуск" }
			};
			ConfigurationDriverHelper.AddAlternativePropertyParameter(property1, "Разомкнутые контакты", "замкнутые контакты в соответствии с задержками", 1);
			ConfigurationDriverHelper.AddAlternativePropertyParameter(property1, "Разомкнутые контакты", "режим «мерцания» с частотой 0,5Гц в соответствии с задержками", 2);
			ConfigurationDriverHelper.AddAlternativePropertyParameter(property1, "замкнутые контакты", "Разомкнутые контакты, в соответствии с задержками", 3);
			ConfigurationDriverHelper.AddAlternativePropertyParameter(property1, "замкнутые контакты", "режим «мерцания» с частотой 0,5Гц в соответствии с задержками", 4);
			ConfigurationDriverHelper.AddAlternativePropertyParameter(property1, "режим «мерцания» с частотой 0,5Гц", "Разомкнутые контакты, в соответствии с задержками", 5);
			ConfigurationDriverHelper.AddAlternativePropertyParameter(property1, "режим «мерцания» с частотой 0,5Гц", "замкнутые контакты в соответствии с задержками", 6);
			driver.Properties.Add(property1);

			ConfigurationDriverHelper.AddIntProprety(driver, 0x83, "Задержка на пуск", -8, 0, 0, 250);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x83, "Время удержания",  8, 0, 0, 250);
		}
	}
}