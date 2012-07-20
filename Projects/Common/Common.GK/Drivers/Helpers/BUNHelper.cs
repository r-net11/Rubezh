using XFiresecAPI;

namespace Commom.GK
{
	public static class BUNHelper
	{
		public static void Create(XDriver xDriver)
		{
			CommonHelper.AddIntProprety(xDriver, 0x8b, "Максимальное время перезапуска, 0.1 с", 0, 1, 1, 255);
			CommonHelper.AddIntProprety(xDriver, 0x8b, "Время разновременного пуска, с", 0, 1, 1, 255);

			if (xDriver.DriverType != XDriverType.Pump)
			{
				CommonHelper.AddPlainEnumProprety(xDriver, 0x8d, "Логика", 0 + 8, "на входах 1 и 2 два обычных датчика", "на входах 1 и 2 два обычных датчика");
			}

			var property3 = new XDriverProperty()
			{
				No = 0x8b,
				Name = "разрешение функции УЗН",
				Caption = "разрешение функции УЗН",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 3 + 8
			};
			xDriver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x8b,
				Name = "наличие в прошивке логики работы с УЗН",
				Caption = "наличие в прошивке логики работы с УЗН",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 7 + 8
			};
			xDriver.Properties.Add(property4);

			if (xDriver.DriverType == XDriverType.Pump)
			{
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "ЭКМ на выходе насоса", 0);
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "УЗН Старт", 1);
			}
			else
			{
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "ДД/ДУ Пуск", 0);
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "ДД/ДУ Стоп", 1);
			}

			if (xDriver.DriverType == XDriverType.Pump)
			{
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "УЗН Стоп", 2);
			}
			if (xDriver.DriverType == XDriverType.DrenazhPump)
			{
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "Авария", 2);
			}
			if (xDriver.DriverType == XDriverType.CompensationPump)
			{
				CommonHelper.AddBoolProprety(xDriver, 0x8b, "Авария", 2);
			}

			if (xDriver.DriverType == XDriverType.Pump)
			{
				CommonHelper.AddIntProprety(xDriver, 0x84, "уставка времени ожидания выхода насоса на режим, с", 0, 3, 3, 30);
			}
			if (xDriver.DriverType == XDriverType.JokeyPump)
			{
				CommonHelper.AddIntProprety(xDriver, 0x84, "уставка времени ожидания восстановления давления, мин", 0, 2, 2, 65535);
			}
			if (xDriver.DriverType == XDriverType.CompensationPump)
			{
				CommonHelper.AddIntProprety(xDriver, 0x84, "уставка времени аварии пневмоемкости, мин", 0, 2, 2, 65535);
			}
		}
	}
}