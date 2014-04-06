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
			SKDEvents.Add(new SKDEvent(1,  "Проход", XStateClass.Norm, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(2, "Проход с нарушением ВРЕМЕНИ", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(3, "Проход с нарушением ЗОНАЛЬНОСТИ", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(4, "Проход с нарушением ВРЕМЕНИ и ЗОНАЛЬНОСТИ", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(5, "Проход разрешен", XStateClass.Norm, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(6, "Нарушение ВРЕМЕНИ", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(7, "Нарушение ЗОНАЛЬНОСТИ", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(8, "Нарушение ВРЕМЕНИ и ЗОНАЛЬНОСТИ", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(9, "Идентификатор НЕ ЗАРЕГИСТРИРОВАН", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(10, "Идентификатор ЗАБЛОКИРОВАН", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(11, "Идентификатор ИЗ СТОП-ЛИСТА", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(12, "Идентификатор ПРОСРОЧЕН", XStateClass.Info, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(13, "Нарушение режима доступа", XStateClass.Failure, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(14, "Взлом ИУ", XStateClass.Failure, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(15, "Проход от ДУ", XStateClass.Norm, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(16, "Запрс прохода от ДУ", XStateClass.Norm, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(17, "Ожидание комиссионирования прохода", XStateClass.Norm, SKDDriverType.Reader));

			SKDEvents.Add(new SKDEvent(18, "Неисправность", XStateClass.Failure, SKDDriverType.Controller));
			SKDEvents.Add(new SKDEvent(19, "Неисправность устранена", XStateClass.Norm, SKDDriverType.Controller));

			SKDEvents.Add(new SKDEvent(20, "Неисправность", XStateClass.Failure, SKDDriverType.Reader));
			SKDEvents.Add(new SKDEvent(21, "Неисправность устранена", XStateClass.Norm, SKDDriverType.Reader));

			SKDEvents.Add(new SKDEvent(22, "Команда управления", XStateClass.Info, SKDDriverType.System));
			SKDEvents.Add(new SKDEvent(23, "Конфигурирование", XStateClass.Info, SKDDriverType.System));
		}
	}

	public class SKDEvent
	{
		public SKDEvent(int no, string name, XStateClass stateClass, SKDDriverType driverType)
		{
			No = no;
			Name = name;
			StateClass = stateClass;
			DriverType = driverType;
		}

		public int No { get; set; }
		public string Name { get; set; }
		public XStateClass StateClass { get; set; }
		public SKDDriverType DriverType { get; set; }
	}
}