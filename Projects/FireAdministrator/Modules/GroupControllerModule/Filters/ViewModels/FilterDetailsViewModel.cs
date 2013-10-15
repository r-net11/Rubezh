using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public XJournalFilter JournalFilter { get; set; }

		public FilterDetailsViewModel(XJournalFilter journalFilter = null)
		{
			StateClasses = new ObservableCollection<FilterStateClassViewModel>();
			foreach (var stateClass in GetAvailableStateClasses())
			{
				StateClasses.Add(new FilterStateClassViewModel(stateClass));
			}

			EventNames = new ObservableCollection<EventNameViewModel>();
			foreach (var eventName in GetAvailableEventNames())
			{
				EventNames.Add(new EventNameViewModel(eventName));
			}

			if (journalFilter == null)
			{
				Title = "Создание новоого фильтра";

				JournalFilter = new XJournalFilter()
				{
					Name = "Новый фильтр",
					Description = "Описание фильтра"
				};
			}
			else
			{
				Title = string.Format("Свойства фильтра: {0}", journalFilter.Name);
				JournalFilter = journalFilter;
			}
			CopyProperties();
		}

		void CopyProperties()
		{
			Name = JournalFilter.Name;
			Description = JournalFilter.Description;
			LastRecordsCount = JournalFilter.LastRecordsCount;

			StateClasses.Where(
				viewModel => JournalFilter.StateClasses.Any(
					x => x == viewModel.StateClass)).All(x => x.IsChecked = true);

			EventNames.Where(
				viewModel => JournalFilter.EventNames.Any(
                    x => x == viewModel.Name)).All(x => x.IsChecked = true);
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		int _lastRecordsCount;
		public int LastRecordsCount
		{
			get { return _lastRecordsCount; }
			set
			{
				_lastRecordsCount = value;
				OnPropertyChanged("LastRecordsCount");
			}
		}

		public ObservableCollection<FilterStateClassViewModel> StateClasses { get; private set; }
		public ObservableCollection<EventNameViewModel> EventNames { get; private set; }

		List<XStateClass> GetAvailableStateClasses()
		{
			var states = new List<XStateClass>();
			states.Add(XStateClass.Fire2);
			states.Add(XStateClass.Fire1);
			states.Add(XStateClass.Attention);
			states.Add(XStateClass.Failure);
			states.Add(XStateClass.Ignore);
			states.Add(XStateClass.On);
			states.Add(XStateClass.Unknown);
			states.Add(XStateClass.Service);
			states.Add(XStateClass.Info);
			states.Add(XStateClass.Norm);
			return states;
		}

		List<string> GetAvailableEventNames()
		{
			var eventNames = new List<string>();
			eventNames.Add("Технология");
			eventNames.Add("Установка часов");
			eventNames.Add("Смена ПО");
			eventNames.Add("Смена БД");
			eventNames.Add("Работа");
			eventNames.Add("Вход пользователя в прибор");
			eventNames.Add("Выход пользователя из прибора");
			eventNames.Add("Ошибка управления");
			eventNames.Add("Введен новый пользователь");
			eventNames.Add("Изменена учетная информация пользователя");
			eventNames.Add("Произведена настройка сети");
			eventNames.Add("Неизвестный тип");
			eventNames.Add("Устройство с таким адресом не описано при конфигурации");
			eventNames.Add("При конфигурации описан другой тип");
			eventNames.Add("Изменился заводской номер");
            eventNames.Add("Пожар-1");
			eventNames.Add("Пожар-2");
			eventNames.Add("Сработка-1");
			eventNames.Add("Сработка-2");
			eventNames.Add("Внимание");
			eventNames.Add("Неисправность");
			eventNames.Add("Тест");
			eventNames.Add("Запыленность");
			eventNames.Add("Информация");
			eventNames.Add("Состояние");
			eventNames.Add("Режим работы");
			eventNames.Add("Параметры");
			eventNames.Add("Норма");
			eventNames.Add("Вход пользователя в систему");
			eventNames.Add("Выход пользователя из системы");
			eventNames.Add("Команда оператора");
			return eventNames;
		}

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
			JournalFilter.LastRecordsCount = LastRecordsCount;
			JournalFilter.StateClasses = StateClasses.Where(x => x.IsChecked).Select(x => x.StateClass).Cast<XStateClass>().ToList();
            JournalFilter.EventNames = EventNames.Where(x => x.IsChecked).Select(x => x.Name).Cast<string>().ToList();
			return base.Save();
		}
	}

	public class EventNameViewModel : BaseViewModel
	{
		public EventNameViewModel(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public string ImageSource
		{
			get
			{
				switch (Name)
				{
					case "Очистка журнала":
					case "Установка часов":
					case "Запись информации о блоке":
					case "Смена ПО":
					case "Устройство с таким адресом не описано при конфигурации":
					case "При конфигурации описан другой тип":
					case "Вход пользователя в систему":
					case "Выход пользователя из системы":
						return "/Controls;component/StateClassIcons/TechnologicalRegime.png";

					case "Технология":
					case "Работа":
					case "Запыленность":
					case "Состояние":
					case "Дежурный":
					case "Команда оператора":
					case "Управление":
					case "Изменился заводской номер":
					case "Режим работы":
					case "Вход пользователя в прибор":
					case "Выход пользователя из прибора":
					case "Подтверждение тревоги":
						return "/Controls;component/StateClassIcons/Service.png";

					case "Смена БД":
						return "/Controls;component/StateClassIcons/DBMissmatch.png";

					case "Неизвестный тип":
						return "/Controls;component/StateClassIcons/Unknown.png";

					case "Пожар-1":
						return "/Controls;component/StateClassIcons/Fire1.png";

					case "Пожар-2":
						return "/Controls;component/StateClassIcons/Fire2.png";

					case "Внимание":
						return "/Controls;component/StateClassIcons/Attention.png";

					case "Неисправность":
						return "/Controls;component/StateClassIcons/Failure.png";

					case "Тест":
						return "/Controls;component/StateClassIcons/Info.png";

					case "Отключение":
						return "/Controls;component/StateClassIcons/Off.png";

					case "Потеря связи с прибором":
					case "Восстановление связи с прибором":
						return "/Controls;component/StateClassIcons/ConnectionLost.png";

					default:
						return "";
				}
			}
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}

}