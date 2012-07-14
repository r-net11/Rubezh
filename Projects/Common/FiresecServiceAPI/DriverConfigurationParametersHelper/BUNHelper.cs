
namespace FiresecAPI.Models
{
	public static class BUNHelper
	{
		public static void Create(Driver driver)
		{
			driver.HasConfigurationProperties = true;

			ConfigurationDriverHelper.AddIntProprety(driver, 0x8b, "Максимальное время перезапуска, 0.1 с", 0, 1, 1, 255);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8b, "Время разновременного пуска, с", 0, 1, 1, 255);

			if (driver.DriverType != DriverType.Pump)
			{
				ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x8d, "Логика", 0 + 8, "на входах 1 и 2 два обычных датчика", "на входах 1 и 2 два обычных датчика");
			}

			var property3 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x8b,
				Name = "разрешение функции УЗН",
				Caption = "разрешение функции УЗН",
				DriverPropertyType = DriverPropertyTypeEnum.BoolType,
				Offset = 3 + 8
			};
			driver.Properties.Add(property3);

			var property4 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x8b,
				Name = "наличие в прошивке логики работы с УЗН",
				Caption = "наличие в прошивке логики работы с УЗН",
				DriverPropertyType = DriverPropertyTypeEnum.BoolType,
				Offset = 7 + 8
			};
			driver.Properties.Add(property4);

			if (driver.DriverType == DriverType.Pump)
			{
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "ЭКМ на выходе насоса", 0);
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "УЗН Старт", 1);
			}
			else
			{
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "ДД/ДУ Пуск", 0);
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "ДД/ДУ Стоп", 1);
			}

			if (driver.DriverType == DriverType.Pump)
			{
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "УЗН Стоп", 2);
			}
			if (driver.DriverType == DriverType.DrenazhPump)
			{
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "Авария", 2);
			}
			if (driver.DriverType == DriverType.CompensationPump)
			{
				ConfigurationDriverHelper.AddBoolProprety(driver, 0x8b, "Авария", 2);
			}

			if (driver.DriverType == DriverType.Pump)
			{
				ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "уставка времени ожидания выхода насоса на режим, с", 0, 3, 3, 30);
			}
			if (driver.DriverType == DriverType.JokeyPump)
			{
				ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "уставка времени ожидания восстановления давления, мин", 0, 2, 2, 65535);
			}
			if (driver.DriverType == DriverType.CompensationPump)
			{
				ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "уставка времени аварии пневмоемкости, мин", 0, 2, 2, 65535);
			}
		}
	}
}