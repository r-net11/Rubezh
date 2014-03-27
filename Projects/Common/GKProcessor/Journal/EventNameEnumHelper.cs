using XFiresecAPI;

namespace GKProcessor
{
	public class EventNamesHelper
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
					return XStateClass.Info;
				case (EventNameEnum.Ошибка_при_выполнении_команды):
				case (EventNameEnum.Ошибка_при_выполнении_команды_над_устройством):
					return XStateClass.Failure;
				default:
					return XStateClass.Unknown;
			}
		}
	}
}