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
		[EventDescription(GlobalSubsystemType.System, "Неизвестное событие", XStateClass.Info)]
		Неизвестное_событие,

		[EventDescription(GlobalSubsystemType.System, "Подтверждение тревоги", XStateClass.Info)]
		Подтверждение_тревоги,

		[EventDescription(GlobalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему,

		[EventDescription(GlobalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы,

		[EventDescription(GlobalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал,

		[EventDescription(GlobalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял,

		[EventDescription(GlobalSubsystemType.System, "Зависание процесса отпроса", XStateClass.Info)]
		Зависание_процесса_отпроса,

		[EventDescription(GlobalSubsystemType.System, "Отсутствует лицензия", XStateClass.Info)]
		Отсутствует_лицензия,

		[EventDescription(GlobalSubsystemType.System, "Лицензия обнаружена", XStateClass.Info)]
		Лицензия_обнаружена,

		[EventDescription(GlobalSubsystemType.System, "Ошибка инициализации мониторинга", XStateClass.Info)]
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

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при выполнении команды", XStateClass.Info)]
		Ошибка_при_выполнении_команды,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при выполнении команды над устройством", XStateClass.Info)]
		Ошибка_при_выполнении_команды_над_устройством,

		[EventDescription(GlobalSubsystemType.GK, "Нет связи с ГК", XStateClass.Info)]
		Нет_связи_с_ГК,

		[EventDescription(GlobalSubsystemType.GK, "Связь с ГК восстановлена", XStateClass.Info)]
		Связь_с_ГК_восстановлена,

		[EventDescription(GlobalSubsystemType.GK, "Конфигурация прибора не соответствует конфигурации ПК", XStateClass.Info)]
		Конфигурация_прибора_не_соответствует_конфигурации_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Конфигурация прибора соответствует конфигурации ПК", XStateClass.Info)]
		Конфигурация_прибора_соответствует_конфигурации_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при синхронизации журнала", XStateClass.Info)]
		Ошибка_при_синхронизации_журнала,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка при опросе состояний компонентов ГК", XStateClass.Info)]
		Ошибка_при_опросе_состояний_компонентов_ГК,

		[EventDescription(GlobalSubsystemType.GK, "Устранена ошибка при опросе состояний компонентов ГК", XStateClass.Info)]
		Устранена_ошибка_при_опросе_состояний_компонентов_ГК,

		[EventDescription(GlobalSubsystemType.GK, "Восстановление связи с прибором", XStateClass.Info)]
		Восстановление_связи_с_прибором,

		[EventDescription(GlobalSubsystemType.GK, "Потеря связи с прибором", XStateClass.Info)]
		Потеря_связи_с_прибором,

		[EventDescription(GlobalSubsystemType.System, "База данных прибора не соответствует базе данных ПК", XStateClass.Info)]
		База_данных_прибора_не_соответствует_базе_данных_ПК,

		[EventDescription(GlobalSubsystemType.GK, "База данных прибора соответствует базе данных ПК", XStateClass.Info)]
		База_данных_прибора_соответствует_базе_данных_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации,

		[EventDescription(GlobalSubsystemType.GK, "Отмена операции", XStateClass.Info)]
		Отмена_операции,

		[EventDescription(GlobalSubsystemType.GK, "ГК в технологическом режиме", XStateClass.Info)]
		ГК_в_технологическом_режиме,

		[EventDescription(GlobalSubsystemType.GK, "ГК в рабочем режиме", XStateClass.Info)]
		ГК_в_рабочем_режиме,

		[EventDescription(GlobalSubsystemType.GK, "Запись всех идентификаторов", XStateClass.Info)]
		Запись_всех_идентификаторов,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в технологический режим", XStateClass.Info)]
		Перевод_в_технологический_режим,

		[EventDescription(GlobalSubsystemType.GK, "Синхронизация времени прибора с временем ПК", XStateClass.Info)]
		Синхронизация_времени_прибора_с_временем_ПК,

		[EventDescription(GlobalSubsystemType.GK, "Смена ПО", XStateClass.Info)]
		Смена_ПО,

		[EventDescription(GlobalSubsystemType.GK, "Смена БД", XStateClass.Info)]
		Смена_БД,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в рабочий режим", XStateClass.Info)]
		Перевод_в_рабочий_режим,

		[EventDescription(GlobalSubsystemType.GK, "Вход пользователя в прибор", XStateClass.Info)]
		Вход_пользователя_в_прибор,

		[EventDescription(GlobalSubsystemType.GK, "Выход пользователя из прибора", XStateClass.Info)]
		Выход_пользователя_из_прибора,

		[EventDescription(GlobalSubsystemType.GK, "Ошибка управления", XStateClass.Info)]
		Ошибка_управления,

		[EventDescription(GlobalSubsystemType.GK, "Введен новый пользователь", XStateClass.Info)]
		Введен_новый_пользователь,

		[EventDescription(GlobalSubsystemType.GK, "Изменена учетная информация пользователя", XStateClass.Info)]
		Изменена_учетная_информация_пользователя,

		[EventDescription(GlobalSubsystemType.GK, "Произведена настройка сети", XStateClass.Info)]
		Произведена_настройка_сети,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный код события контроллекра", XStateClass.Info)]
		Неизвестный_код_события_контроллекра,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный тип", XStateClass.Info)]
		Неизвестный_тип,

		[EventDescription(GlobalSubsystemType.GK, "Устройство с таким адресом не описано при конфигурации", XStateClass.Info)]
		Устройство_с_таким_адресом_не_описано_при_конфигурации,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный код события устройства", XStateClass.Info)]
		Неизвестный_код_события_устройства,

		[EventDescription(GlobalSubsystemType.GK, "При конфигурации описан другой тип", XStateClass.Info)]
		При_конфигурации_описан_другой_тип,

		[EventDescription(GlobalSubsystemType.GK, "Изменился заводской номер", XStateClass.Info)]
		Изменился_заводской_номер,

		[EventDescription(GlobalSubsystemType.GK, "Пожар-1", XStateClass.Info)]
		Пожар_1,

		[EventDescription(GlobalSubsystemType.GK, "Сработка-1", XStateClass.Info)]
		Сработка_1,

		[EventDescription(GlobalSubsystemType.GK, "Пожар-2", XStateClass.Info)]
		Пожар_2,

		[EventDescription(GlobalSubsystemType.GK, "Сработка-2", XStateClass.Info)]
		Сработка_2,

		[EventDescription(GlobalSubsystemType.GK, "Внимание", XStateClass.Info)]
		Внимание,

		[EventDescription(GlobalSubsystemType.GK, "Неисправность", XStateClass.Failure)]
		Неисправность,

		[EventDescription(GlobalSubsystemType.GK, "Неисправность устранена", XStateClass.Norm)]
		Неисправность_устранена,

		[EventDescription(GlobalSubsystemType.GK, "Тест", XStateClass.Info)]
		Тест,

		[EventDescription(GlobalSubsystemType.GK, "Тест устранен", XStateClass.Info)]
		Тест_устранен,

		[EventDescription(GlobalSubsystemType.GK, "Запыленность", XStateClass.Info)]
		Запыленность,

		[EventDescription(GlobalSubsystemType.GK, "Запыленность устранена", XStateClass.Info)]
		Запыленность_устранена,

		[EventDescription(GlobalSubsystemType.GK, "Информация", XStateClass.Info)]
		Информация,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в автоматический режим", XStateClass.Info)]
		Перевод_в_автоматический_режим,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в ручной режим", XStateClass.Info)]
		Перевод_в_ручной_режим,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в отключенный режим", XStateClass.Info)]
		Перевод_в_отключенный_режим,

		[EventDescription(GlobalSubsystemType.GK, "Перевод в неопределенный режим", XStateClass.Info)]
		Перевод_в_неопределенный_режим,

		[EventDescription(GlobalSubsystemType.GK, "Запись параметра", XStateClass.Info)]
		Запись_параметра,

		[EventDescription(GlobalSubsystemType.GK, "Норма", XStateClass.Info)]
		Норма,

		[EventDescription(GlobalSubsystemType.GK, "Неизвестный код события объекта", XStateClass.Info)]
		Неизвестный_код_события_объекта,

		[EventDescription(GlobalSubsystemType.SKD, "Потеря связи", XStateClass.Info)]
		Потеря_связи,

		[EventDescription(GlobalSubsystemType.SKD, "Восстановление связи", XStateClass.Info)]
		Восстановление_связи,

		[EventDescription(GlobalSubsystemType.SKD, "Проход", XStateClass.Info)]
		Проход,

		[EventDescription(GlobalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Info)]
		Дверь_не_закрыта,

		[EventDescription(GlobalSubsystemType.SKD, "Взлом", XStateClass.Info)]
		Взлом,

		[EventDescription(GlobalSubsystemType.SKD, "Повторный_проход", XStateClass.Info)]
		Повторный_проход,

		[EventDescription(GlobalSubsystemType.SKD, "Принуждение", XStateClass.Info)]
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