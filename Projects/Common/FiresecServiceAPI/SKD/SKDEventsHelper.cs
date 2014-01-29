using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI
{
	public static class SKDEventsHelper
	{
		public static List<SKDEvent> SKDEvents { get; private set; }

		static SKDEventsHelper()
		{
			SKDEvents = new List<SKDEvent>();
			SKDEvents.Add(new SKDEvent(1,  "Проход"));
			SKDEvents.Add(new SKDEvent(2,  "Проход с нарушением ВРЕМЕНИ"));
			SKDEvents.Add(new SKDEvent(3,  "Проход с нарушением ЗОНАЛЬНОСТИ"));
			SKDEvents.Add(new SKDEvent(4,  "Проход с нарушением ВРЕМЕНИ и ЗОНАЛЬНОСТИ"));
			SKDEvents.Add(new SKDEvent(5,  "Постановка на охрану"));
			SKDEvents.Add(new SKDEvent(6,  "Снятие с охраны"));
			SKDEvents.Add(new SKDEvent(7,  "Проход разрешен"));
			SKDEvents.Add(new SKDEvent(8,  "Нарушение ВРЕМЕНИ"));
			SKDEvents.Add(new SKDEvent(9,  "Нарушение ЗОНАЛЬНОСТИ"));
			SKDEvents.Add(new SKDEvent(10, "Нарушение ВРЕМЕНИ и ЗОНАЛЬНОСТИ"));
			SKDEvents.Add(new SKDEvent(11, "Идентификатор НЕ ЗАРЕГИСТРИРОВАН"));
			SKDEvents.Add(new SKDEvent(12, "Идентификатор ЗАБЛОКИРОВАН"));
			SKDEvents.Add(new SKDEvent(13, "Идентификатор ИЗ СТОП-ЛИСТА"));
			SKDEvents.Add(new SKDEvent(14, "Идентификатор ПРОСРОЧЕН"));
			SKDEvents.Add(new SKDEvent(15, "Нарушение режима доступа"));
			SKDEvents.Add(new SKDEvent(16, "Взлом ИУ"));
			SKDEvents.Add(new SKDEvent(17, "Проход от ДУ"));
			SKDEvents.Add(new SKDEvent(18, "Запрс прохода от ДУ"));
			SKDEvents.Add(new SKDEvent(19, "Ожидание комиссионирования прохода"));
		}
	}

	public class SKDEvent
	{
		public SKDEvent(int no, string name)
		{
			No = no;
			Name = name;
		}

		public int No { get; set; }
		public string Name { get; set; }
	}
}