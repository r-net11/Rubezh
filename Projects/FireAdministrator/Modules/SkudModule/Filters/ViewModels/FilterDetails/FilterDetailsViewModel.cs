using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDJournalFilter JournalFilter { get; set; }

		public FilterDetailsViewModel(SKDJournalFilter journalFilter = null)
		{
			EventNames = new ObservableCollection<SKDEventNameViewModel>();
			EventNames.Add(new SKDEventNameViewModel("Проход"));
			EventNames.Add(new SKDEventNameViewModel("Проход с нарушением ВРЕМЕНИ"));
			EventNames.Add(new SKDEventNameViewModel("Проход с нарушением ЗОНАЛЬНОСТИ"));

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

			Devices = new ObservableCollection<SKDDevice>();
		}

		void CopyProperties()
		{
			Name = JournalFilter.Name;
			Description = JournalFilter.Description;

			foreach (var eventViewModel in EventNames)
			{
				foreach (var eventName in JournalFilter.EventNames)
				{
					if (eventName == eventViewModel.Name)
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

		public ObservableCollection<SKDDevice> Devices { get; private set; }

		public ObservableCollection<SKDEventNameViewModel> EventNames { get; private set; }

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
			JournalFilter.EventNames = EventNames.Where(x => x.IsChecked).Select(x => x.Name).ToList();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return EventNames.Where(x => x.IsChecked == true).ToList().Count > 0;
		}
	}
}