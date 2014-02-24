using System.Collections.Generic;
using XFiresecAPI;

namespace FiresecAPI
{
	public static class SKDEventsHelper
	{
		public static List<SKDEvent> SKDEvents { get; private set; }

		static SKDEventsHelper()
		{
			SKDEvents = new List<SKDEvent>();
			SKDEvents.Add(new SKDEvent(1,  "Проход", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(2, "Проход с нарушением ВРЕМЕНИ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(3, "Проход с нарушением ЗОНАЛЬНОСТИ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(4, "Проход с нарушением ВРЕМЕНИ и ЗОНАЛЬНОСТИ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(5, "Проход разрешен", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(6, "Нарушение ВРЕМЕНИ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(7, "Нарушение ЗОНАЛЬНОСТИ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(8, "Нарушение ВРЕМЕНИ и ЗОНАЛЬНОСТИ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(9, "Идентификатор НЕ ЗАРЕГИСТРИРОВАН", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(10, "Идентификатор ЗАБЛОКИРОВАН", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(11, "Идентификатор ИЗ СТОП-ЛИСТА", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(12, "Идентификатор ПРОСРОЧЕН", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(13, "Нарушение режима доступа", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(14, "Взлом ИУ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(15, "Проход от ДУ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(16, "Запрс прохода от ДУ", SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(17, "Ожидание комиссионирования прохода", SKDDriverType.Reader));

			SKDEvents.Add(new SKDEvent(18, "Неисправность", SKDDriverType.Controller));
			SKDEvents.Add(new SKDEvent(19, "Неисправность устранена", SKDDriverType.Controller));
		}
	}

	public class SKDEvent
	{
		public SKDEvent(int no, string name, SKDDriverType driverType)
		{
			No = no;
			Name = name;
			DriverType = driverType;
		}

		public int No { get; set; }
		public string Name { get; set; }
		public SKDDriverType DriverType { get; set; }
	}
}