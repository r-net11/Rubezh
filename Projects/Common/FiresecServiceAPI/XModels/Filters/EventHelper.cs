using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFiresecAPI
{
    public static class EventHelper
    {
        public static List<XEvent> GetAllEvents()
        {
            var eventNames = new List<XEvent>();

            eventNames.Add(new XEvent("Очистка журнала", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("Установка часов", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("Запись информации о блоке", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("Смена ПО", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("Устройство с таким адресом не описано при конфигурации", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("При конфигурации описан другой тип", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("Вход пользователя в систему", XStateClass.TechnologicalRegime));
            eventNames.Add(new XEvent("Выход пользователя из системы", XStateClass.TechnologicalRegime));

            eventNames.Add(new XEvent("Технология", XStateClass.Service));
            eventNames.Add(new XEvent("Работа", XStateClass.Service));
            eventNames.Add(new XEvent("Запыленность", XStateClass.Service));
            eventNames.Add(new XEvent("Состояние", XStateClass.Service));
            eventNames.Add(new XEvent("Дежурный", XStateClass.Service));
            eventNames.Add(new XEvent("Команда оператора", XStateClass.Service));
            eventNames.Add(new XEvent("Управление", XStateClass.Service));
            eventNames.Add(new XEvent("Изменился заводской номер", XStateClass.Service));
            eventNames.Add(new XEvent("Режим работы", XStateClass.Service));
            eventNames.Add(new XEvent("Вход пользователя в прибор", XStateClass.Service));
            eventNames.Add(new XEvent("Выход пользователя из прибора", XStateClass.Service));
            eventNames.Add(new XEvent("Подтверждение тревоги", XStateClass.Service));

            eventNames.Add(new XEvent("Потеря связи с прибором", XStateClass.ConnectionLost));
            eventNames.Add(new XEvent("Восстановление связи с прибором", XStateClass.ConnectionLost));

            eventNames.Add(new XEvent("Смена БД", XStateClass.DBMissmatch));
            eventNames.Add(new XEvent("Неизвестный тип", XStateClass.Unknown));
            eventNames.Add(new XEvent("Пожар-1", XStateClass.Fire1));
            eventNames.Add(new XEvent("Пожар-2", XStateClass.Fire2));
            eventNames.Add(new XEvent("Внимание", XStateClass.Attention));
            eventNames.Add(new XEvent("Неисправность", XStateClass.Failure));
            eventNames.Add(new XEvent("Тест", XStateClass.Info));
            eventNames.Add(new XEvent("Отключение", XStateClass.Off));

            return eventNames;
        }
    }
}
