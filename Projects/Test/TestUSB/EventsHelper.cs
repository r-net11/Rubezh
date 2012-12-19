using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUSB
{
	public static class EventsHelper
	{
		static List<EventItem> EventItems = new List<EventItem>();

		static EventsHelper()
		{
			EventItems.Add(new EventItem(0x01, "[Тревога от обойдённого ИП/Пожарная тревога/Дым ниже порога]"));
			EventItems.Add(new EventItem(0x02, "[Тест кнопка/Тест лазер]"));
			EventItems.Add(new EventItem(0x04, "[Выключение/Включение/Отмена задержки автозапуска] устройства оператором"));
			EventItems.Add(new EventItem(0x05, "Исполнительное устройство [выключено/включено]"));
			EventItems.Add(new EventItem(0x07, "Неисправность прибора [устранена/]"));
			EventItems.Add(new EventItem(0x08, "Обнаружено отсутствующее в базе устройство"));
			EventItems.Add(new EventItem(0x0A, "Команда на смену ПО"));
			EventItems.Add(new EventItem(0x09, "Связь с устройством [восстановлена/потеряна]"));
			EventItems.Add(new EventItem(0x0B, "Включение питания"));
			EventItems.Add(new EventItem(0x0C, "[Корпус закрыт/Вскрытие]"));
			EventItems.Add(new EventItem(0x0D, "[Системная неисправность сброшена/Системная неисправность]"));
			EventItems.Add(new EventItem(0x0E, "[Внимание от обойденного ИП/Внимание]"));
			EventItems.Add(new EventItem(0x0F, "[АЛС в рабочем состоянии/КЗ АЛС/Обрыв кольцевой АЛС/Перегрузка АЛС/Обрыв кольцевой АЛС устранен]"));
			EventItems.Add(new EventItem(0x10, "Реконфигурация базы"));
			EventItems.Add(new EventItem(0x23, "[Устранение неисправности/Неисправность] устройства"));
			EventItems.Add(new EventItem(0x25, "Предварительный уровень запыленности датчика [устранен/достигнут]"));
			EventItems.Add(new EventItem(0x26, "Приоритет теплового канала [отключен/установлен]"));
			EventItems.Add(new EventItem(0x27, "Критический уровень запыленности датчика [устранен/достигнут]"));
			EventItems.Add(new EventItem(0x28, "Получена команда управления устройством"));
			EventItems.Add(new EventItem(0x30, "Тревога [обойденное/ /тихая/предварительная (активация задержки на вход)]"));
			EventItems.Add(new EventItem(0x31, "[Зона поставлена на охрану/Зона снята с охраны/неудачная постановка зоны/постановка зоны на охрану(задержка)/Снятие зоны невозможно]"));
			EventItems.Add(new EventItem(0x32, "Сброс выполнен"));
			EventItems.Add(new EventItem(0x33, "Активирована задержка на вход"));
			EventItems.Add(new EventItem(0x34, "Обход устройства [снят/]"));
			EventItems.Add(new EventItem(0x36, "Тест системы"));
			EventItems.Add(new EventItem(0x35, "Сообщения насосов"));
			EventItems.Add(new EventItem(0x37, "Сообщения НС"));
			EventItems.Add(new EventItem(0x38, "Сообщение ШУЗ"));
			EventItems.Add(new EventItem(0x39, "[Сообщения МПТ/Ручной запуск/Ручной останов/Тушение/Пуск блокирован (автоматика отключена)/Автоматика включена/Автоматика отключена/Отложенный запуск/Запуск возобновлён/Нарушение датчика 'двери-окна'-отложенный запуск/Восстановление датчика 'двери-окна'-запуск возобновлён/Срабатывание датчика давления/Восстановление датчика давления/Срабатывание датчика массы/Восстановление датчика массы/Невозможно включить автоматику по команде/Открыта дверь-запуск невозможен/Задержка автозапуска/Вскрытие корпуса/Корпус закрыт]"));
			EventItems.Add(new EventItem(0x3A, "[Состояние 1/Состояние 2]"));
			EventItems.Add(new EventItem(0x3b, "[Обход зоны снят/Обход зоны]"));
			EventItems.Add(new EventItem(0x3c, "Нажатие кнопки"));
			EventItems.Add(new EventItem(0x3d, "[Давление в норме/Давление ниже нормы/Давление выше нормы]"));
			EventItems.Add(new EventItem(0x3f, "[Тест зоны снят/Тест зоны]"));
			EventItems.Add(new EventItem(0x3e, "[Имитация выключения ИУ/Имитация включения ИУ]"));
			EventItems.Add(new EventItem(0x40, "[Неисправность АСПТ/Автоматический режим АСПТ/Ручной режим АСПТ/Отключение АСПТ]"));
			EventItems.Add(new EventItem(0x41, "[Останов АСПТ/Запуск АСПТ]"));
			EventItems.Add(new EventItem(0x42, "[Попытка запуска АСПТ/Ручной запуск АСПТ]"));
			EventItems.Add(new EventItem(0x43, "[/Вращение к положению 'ОТКРЫТО'/Вращение к положению 'ЗАКРЫТО'/Задержанный пуск/Заслонка открыта/Заслонка закрыта/Разрешение команды ППКП 'НОРМА'/Запрет команды ППКП 'НОРМА']"));
			EventItems.Add(new EventItem(0x44, "[Задержка на включение/Пуск]"));
			EventItems.Add(new EventItem(0x45, "Сброс выполнен"));
			EventItems.Add(new EventItem(0x46, "Неудачная постановка"));
			EventItems.Add(new EventItem(0x47, "Неверный [/ключ/пароль/внешний ключ]"));
			EventItems.Add(new EventItem(0x48, "[Тестовый режим отключен/Тестовый режим включен]"));
			EventItems.Add(new EventItem(0x4a, "[Включение вывода/Отключение вывода]"));
			EventItems.Add(new EventItem(0x50, "Ошибка доставки сообщения УОО-ТЛ [устранена/]"));
			EventItems.Add(new EventItem(0x51, "Переполнение внутреннего буфера УОО-ТЛ"));
			EventItems.Add(new EventItem(0x52, "Неисправность телефонной линии УОО-ТЛ [устранена/]"));
			EventItems.Add(new EventItem(0x80, "[Переход на основное питание/Переход на резервное питание/Переход на основное питание-2/Переход на резервное питание-2]"));
			EventItems.Add(new EventItem(0x81, "[/Элементу питания требуется замена]"));
			EventItems.Add(new EventItem(0x83, "[Неисправность выхода устранена/Обрыв выхода/Перегрузка выхода]"));
			EventItems.Add(new EventItem(0x84, "Сбой обмена"));
			EventItems.Add(new EventItem(0x85, "[Восстановление/Потеря] связи с мониторинговой станцией"));
		}

		public static string Get(int code)
		{
			var eventName = EventItems.FirstOrDefault(x => x.Code == code);
			if (eventName == null)
				return "Неизвестный код события " + code.ToString("x2");
			return eventName.Name;
		}
	}

	public class EventItem
	{
		public EventItem(int code, string name)
		{
			Code = code;
			Name = name;
		}

		public int Code { get; set; }
		public string Name { get; set; }
	}
}