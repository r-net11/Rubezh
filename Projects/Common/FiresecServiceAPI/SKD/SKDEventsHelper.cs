using System.Collections.Generic;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class SKDEventsHelper
	{
		public static List<SKDEvent> SKDEvents { get; private set; }

		static SKDEventsHelper()
		{
			SKDEvents = new List<SKDEvent>();
			SKDEvents.Add(new SKDEvent(1, EventNameEnum.Проход, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(2, EventNameEnum.Проход_с_нарушением_ВРЕМЕНИ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(3, EventNameEnum.Проход_с_нарушением_ЗОНАЛЬНОСТИ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(4, EventNameEnum.Проход_с_нарушением_ВРЕМЕНИ_и_ЗОНАЛЬНОСТИ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(5, EventNameEnum.Проход_разрешен, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(6, EventNameEnum.Нарушение_ВРЕМЕНИ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(7, EventNameEnum.Нарушение_ЗОНАЛЬНОСТИ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(8, EventNameEnum.Нарушение_ВРЕМЕНИ_и_ЗОНАЛЬНОСТИ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(9, EventNameEnum.Идентификатор_НЕ_ЗАРЕГИСТРИРОВАН, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(10, EventNameEnum.Идентификатор_ЗАБЛОКИРОВАН, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(11, EventNameEnum.Деактивированный_идентификатор, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(12, EventNameEnum.Идентификатор_ПРОСРОЧЕН, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(13, EventNameEnum.Нарушение_режима_доступа, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(14, EventNameEnum.Взлом_ИУ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(15, EventNameEnum.Проход_от_ДУ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(16, EventNameEnum.Запрос_прохода_от_ДУ, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(17, EventNameEnum.Ожидание_комиссионирования_прохода, SKDDriverType.Reader));

			SKDEvents.Add(new SKDEvent(18,  EventNameEnum.Неисправность, SKDDriverType.Controller));
			SKDEvents.Add(new SKDEvent(19, EventNameEnum.Неисправность_устранена, SKDDriverType.Controller));

			SKDEvents.Add(new SKDEvent(20, EventNameEnum.Неисправность, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(21, EventNameEnum.Неисправность_устранена, SKDDriverType.Reader));

			SKDEvents.Add(new SKDEvent(22, EventNameEnum.Команда_управления, SKDDriverType.System));
			SKDEvents.Add(new SKDEvent(23, EventNameEnum.Конфигурирование, SKDDriverType.System));
		}
	}

	public class SKDEvent
	{
		public SKDEvent(int no, EventNameEnum name, SKDDriverType driverType)
		{
			No = no;
			Name = name;
			StateClass = EventNamesHelper.GetStateClass(name);
			DriverType = driverType;
		}

		public int No { get; set; }
		public EventNameEnum Name { get; set; }
		public XStateClass StateClass { get; set; }
		public SKDDriverType DriverType { get; set; }
	}
}