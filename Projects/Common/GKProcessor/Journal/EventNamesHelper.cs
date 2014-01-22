using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKProcessor
{
    public class EventNamesHelper
    {
        public static XStateClass GetStateClass(EventName eventName)
        {
            switch (eventName)
            {
                case (EventName.Подтверждение_тревоги):
                case (EventName.Смена_пользователя):
                case (EventName.Вход_пользователя_в_систему):
                case (EventName.Выход_пользователя_из_системы):
                case (EventName.Зависание_процесса_отпроса):
                case (EventName.Отсутствует_лицензия):
                case (EventName.Лицензия_обнаружена):
                case (EventName.Ошибка_инициализации_мониторинга):
                case (EventName.Применение_конфигурации):
                    return XStateClass.Norm;
                case (EventName.Обновление_ПО_прибора):
                case (EventName.Запись_конфигурации_в_прибор):
                case (EventName.Чтение_конфигурации_из_прибора):
                case (EventName.Синхронизация_времени):
                case (EventName.Запрос_информации_об_устройсве):
                case (EventName.Команда_оператора):
                    return XStateClass.Info;
                case (EventName.Ошибка_при_выполнении_команды):
                case (EventName.Ошибка_при_выполнении_команды_над_устройством):
                    return XStateClass.Failure;
                default:
                    return XStateClass.Unknown;
            }
        }
    }
}
