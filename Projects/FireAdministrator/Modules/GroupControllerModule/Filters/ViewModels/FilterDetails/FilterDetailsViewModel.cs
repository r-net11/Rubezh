using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using GKProcessor;
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
			foreach (var eventName in EventNameHelper.EventNames)
			{
				EventNames.Add(new EventNameViewModel(eventName));
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
			
			foreach (var eventViewModel in EventNames)
			{
				foreach (var eventName in JournalFilter.EventNames)
				{
					if (eventName == eventViewModel.EventName.Name)
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

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
			JournalFilter.LastRecordsCount = LastRecordsCount;
			JournalFilter.StateClasses = StateClasses.Where(x => x.IsChecked).Select(x => x.StateClass).Cast<XStateClass>().ToList();
			JournalFilter.EventNames = EventNames.Where(x => x.IsChecked).Select(x => x.EventName.Name).ToList();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return StateClasses.Where(x => x.IsChecked == true).ToList().Count > 0 ||
				EventNames.Where(x => x.IsChecked == true).ToList().Count > 0;
		}
	}
}