using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Common.GK;

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

			JournalDescriptionStates = new ObservableCollection<JournalDescriptionStateViewModel>();
            foreach (var journalDescriptionState in JournalDescriptionStateHelper.JournalDescriptionStates)
			{
                JournalDescriptionStates.Add(new JournalDescriptionStateViewModel(journalDescriptionState));
			}

			if (journalFilter == null)
			{
				Title = "Создание нового фильтра";

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
            
            foreach (var eventViewModel in JournalDescriptionStates)
            {
                foreach (var xEvent in JournalFilter.EventNames)
                {
                    if (xEvent.Name == eventViewModel.JournalDescriptionState.Name)
                        eventViewModel.IsChecked = true;
                }
            }
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
		public ObservableCollection<JournalDescriptionStateViewModel> JournalDescriptionStates { get; private set; }

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

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
			JournalFilter.LastRecordsCount = LastRecordsCount;
			JournalFilter.StateClasses = StateClasses.Where(x => x.IsChecked).Select(x => x.StateClass).Cast<XStateClass>().ToList();
            JournalFilter.EventNames = JournalDescriptionStates.Where(x => x.IsChecked).Select(x => x.JournalDescriptionState).ToList();
			return base.Save();
		}

        protected override bool CanSave()
        {
            var result = StateClasses.Where(x => x.IsChecked == true).ToList().Count > 0 ||
                JournalDescriptionStates.Where(x => x.IsChecked == true).ToList().Count > 0;
            return result;
        }
	}

	

}