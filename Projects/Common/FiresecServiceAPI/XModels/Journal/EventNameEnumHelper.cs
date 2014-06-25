namespace FiresecAPI.GK
{
	public static class EventNamesHelper
	{
		public static XStateClass GetStateClass(EventNameEnum eventName)
		{
			switch (eventName)
			{
				case (EventNameEnum.Подтверждение_тревоги):
				case (EventNameEnum.Вход_пользователя_в_систему):
				case (EventNameEnum.Выход_пользователя_из_системы):
				case (EventNameEnum.Дежурство_сдал):
				case (EventNameEnum.Дежурство_принял):
				case (EventNameEnum.Зависание_процесса_отпроса):
				case (EventNameEnum.Отсутствует_лицензия):
				case (EventNameEnum.Лицензия_обнаружена):
				case (EventNameEnum.Ошибка_инициализации_мониторинга):
				case (EventNameEnum.Применение_конфигурации):
				case (EventNameEnum.Проход):
				case (EventNameEnum.Проход_разрешен):
				case (EventNameEnum.Проход_от_ДУ):
				case (EventNameEnum.Запрос_прохода_от_ДУ):
				case (EventNameEnum.Ожидание_комиссионирования_прохода):
				case (EventNameEnum.Неисправность_устранена):
					return XStateClass.Norm;
				case (EventNameEnum.Обновление_ПО_прибора):
				case (EventNameEnum.Запись_конфигурации_в_прибор):
				case (EventNameEnum.Чтение_конфигурации_из_прибора):
				case (EventNameEnum.Синхронизация_времени):
				case (EventNameEnum.Запрос_информации_об_устройстве):
				case (EventNameEnum.Команда_оператора):
				case (EventNameEnum.Отмена_операции):
				case (EventNameEnum.ГК_в_технологическом_режиме):
				case (EventNameEnum.ГК_в_рабочем_режиме):
				case (EventNameEnum.Запись_всех_идентификаторов):
				case (EventNameEnum.Устранена_ошибка_при_синхронизации_временных_интервалов):
				case (EventNameEnum.Проход_с_нарушением_ВРЕМЕНИ):
				case (EventNameEnum.Проход_с_нарушением_ЗОНАЛЬНОСТИ):
				case (EventNameEnum.Проход_с_нарушением_ВРЕМЕНИ_и_ЗОНАЛЬНОСТИ):
				case (EventNameEnum.Нарушение_ВРЕМЕНИ):
				case (EventNameEnum.Нарушение_ЗОНАЛЬНОСТИ):
				case (EventNameEnum.Нарушение_ВРЕМЕНИ_и_ЗОНАЛЬНОСТИ):
				case (EventNameEnum.Идентификатор_НЕ_ЗАРЕГИСТРИРОВАН):
				case (EventNameEnum.Идентификатор_ЗАБЛОКИРОВАН):
				case (EventNameEnum.Деактивированный_идентификатор):
				case (EventNameEnum.Идентификатор_ПРОСРОЧЕН):
				case (EventNameEnum.Команда_управления):
				case (EventNameEnum.Конфигурирование):
					return XStateClass.Info;
				case (EventNameEnum.Ошибка_при_выполнении_команды):
				case (EventNameEnum.Ошибка_при_выполнении_команды_над_устройством):
				case (EventNameEnum.Ошибка_при_синхронизации_временных_интервалов):
				case (EventNameEnum.Нарушение_режима_доступа):
				case (EventNameEnum.Взлом_ИУ):
				case (EventNameEnum.Неисправность):
					return XStateClass.Failure;
				default:
					return XStateClass.Unknown;
			}
		}
	}
}