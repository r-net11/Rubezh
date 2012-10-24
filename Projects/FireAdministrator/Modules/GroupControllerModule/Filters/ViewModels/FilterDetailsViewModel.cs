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
            StateTypes = new ObservableCollection<StateTypeViewModel>();
            foreach (var stateType in GetAvailableStates())
			{
				StateTypes.Add(new StateTypeViewModel(stateType));
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

            StateTypes.Where(
                eventViewModel => JournalFilter.StateTypes.Any(
                    x => x == eventViewModel.StateType)).All(x => x.IsChecked = true);
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

        public ObservableCollection<StateTypeViewModel> StateTypes { get; private set; }

        List<XStateType> GetAvailableStates()
        {
            var states = new List<XStateType>();
            states.Add(XStateType.Attention);
            states.Add(XStateType.Fire1);
            states.Add(XStateType.Fire2);
            states.Add(XStateType.Test);
            states.Add(XStateType.Failure);
            states.Add(XStateType.Ignore);
            states.Add(XStateType.On);
            return states;
        }

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
            JournalFilter.LastRecordsCount = LastRecordsCount;
            JournalFilter.StateTypes = StateTypes.Where(x => x.IsChecked).Select(x => x.StateType).Cast<XStateType>().ToList();
			return base.Save();
		}
    }
}