using System;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public ArchiveFilterViewModel(SKDArchiveFilter archiveFilter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);
			Initialize(archiveFilter);
		}

		void Initialize(SKDArchiveFilter archiveFilter)
		{
			StartDateTime = new DateTimePairViewModel(archiveFilter.StartDate);
			EndDateTime = new DateTimePairViewModel(archiveFilter.EndDate);
			InitializeEventNames(archiveFilter);
		}

		public void InitializeEventNames(SKDArchiveFilter archiveFilter)
		{
			EventNames = new CheckBoxItemList<EventNameViewModel>();
			//foreach (var eventName in EventNameHelper.EventNames)
			//{
			//    EventNames.Add(new EventNameViewModel(eventName, DistinctDatabaseNames));
			//}
			//EventNames.Items.Sort(EventNameViewModel.Compare);
			//foreach (var eventName in archiveFilter.EventNames)
			//{
			//    var eventNameViewModel = EventNames.Items.FirstOrDefault(x => (x as EventNameViewModel).EventName.Name == eventName);
			//    if (eventNameViewModel != null)
			//    {
			//        eventNameViewModel.IsChecked = true;
			//    }
			//}
		}

		public DateTime ArchiveFirstDate
		{
			get { return ArchiveViewModel.ArchiveFirstDate; }
		}

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

		DateTimePairViewModel _startDateTime;
		public DateTimePairViewModel StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged("StartDateTime");
			}
		}

		DateTimePairViewModel _endDateTime;
		public DateTimePairViewModel EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged("EndDateTime");
			}
		}

		bool useDeviceDateTime;
		public bool UseDeviceDateTime
		{
			get { return useDeviceDateTime; }
			set
			{
				useDeviceDateTime = value;
				OnPropertyChanged("UseDeviceDateTime");
			}
		}

		public CheckBoxItemList<EventNameViewModel> EventNames { get; private set; }

		public SKDArchiveFilter GetModel()
		{
			var archiveFilter = new SKDArchiveFilter()
			{
				StartDate = StartDateTime.DateTime,
				EndDate = EndDateTime.DateTime,
				UseDeviceDateTime = UseDeviceDateTime
			};
			foreach (var eventName in EventNames.Items)
			{
				if (eventName.IsChecked)
					archiveFilter.EventNames.Add(eventName.EventName.Name);
			}
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new SKDArchiveFilter());
			OnPropertyChanged(()=>EventNames);
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (StartDateTime.DateTime > EndDateTime.DateTime)
			{
				MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
				return;
			}
			Close(true);
		}
		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			Close(false);
		}
	}
}