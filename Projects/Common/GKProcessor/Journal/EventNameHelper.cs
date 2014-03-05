using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using XFiresecAPI;

namespace GKProcessor
{
	public static class EventNameHelper
	{
		public static XStateClass GetStateClassByName(string name)
		{
			var eventName = EventNames.FirstOrDefault(x => x.Name == name);
			if (eventName != null)
				return eventName.StateClass;
			return XStateClass.Norm;
		}

		static EventNameHelper()
		{
			EventNames = new List<EventName>();
			Add("Перевод в технологический режим", XStateClass.TechnologicalRegime, "Перевод в технологический режим для обновления ПО, чтения и записи конфигурации");
			Add("Синхронизация времени прибора с временем ПК", XStateClass.Info, "Синхронизация часов ГК и операционной системы");
			Add("Смена ПО", XStateClass.TechnologicalRegime, "Изменение программного обеспечения ГК и КАУ");
			Add("Смена БД", XStateClass.TechnologicalRegime, "Изменение базы данных ГК и КАУ");
			Add("Перевод в рабочий режим", XStateClass.TechnologicalRegime, "Перевод в рабочий режим");
			Add("Вход пользователя в прибор", XStateClass.Info, "Вход пользователя в ГК");
			Add("Выход пользователя из прибора", XStateClass.Info, "Выход пользователя из ГК");
			Add("Ошибка управления", XStateClass.Failure, "Ошибка управления исполнительным устройством");
			Add("Введен новый пользователь", XStateClass.Info, "Добавление нового пользователя ГК");
			Add("Изменена учетная информация пользователя", XStateClass.Info, "Изменение учетной информации пользователя ГК");
			Add("Произведена настройка сети", XStateClass.Info, "Изменение конфигурации сети, к которой подключён ГК");
			Add("Неизвестный тип", XStateClass.Unknown, "Подключено устройство неподдерживаемого типа");
			Add("Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown, "На шлейфе КАУ, обнаружено устройство с адресом, не описанным в конфигурации");
			Add("При конфигурации описан другой тип", XStateClass.Unknown, "По адресу обнаружено устройство с типом, не соответствующим типу, описанному в конфигурации");
			Add("Изменился заводской номер", XStateClass.Info, "По адресу обнаружено устройство с другим заводским номером");
			Add("Пожар-1", XStateClass.Fire1, "Событие инициируется зоной");
			Add("Сработка-1", XStateClass.Fire1, "Событие инициируется устройством");
			Add("Пожар-2", XStateClass.Fire2, "Событие инициируется зоной");
			Add("Сработка-2", XStateClass.Fire2, "Событие инициируется устройством");
			Add("Внимание", XStateClass.Attention, "Событие инициируется зоной");
			Add("Неисправность", XStateClass.Failure, "Событие инициируется устройством");
			Add("Неисправность устранена", XStateClass.Info, "Событие инициируется устройством");
			Add("Тест", XStateClass.Test, "Событие инициируется тест-кнопкой устройства");
			Add("Тест устранен", XStateClass.Info, "Событие инициируется тест-кнопкой устройства");
			Add("Запыленность", XStateClass.Service, "Запылённость датчика");
			Add("Запыленность устранена", XStateClass.Info, "Запылённость датчика");
			Add("Информация", XStateClass.Info, "Событие инициируется устройством");
			Add("Состояние", XStateClass.Info, "Изменение состояния объекта");

			Add("Открыто", XStateClass.Info, "");
			Add("Закрыто", XStateClass.Info, "");
			Add("Открывается", XStateClass.Info, "");
			Add("Закрывается", XStateClass.Info, "");

			Add("Отсчет задержки", XStateClass.Info, "");
			Add("Включено", XStateClass.On, "Изменение состояния на Включено");
			Add("Выключено", XStateClass.Off, "Изменение состояния на Выключено");
			Add("Включается", XStateClass.TurningOn, "Изменение состояния на Включается");
			Add("Выключается", XStateClass.TurningOff, "Изменение состояния на Выключается");
			Add("Кнопка", XStateClass.Info, "");
			Add("Изменение автоматики по неисправности", XStateClass.Info, "");
			Add("Изменение автоматики по кнопке СТОП", XStateClass.Info, "");
			Add("Изменение автоматики по датчику ДВЕРИ-ОКНА", XStateClass.Info, "");
			Add("Изменение автоматики по ТМ", XStateClass.Info, "");
			Add("Автоматика включена", XStateClass.Info, "");
			Add("Ручной пуск АУП от ИПР", XStateClass.Info, "");
			Add("Отложенный пуск АУП по датчику ДВЕРИ-ОКНА", XStateClass.Info, "");
			Add("Пуск АУП завершен", XStateClass.Info, "");
			Add("Останов тушения по кнопке СТОП", XStateClass.Info, "");
			Add("Программирование мастер-ключа", XStateClass.Info, "");
			Add("Отсчет удержания", XStateClass.Info, "");
			Add("Уровень высокий", XStateClass.Info, "");
			Add("Уровень низкий", XStateClass.Info, "");
			Add("Ход по команде с УЗЗ", XStateClass.Info, "");
			Add("У ДУ сообщение ПУСК НЕВОЗМОЖЕН", XStateClass.Info, "");
			Add("Авария пневмоемкости", XStateClass.Info, "");
			Add("Уровень аварийный", XStateClass.Info, "");
			Add("Запрет пуска НС", XStateClass.Info, "");
			Add("Запрет пуска компрессора", XStateClass.Info, "");
			Add("Команда с УЗН", XStateClass.Info, "");
			Add("Перевод в режим ручного управления", XStateClass.Info, "");
			Add("Состояние не определено", XStateClass.Info, "Изменение состояния на неопределенное");
			Add("Остановлено", XStateClass.Info, "Изменение состояния на Остановлено");

			Add("Режим работы", XStateClass.Info, "Изменение режима работы: ручной, автоматический, отключено");
			Add("Перевод в автоматический режим", XStateClass.Norm, "Перевод в автоматический режим");
			Add("Перевод в ручной режим", XStateClass.AutoOff, "Перевод в ручной режим");
			Add("Перевод в отключенный режим", XStateClass.Ignore, "Перевод в отключенный режим");
			Add("Перевод в неопределенный режим", XStateClass.Unknown, "Перевод в неопределенный режим");
			Add("Запись параметра", XStateClass.Info, "Запись параметров в объект");
			Add("Норма", XStateClass.Norm, "Переход в состояние Норма");
			Add("Вход пользователя в систему", XStateClass.Norm, "Вход пользователя в ОПС Firesec");
			Add("Выход пользователя из системы", XStateClass.Norm, "Выход пользователя из ОПС Firesec");
			Add("Команда оператора", XStateClass.Info, "Команда на сброс, управление ИУ, отключение, снятие отключения");
			foreach (EventNameEnum item in Enum.GetValues(typeof(EventNameEnum)))
			{
				Add(item.ToDescription(), EventNamesHelper.GetStateClass(item), "");
			}
		}

		static void Add(string name, XStateClass stateClass, string description)
		{
			var eventName = new EventName(name, stateClass, description);
			EventNames.Add(eventName);
		}

		public static List<EventName> EventNames { get; private set; }
	}
}