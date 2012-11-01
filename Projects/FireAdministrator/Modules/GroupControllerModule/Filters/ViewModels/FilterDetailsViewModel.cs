using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.Generic;

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

            EventNames = new ObservableCollection<FilterEventNameViewModel>();
            foreach (var eventName in GetAvailableEventNames())
            {
                EventNames.Add(new FilterEventNameViewModel(eventName));
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
                    x => x == viewModel.EventName)).All(x => x.IsChecked = true);
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
        public ObservableCollection<FilterEventNameViewModel> EventNames { get; private set; }

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
            eventNames.Add("Очистка журнала");
            eventNames.Add("Установка часов");
            eventNames.Add("Запись информации о блоке");
            eventNames.Add("Смена ПО");
            eventNames.Add("Смена БД");
            eventNames.Add("Работа");
            eventNames.Add("Неизвестный тип");
            eventNames.Add("Устройство с таким адресом не описано при конфигурации");
            eventNames.Add("При конфигурации описан другой тип");
            eventNames.Add("Изменился заводской номер");
            eventNames.Add("Пожар");
            eventNames.Add("Пожар-2");
            eventNames.Add("Внимание");
            eventNames.Add("Неисправность");
            eventNames.Add("Тест");
            eventNames.Add("Запыленность");
            eventNames.Add("Управление");
            eventNames.Add("Состояние");
            eventNames.Add("Режим работы");
            eventNames.Add("Дежурный");
            eventNames.Add("Отключение");
            eventNames.Add("Вход пользователя в систему");
            eventNames.Add("Выход пользователя из системы");
            return eventNames;
        }

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
            JournalFilter.LastRecordsCount = LastRecordsCount;
            JournalFilter.StateClasses = StateClasses.Where(x => x.IsChecked).Select(x => x.StateClass).Cast<XStateClass>().ToList();
            JournalFilter.EventNames = EventNames.Where(x => x.IsChecked).Select(x => x.EventName).Cast<string>().ToList();
			return base.Save();
		}
    }
}