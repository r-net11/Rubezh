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

		[EventDescription(GlobalSubsystemType.SKD, "Ошибка при синхронизации временных интервалов", XStateClass.Info)]
		Ошибка_при_синхронизации_временных_интервалов,

		[EventDescription(GlobalSubsystemType.SKD, "Устранена ошибка при синхронизации временных интервалов", XStateClass.Info)]
		Устранена_ошибка_при_синхронизации_временных_интервалов,

		[EventDescription(GlobalSubsystemType.SKD, "Проход", XStateClass.Info)]
		Проход,

		[EventDescription(GlobalSubsystemType.SKD, "Проход с нарушением ВРЕМЕНИ", XStateClass.Info)]
		Проход_с_нарушением_ВРЕМЕНИ,


		[EventDescription(GlobalSubsystemType.SKD, "Проход с нарушением ЗОНАЛЬНОСТИ", XStateClass.Info)]
		Проход_с_нарушением_ЗОНАЛЬНОСТИ,

		[EventDescription(GlobalSubsystemType.SKD, "Проход с нарушением ВРЕМЕНИ и ЗОНАЛЬНОСТИ", XStateClass.Info)]
		Проход_с_нарушением_ВРЕМЕНИ_и_ЗОНАЛЬНОСТИ,

		[EventDescription(GlobalSubsystemType.SKD, "Проход разрешен", XStateClass.Info)]
		Проход_разрешен,

		[EventDescription(GlobalSubsystemType.SKD, "Нарушение ВРЕМЕНИ", XStateClass.Info)]
		Нарушение_ВРЕМЕНИ,

		[EventDescription(GlobalSubsystemType.SKD, "Нарушение ЗОНАЛЬНОСТИ", XStateClass.Info)]
		Нарушение_ЗОНАЛЬНОСТИ,

		[EventDescription(GlobalSubsystemType.SKD, "Нарушение ВРЕМЕНИ и ЗОНАЛЬНОСТИ", XStateClass.Info)]
		Нарушение_ВРЕМЕНИ_и_ЗОНАЛЬНОСТИ,

		[EventDescription(GlobalSubsystemType.SKD, "Идентификатор НЕ ЗАРЕГИСТРИРОВАН", XStateClass.Info)]
		Идентификатор_НЕ_ЗАРЕГИСТРИРОВАН,

		[EventDescription(GlobalSubsystemType.SKD, "Идентификатор_ЗАБЛОКИРОВАН", XStateClass.Info)]
		Идентификатор_ЗАБЛОКИРОВАН,

		[EventDescription(GlobalSubsystemType.SKD, "Деактивированный идентификатор", XStateClass.Info)]
		Деактивированный_идентификатор,

		[EventDescription(GlobalSubsystemType.SKD, "Идентификатор ПРОСРОЧЕН", XStateClass.Info)]
		Идентификатор_ПРОСРОЧЕН,

		[EventDescription(GlobalSubsystemType.SKD, "Нарушение режима доступа", XStateClass.Info)]
		Нарушение_режима_доступа,

		[EventDescription(GlobalSubsystemType.SKD, "Взлом ИУ", XStateClass.Info)]
		Взлом_ИУ,

		[EventDescription(GlobalSubsystemType.SKD, "Проход от ДУ", XStateClass.Info)]
		Проход_от_ДУ,

		[EventDescription(GlobalSubsystemType.SKD, "Запрос прохода от ДУ", XStateClass.Info)]
		Запрос_прохода_от_ДУ,

		[EventDescription(GlobalSubsystemType.SKD, "Ожидание комиссионирования прохода", XStateClass.Info)]
		Ожидание_комиссионирования_прохода,

		[EventDescription(GlobalSubsystemType.SKD, "Неисправность", XStateClass.Info)]
		Неисправность,

		[EventDescription(GlobalSubsystemType.SKD, "Неисправность устранена", XStateClass.Info)]
		Неисправность_устранена,

		[EventDescription(GlobalSubsystemType.SKD, "Команда управления", XStateClass.Info)]
		Команда_управления,

		[EventDescription(GlobalSubsystemType.SKD, "Конфигурирование", XStateClass.Info)]
		Конфигурирование,

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

		[EventDescription(GlobalSubsystemType.SKD, "Команда на открытие двери", XStateClass.Info)]
		Команда_на_открытие_двери,

		[EventDescription(GlobalSubsystemType.SKD, "Команда на закрытие двери", XStateClass.Info)]
		Команда_на_закрытие_двери,
	}
}