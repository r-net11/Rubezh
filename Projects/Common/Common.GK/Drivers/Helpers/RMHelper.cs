using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
	public static class RMHelper
	{
		public static void Create(XDriver xDriver)
		{
			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Конфигурация релейного модуля",
				Caption = "Конфигурация релейного модуля",
				Default = 1,
				AlternativePareterNames = new List<string>() { "Стоп", "Пуск" }
			};
			CommonHelper.AddPropertyParameter(property1, "Разомкнуто Замкнуто", 1);
			CommonHelper.AddPropertyParameter(property1, "Разомкнуто Мерцает", 2);
			CommonHelper.AddPropertyParameter(property1, "Замкнуто Разомкнуто", 3);
			CommonHelper.AddPropertyParameter(property1, "Замкнуто Мерцает", 4);
			CommonHelper.AddPropertyParameter(property1, "Мерцает Разомкнуто", 5);
			CommonHelper.AddPropertyParameter(property1, "Мерцает Замкнуто", 6);
			xDriver.Properties.Add(property1);

			CommonHelper.AddIntProprety(xDriver, 0x83, "Задержка на пуск", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0x83, "Время удержания",  8, 0, 0, 255);

			var property2 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип контроля выхода",
				Caption = "Тип контроля выхода",
				Default = 1
			};
			CommonHelper.AddPropertyParameter(property2, "Состояние цепи не контролируется", 1);
			CommonHelper.AddPropertyParameter(property2, "Цепь контролируется только на обрыв", 2);
			CommonHelper.AddPropertyParameter(property2, "Цепь контролируется только на короткое замыкание", 3);
			CommonHelper.AddPropertyParameter(property2, "Цепь контролируется на короткое замыкание и на обрыв", 4);
			xDriver.Properties.Add(property2);
		}
	}
}