using System.Collections.Generic;
using XFiresecAPI;

namespace Commom.GK
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
			CommonHelper.AddAlternativePropertyParameter(property1, "Разомкнутые контакты", "замкнутые контакты в соответствии с задержками", 1);
			CommonHelper.AddAlternativePropertyParameter(property1, "Разомкнутые контакты", "режим «мерцания» с частотой 0,5Гц в соответствии с задержками", 2);
			CommonHelper.AddAlternativePropertyParameter(property1, "замкнутые контакты", "Разомкнутые контакты, в соответствии с задержками", 3);
			CommonHelper.AddAlternativePropertyParameter(property1, "замкнутые контакты", "режим «мерцания» с частотой 0,5Гц в соответствии с задержками", 4);
			CommonHelper.AddAlternativePropertyParameter(property1, "режим «мерцания» с частотой 0,5Гц", "Разомкнутые контакты, в соответствии с задержками", 5);
			CommonHelper.AddAlternativePropertyParameter(property1, "режим «мерцания» с частотой 0,5Гц", "замкнутые контакты в соответствии с задержками" , 6);
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