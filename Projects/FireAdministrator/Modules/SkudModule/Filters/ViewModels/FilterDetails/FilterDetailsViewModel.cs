using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDJournalFilter JournalFilter { get; set; }

		public FilterDetailsViewModel(SKDJournalFilter journalFilter = null)
		{
			EventNames = new ObservableCollection<SKDEventNameViewModel>();
			EventNames.Add(new SKDEventNameViewModel(""));
			EventNames.Add(new SKDEventNameViewModel(""));

			if (journalFilter == null)
			{
				Title = "Создание нового фильтра";

				JournalFilter = new SKDJournalFilter()
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

			foreach (var eventViewModel in EventNames)
			{
				foreach (var eventName in JournalFilter.EventNames)
				{
					if (eventName == eventViewModel.EventName)
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

		public ObservableCollection<SKDEventNameViewModel> EventNames { get; private set; }

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
			JournalFilter.EventNames = EventNames.Where(x => x.IsChecked).Select(x => x.EventName).ToList();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return EventNames.Where(x => x.IsChecked == true).ToList().Count > 0;
		}
	}
}