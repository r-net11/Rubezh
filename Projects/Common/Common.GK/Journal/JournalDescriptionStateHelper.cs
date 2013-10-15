using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class JournalDescriptionStateHelper
	{
		public static XStateClass GetStateClassByName(string name)
		{
			var journalDescriptionState = JournalDescriptionStates.FirstOrDefault(x => x.Name == name);
			if (journalDescriptionState != null)
				return journalDescriptionState.StateClass;
			return XStateClass.Norm;
		}

		static JournalDescriptionStateHelper()
		{
			JournalDescriptionStates = new List<JournalDescriptionState>();
			Add("Технология", XStateClass.TechnologicalRegime);
			Add("Установка часов", XStateClass.Info);
			Add("Запись информации о блоке", XStateClass.TechnologicalRegime);
			Add("Смена ПО", XStateClass.TechnologicalRegime);
			Add("Смена БД", XStateClass.TechnologicalRegime);
			Add("Работа", XStateClass.On);
			Add("Вход пользователя в прибор", XStateClass.Info);
			Add("Выход пользователя из прибора", XStateClass.Info);
			Add("Ошибка управления", XStateClass.Failure);
			Add("Введен новый пользователь", XStateClass.Info);
			Add("Изменена учетная информация пользователя", XStateClass.Info);
			Add("Произведена настройка сети", XStateClass.Info);
			Add("Неизвестный тип", XStateClass.Unknown);
			Add("Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown);
			Add("При конфигурации описан другой тип", XStateClass.Unknown);
			Add("Изменился заводской номер", XStateClass.Info);
			Add("Пожар-1", XStateClass.Fire1);
			Add("Сработка-1", XStateClass.Fire1);
			Add("Пожар-2", XStateClass.Fire2);
			Add("Сработка-2", XStateClass.Fire2);
			Add("Внимание", XStateClass.Attention);
			Add("Неисправность", XStateClass.Failure);
			Add("Тест", XStateClass.Test);
			Add("Запыленность", XStateClass.Service);
			Add("Информация", XStateClass.Info);
			Add("Состояние", XStateClass.Info);
			Add("Режим работы", XStateClass.Info);
			Add("Параметры", XStateClass.Info);
			Add("Норма", XStateClass.Norm);
			Add("Вход пользователя в систему", XStateClass.Info);
			Add("Выход пользователя из системы", XStateClass.Info);
			Add("Команда оператора", XStateClass.Info);
		}

		static void Add(string name, XStateClass stateClass)
		{
			var journalDescriptionState = new JournalDescriptionState(name, stateClass);
			JournalDescriptionStates.Add(journalDescriptionState);
		}

		public static List<JournalDescriptionState> JournalDescriptionStates { get; private set; }

		public class JournalDescriptionState
		{
			public JournalDescriptionState(string name, XStateClass stateClass)
			{
				Name = name;
				StateClass = stateClass;
			}

			public string Name { get; private set; }
			public XStateClass StateClass { get; private set; }
		}
	}
}