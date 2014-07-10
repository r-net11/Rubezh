using System;
using FiresecAPI.Journal;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public FilterNamesViewModel FilterNamesViewModel { get; private set; }
		public FilterObjectsViewModel FilterObjectsViewModel { get; private set; }

		public ArchiveFilterViewModel(ArchiveFilter archiveFilter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);
			FilterNamesViewModel = new FilterNamesViewModel(archiveFilter);
			FilterObjectsViewModel = new FilterObjectsViewModel(archiveFilter);
			Initialize(archiveFilter);
		}

		void Initialize(ArchiveFilter archiveFilter)
		{
			StartDateTime = new DateTimePairViewModel(archiveFilter.StartDate);
			EndDateTime = new DateTimePairViewModel(archiveFilter.EndDate);
			FilterNamesViewModel.Initialize(archiveFilter);
			FilterObjectsViewModel.Initialize(archiveFilter);
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

		public ArchiveFilter GetModel()
		{
			var archiveFilter = FilterNamesViewModel.GetModel();
			archiveFilter.StartDate = StartDateTime.DateTime;
			archiveFilter.EndDate = EndDateTime.DateTime;
			archiveFilter.UseDeviceDateTime = UseDeviceDateTime;

			var objectsFilter = FilterObjectsViewModel.GetModel();
			foreach (var journalSubsystemTypes in objectsFilter.JournalSubsystemTypes)
			{
				if (!archiveFilter.JournalSubsystemTypes.Contains(journalSubsystemTypes))
					archiveFilter.JournalSubsystemTypes.Add(journalSubsystemTypes);
			}
			archiveFilter.JournalObjectTypes = objectsFilter.JournalObjectTypes;
			archiveFilter.ObjectUIDs = objectsFilter.ObjectUIDs;
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new ArchiveFilter());
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