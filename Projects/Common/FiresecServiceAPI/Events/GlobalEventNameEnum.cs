using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FiresecAPI.GK;

namespace FiresecAPI.Events
{
	public enum GlobalEventNameEnum
	{
		[DescriptionAttribute("")]
		NULL,

		[EventDescription(GlobalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие,

		[EventDescription(GlobalSubsystemType.System, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги,

		[EventDescription(GlobalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему,

		[EventDescription(GlobalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы,

		[EventDescription(GlobalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал,

		[EventDescription(GlobalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял,

		[EventDescription(GlobalSubsystemType.System, "Зависание процесса отпроса", XStateClass.Unknown)]
		Зависание_процесса_отпроса,

		[EventDescription(GlobalSubsystemType.System, "Отсутствует лицензия", XStateClass.HasNoLicense)]
		Отсутствует_лицензия,

		[EventDescription(GlobalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasNoLicense)]
		Лицензия_обнаружена,

		[EventDescription(GlobalSubsystemType.System, "Ошибка инициализации мониторинга", XStateClass.Unknown)]
		Ошибка_инициализации_мониторинга,

		[EventDescription(GlobalSubsystemType.GK, "Обновление ПО прибора", XStateClass.Info)]
		Обновление_ПО_прибора,

		[EventDescription(GlobalSubsystemType.GK, "Запись конфигурации в прибор", XStateClass.Info)]
		Запись_конфигурации_в_прибор,

		[EventDescription(GlobalSubsystemType.GK, "Чтение конфигурации из прибора", XStateClass.Info)]
		Чтение_конфигурации_из_прибора,

		[EventDescription(GlobalSubsystemType.GK, "Запрос информации об устройстве", XStateClass.Info)]
		Запрос_информации_об_устройстве,

		[EventDescription(GlobalSubsystemType.GK, "Синхронизация времени", XStateClass.Info)]
		Синхронизация_времени,

		[EventDescription(GlobalSubsystemType.GK, "Команда оператора", XStateClass.Info)]
		Команда_оператора,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при выполнении команды", XStateClass.Failure)]
		Ошибка_при_выполнении_команды,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при выполнении команды над устройством", XStateClass.Failure)]
		Ошибка_при_выполнении_команды_над_устройством,

		[EventDescription(GlobalSubsystemType.GK, "Нет связи с ГК", XStateClass.ConnectionLost)]
		Нет_связи_с_ГК,

		[EventDescription(GlobalSubsystemType.GK, "Связь с ГК восстановлена", XStateClass.ConnectionLost)]
		Связь_с_ГК_восстановлена,

		[EventDescription(GlobalSubsystemType.GK, "Конфигурация прибора не соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_не_соответствует_конфигурации_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Конфигурация прибора соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_соответствует_конфигурации_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при синхронизации журнала", XStateClass.Unknown)]
		Ошибка_при_синхронизации_журнала,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Ошибка_при_опросе_состояний_компонентов_ГК,

		[EventDescription(GlobalSubsystemType.GK, "Устранена ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Устранена_ошибка_при_опросе_состояний_компонентов_ГК,

		[EventDescription(GlobalSubsystemType.GK, "Восстановление связи с прибором", XStateClass.ConnectionLost)]
		Восстановление_связи_с_прибором,

		[EventDescription(GlobalSubsystemType.GK, "Потеря связи с прибором", XStateClass.ConnectionLost)]
		Потеря_связи_с_прибором,

		[EventDescription(GlobalSubsystemType.System, "База данных прибора не соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_не_соответствует_базе_данных_ПК,

		[EventDescription(GlobalSubsystemType.GK, "База данных прибора соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_соответствует_базе_данных_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации,

		[EventDescription(GlobalSubsystemType.GK, "Отмена операции", XStateClass.Info)]
		Отмена_операции,

		[EventDescription(GlobalSubsystemType.GK, "ГК в технологическом режиме", XStateClass.TechnologicalRegime)]
		ГК_в_технологическом_режиме,

		[EventDescription(GlobalSubsystemType.GK, "ГК в рабочем режиме", XStateClass.Info)]
		ГК_в_рабочем_режиме,

		[EventDescription(GlobalSubsystemType.GK, "Запись всех идентификаторов", XStateClass.Info)]
		Запись_всех_идентификаторов,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в технологический режим", XStateClass.TechnologicalRegime)]
		Перевод_в_технологический_режим,

		[EventDescription(GlobalSubsystemType.GK, "Синхронизация времени прибора с временем ПК", XStateClass.Info)]
		Синхронизация_времени_прибора_с_временем_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Смена ПО", XStateClass.TechnologicalRegime)]
		Смена_ПО,

		[EventDescription(GlobalSubsystemType.GK, "Смена БД", XStateClass.TechnologicalRegime)]
		Смена_БД,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в рабочий режим", XStateClass.Info)]
		Перевод_в_рабочий_режим,

		[EventDescription(GlobalSubsystemType.GK, "Вход пользователя в прибор", XStateClass.Info)]
		Вход_пользователя_в_прибор,

		[EventDescription(GlobalSubsystemType.GK, "Выход пользователя из прибора", XStateClass.Info)]
		Выход_пользователя_из_прибора,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка управления", XStateClass.Failure)]
		Ошибка_управления,

		[EventDescription(GlobalSubsystemType.GK, "Введен новый пользователь", XStateClass.Info)]
		Введен_новый_пользователь,

		[EventDescription(GlobalSubsystemType.GK, "Изменена учетная информация пользователя", XStateClass.Info)]
		Изменена_учетная_информация_пользователя,

		[EventDescription(GlobalSubsystemType.GK, "Произведена настройка сети", XStateClass.Info)]
		Произведена_настройка_сети,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный код события контроллекра", XStateClass.Unknown)]
		Неизвестный_код_события_контроллекра,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный тип", XStateClass.Unknown)]
		Неизвестный_тип,

		[EventDescription(GlobalSubsystemType.GK, "Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown)]
		Устройство_с_таким_адресом_не_описано_при_конфигурации,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный код события устройства", XStateClass.Unknown)]
		Неизвестный_код_события_устройства,

		[EventDescription(GlobalSubsystemType.GK, "При конфигурации описан другой тип", XStateClass.Unknown)]
		При_конфигурации_описан_другой_тип,

		[EventDescription(GlobalSubsystemType.GK, "Изменился заводской номер", XStateClass.Info)]
		Изменился_заводской_номер,

		[EventDescription(GlobalSubsystemType.GK, "Пожар-1", XStateClass.Fire1)]
		Пожар_1,

		[EventDescription(GlobalSubsystemType.GK, "Сработка-1", XStateClass.Fire1)]
		Сработка_1,

		[EventDescription(GlobalSubsystemType.GK, "Пожар-2", XStateClass.Fire2)]
		Пожар_2,

		[EventDescription(GlobalSubsystemType.GK, "Сработка-2", XStateClass.Fire2)]
		Сработка_2,

		[EventDescription(GlobalSubsystemType.GK, "Внимание", XStateClass.Attention)]
		Внимание,

		[EventDescription(GlobalSubsystemType.GK, "Неисправность", XStateClass.Failure)]
		Неисправность,

		[EventDescription(GlobalSubsystemType.GK, "Неисправность устранена", XStateClass.Norm)]
		Неисправность_устранена,

		[EventDescription(GlobalSubsystemType.GK, "Тест", XStateClass.Test)]
		Тест,

		[EventDescription(GlobalSubsystemType.GK, "Тест устранен", XStateClass.Test)]
		Тест_устранен,

		[EventDescription(GlobalSubsystemType.GK, "Запыленность", XStateClass.Service)]
		Запыленность,

		[EventDescription(GlobalSubsystemType.GK, "Запыленность устранена", XStateClass.Service)]
		Запыленность_устранена,

		[EventDescription(GlobalSubsystemType.GK, "Информация", XStateClass.Info)]
		Информация,

		[EventDescription(GlobalSubsystemType.GK, "Отсчет задержки", XStateClass.Info)]
		Отсчет_задержки,

		[EventDescription(GlobalSubsystemType.GK, "Включено", XStateClass.On)]
		Включено,

		[EventDescription(GlobalSubsystemType.GK, "Выключено", XStateClass.Off)]
		Выключено,

		[EventDescription(GlobalSubsystemType.GK, "Включается", XStateClass.TurningOn)]
		Включается,

		[EventDescription(GlobalSubsystemType.GK, "Выключается", XStateClass.TurningOff)]
		Выключается,

		[EventDescription(GlobalSubsystemType.GK, "Кнопка", XStateClass.Info)]
		Кнопка,

		[EventDescription(GlobalSubsystemType.GK, "Изменение автоматики по неисправности", XStateClass.AutoOff)]
		Изменение_автоматики_по_неисправности,

		[EventDescription(GlobalSubsystemType.GK, "Изменение автоматики по кнопке СТОП", XStateClass.AutoOff)]
		Изменение_автоматики_по_кнопке_СТОП,

		[EventDescription(GlobalSubsystemType.GK, "Изменение автоматики по датчику ДВЕРИ-ОКНА", XStateClass.AutoOff)]
		Изменение_автоматики_по_датчику_ДВЕРИ_ОКНА,

		[EventDescription(GlobalSubsystemType.GK, "Изменение автоматики по ТМ", XStateClass.AutoOff)]
		Изменение_автоматики_по_ТМ,

		[EventDescription(GlobalSubsystemType.GK, "Автоматика включена", XStateClass.AutoOff)]
		Автоматика_включена,

		[EventDescription(GlobalSubsystemType.GK, "Ручной пуск АУП от ИПР", XStateClass.On)]
		Ручной_пуск_АУП_от_ИПР,

		[EventDescription(GlobalSubsystemType.GK, "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА", XStateClass.On)]
		Отложенный_пуск_АУП_по_датчику_ДВЕРИ_ОКНА,

		[EventDescription(GlobalSubsystemType.GK, "Пуск АУП завершен", XStateClass.On)]
		Пуск_АУП_завершен,

		[EventDescription(GlobalSubsystemType.GK, "Останов тушения по кнопке СТОП", XStateClass.Off)]
		Останов_тушения_по_кнопке_СТОП,

		[EventDescription(GlobalSubsystemType.GK, "Программирование мастер-ключа", XStateClass.Info)]
		Программирование_мастер_ключа,

		[EventDescription(GlobalSubsystemType.GK, "Отсчет удержания", XStateClass.Info)]
		Отсчет_удержания,

		[EventDescription(GlobalSubsystemType.GK, "Уровень высокий", XStateClass.Info)]
		Уровень_высокий,

		[EventDescription(GlobalSubsystemType.GK, "Уровень низкий", XStateClass.Info)]
		Уровень_низкий,

		[EventDescription(GlobalSubsystemType.GK, "Ход по команде с УЗЗ", XStateClass.On)]
		Ход_по_команде_с_УЗЗ,

		[EventDescription(GlobalSubsystemType.GK, "У ДУ сообщение ПУСК НЕВОЗМОЖЕН", XStateClass.Failure)]
		У_ДУ_сообщение_ПУСК_НЕВОЗМОЖЕН,

		[EventDescription(GlobalSubsystemType.GK, "Авария пневмоемкости", XStateClass.Failure)]
		Авария_пневмоемкости,

		[EventDescription(GlobalSubsystemType.GK, "Уровень аварийный", XStateClass.Failure)]
		Уровень_аварийный,

		[EventDescription(GlobalSubsystemType.GK, "Запрет пуска НС", XStateClass.Off)]
		Запрет_пуска_НС,

		[EventDescription(GlobalSubsystemType.GK, "Запрет пуска компрессора", XStateClass.Off)]
		Запрет_пуска_компрессора,

		[EventDescription(GlobalSubsystemType.GK, "Команда с УЗН", XStateClass.Info)]
		Команда_с_УЗН,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в режим ручного управления", XStateClass.AutoOff)]
		Перевод_в_режим_ручного_управления,

		[EventDescription(GlobalSubsystemType.GK, "Состояние не определено", XStateClass.Unknown)]
		Состояние_не_определено,

		[EventDescription(GlobalSubsystemType.GK, "Остановлено", XStateClass.Off)]
		Остановлено,

		[EventDescription(GlobalSubsystemType.GK, "Состояние Неизвестно", XStateClass.Unknown)]
		Состояние_Неизвестно,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в автоматический режим", XStateClass.Norm)]
		Перевод_в_автоматический_режим,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в ручной режим", XStateClass.AutoOff)]
		Перевод_в_ручной_режим,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в отключенный режим", XStateClass.Ignore)]
		Перевод_в_отключенный_режим,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в неопределенный режим", XStateClass.Unknown)]
		Перевод_в_неопределенный_режим,

		[EventDescription(GlobalSubsystemType.GK, "Запись параметра", XStateClass.Info)]
		Запись_параметра,

		[EventDescription(GlobalSubsystemType.GK, "Норма", XStateClass.Norm)]
		Норма,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный код события объекта", XStateClass.Unknown)]
		Неизвестный_код_события_объекта,

		[EventDescription(GlobalSubsystemType.SKD, "Потеря связи", XStateClass.ConnectionLost)]
		Потеря_связи,

		[EventDescription(GlobalSubsystemType.SKD, "Восстановление связи", XStateClass.ConnectionLost)]
		Восстановление_связи,

		[EventDescription(GlobalSubsystemType.SKD, "Проход", XStateClass.Info)]
		Проход,

		[EventDescription(GlobalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Failure)]
		Дверь_не_закрыта,

		[EventDescription(GlobalSubsystemType.SKD, "Взлом", XStateClass.Attention)]
		Взлом,

		[EventDescription(GlobalSubsystemType.SKD, "Повторный_проход", XStateClass.Attention)]
		Повторный_проход,

		[EventDescription(GlobalSubsystemType.SKD, "Принуждение", XStateClass.Attention)]
		Принуждение,

		[EventDescription(GlobalSubsystemType.SKD, "Запрос пароля", XStateClass.Info)]
		Запрос_пароля,

		[EventDescription(GlobalSubsystemType.SKD, "Установка пароля", XStateClass.Info)]
		Установка_пароля,

		[EventDescription(GlobalSubsystemType.SKD, "Сброс Контроллера", XStateClass.Info)]
		Сброс_Контроллера,

		[EventDescription(GlobalSubsystemType.SKD, "Перезагрузка Контроллера", XStateClass.Info)]
		Перезагрузка_Контроллера,

		[EventDescription(GlobalSubsystemType.SKD, "Запись графиков работы", XStateClass.Info)]
		Запись_графиков_работы,

		[EventDescription(GlobalSubsystemType.SKD, "Обновление ПО Контроллера", XStateClass.Info)]
		Обновление_ПО_Контроллера,

		[EventDescription(GlobalSubsystemType.SKD, "Запрос конфигурации двери", XStateClass.Info)]
		Запрос_конфигурации_двери,

		[EventDescription(GlobalSubsystemType.SKD, "Запись конфигурации двери", XStateClass.Info)]
		Запись_конфигурации_двери,

		[EventDescription(GlobalSubsystemType.SKD, "Команда на открытие двери", XStateClass.Info)]
		Команда_на_открытие_двери,

		[EventDescription(GlobalSubsystemType.SKD, "Команда на закрытие двери", XStateClass.Info)]
		Команда_на_закрытие_двери,

		[EventDescription(GlobalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты,

		[EventDescription(GlobalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты,

		[EventDescription(GlobalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты,
	}
}