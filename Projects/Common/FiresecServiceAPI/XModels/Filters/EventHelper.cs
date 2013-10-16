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

            eventNames.Add(new XEvent("Установка часов", XStateClass.TechnologicalRegime));//
            eventNames.Add(new XEvent("Смена ПО", XStateClass.TechnologicalRegime));//только в гк и кау
            eventNames.Add(new XEvent("Устройство с таким адресом не описано при конфигурации", XStateClass.TechnologicalRegime));//есть на шлейфе но нет в конф
            eventNames.Add(new XEvent("При конфигурации описан другой тип", XStateClass.TechnologicalRegime));//
            eventNames.Add(new XEvent("Вход пользователя в систему", XStateClass.TechnologicalRegime)); //
            eventNames.Add(new XEvent("Выход пользователя из системы", XStateClass.TechnologicalRegime)); //

            eventNames.Add(new XEvent("Технология", XStateClass.Service));// перевод в техн режим для обн по, зап, чт конф
            eventNames.Add(new XEvent("Работа", XStateClass.Service));//работа шкафов
            eventNames.Add(new XEvent("Запыленность", XStateClass.Service));//кр ур зап у дат
            eventNames.Add(new XEvent("Состояние", XStateClass.Service));//изм сост объекта
            eventNames.Add(new XEvent("Команда оператора", XStateClass.Service));//команда на сброс, упр исп устр, откл, сн откл
            
            eventNames.Add(new XEvent("Изменился заводской номер", XStateClass.Service));//
            eventNames.Add(new XEvent("Режим работы", XStateClass.Service));//авт руч откл
            eventNames.Add(new XEvent("Вход пользователя в прибор", XStateClass.Service));//
            eventNames.Add(new XEvent("Выход пользователя из прибора", XStateClass.Service));//
            
            eventNames.Add(new XEvent("Потеря связи с прибором", XStateClass.ConnectionLost));//
            eventNames.Add(new XEvent("Восстановление связи с прибором", XStateClass.ConnectionLost));//

            eventNames.Add(new XEvent("Смена БД", XStateClass.DBMissmatch));//в ГК и КАУ
            eventNames.Add(new XEvent("Неизвестный тип", XStateClass.Unknown));//не подд
            eventNames.Add(new XEvent("Пожар-1", XStateClass.Fire1));//
            eventNames.Add(new XEvent("Пожар-2", XStateClass.Fire2));//из зоны
            eventNames.Add(new XEvent("Сработка-1", XStateClass.Fire1));//
            eventNames.Add(new XEvent("Сработка-2", XStateClass.Fire2));//из устр
            eventNames.Add(new XEvent("Внимание", XStateClass.Attention));//из зоны
            eventNames.Add(new XEvent("Неисправность", XStateClass.Failure));//из устр
            eventNames.Add(new XEvent("Тест", XStateClass.Info));//из устр
            eventNames.Add(new XEvent("Информация", XStateClass.Info));//из устр

            eventNames.Add(new XEvent("Норма", XStateClass.Info));//переход в сост норма
            eventNames.Add(new XEvent("Параметры", XStateClass.Info));//изм параметров
            eventNames.Add(new XEvent("Ошибка управления", XStateClass.Info));
            eventNames.Add(new XEvent("Введен новый пользователь", XStateClass.Info));
            eventNames.Add(new XEvent("Изменена учетная информация пользователя", XStateClass.Info));
            eventNames.Add(new XEvent("Произведена настройка сети", XStateClass.Info));//изм сет настр
            
            return eventNames;
        }
    }
}
