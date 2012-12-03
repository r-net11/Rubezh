using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public ArchiveFilterViewModel(ArchiveFilter archiveFilter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);

			Initialize();

            StartDateTime = archiveFilter.StartDate;
            EndDateTime = DateTime.Now;

			UseSystemDate = archiveFilter.UseSystemDate;

			if (archiveFilter.Descriptions.IsNotNullOrEmpty())
			{
				JournalEvents.Where(x => archiveFilter.Descriptions.Any(description => description == x.Name)).
							  AsParallel().ForAll(x => x.IsEnable = true);
				JournalTypes.Where(x => JournalEvents.Any(journalEvent => journalEvent.ClassId == x.StateType && journalEvent.IsEnable)).
							 AsParallel().ForAll(x => x.IsEnable = true);
			}

			if (archiveFilter.DeviceNames.IsNotNullOrEmpty())
			{
				foreach (var device in Devices)
				{
					device.IsChecked = archiveFilter.DeviceNames.Any(x => x == device.DatabaseName);
				}
			}

			if (archiveFilter.Subsystems.IsNotNullOrEmpty())
			{
				foreach (var subsystem in Subsystems)
				{
					subsystem.IsEnable = archiveFilter.Subsystems.Any(x => x == subsystem.Subsystem);
				}
			}
		}

		void Initialize()
		{
			JournalEvents = new List<EventViewModel>();

			var operationResult = FiresecClient.FiresecManager.FiresecService.GetDistinctDescriptions();
			if (operationResult.HasError == false)
			{
				if (operationResult.Result.Count > 0)
				{
					foreach (var journalRecord in operationResult.Result.Take(2000))
					{
						var eventViewModel = new EventViewModel(journalRecord.StateType, journalRecord.Description);
						JournalEvents.Add(eventViewModel);
					}
				}
			}

			JournalTypes = new List<ClassViewModel>(
				JournalEvents.Select(x => x.ClassId).Distinct().
				Select(x => new ClassViewModel((StateType)x))
			);

			Devices = new List<DeviceViewModel>(
				FiresecClient.FiresecManager.Devices.Where(
				x => x.Driver.Category == DeviceCategoryType.Device).Select(x => new DeviceViewModel(x))
			);

			Subsystems = new List<SubsystemViewModel>();
			foreach (SubsystemType item in Enum.GetValues(typeof(SubsystemType)))
			{
				Subsystems.Add(new SubsystemViewModel(item));
			}
		}

		public DateTime ArchiveFirstDate
		{
			get { return ArchiveViewModel.ArchiveFirstDate; }
		}

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

		public List<ClassViewModel> JournalTypes { get; private set; }
		public List<EventViewModel> JournalEvents { get; private set; }
		public List<SubsystemViewModel> Subsystems { get; private set; }

		bool _useSystemDate;
		public bool UseSystemDate
		{
			get { return _useSystemDate; }
			set
			{
				_useSystemDate = value;
				OnPropertyChanged("UseSystemDate");
			}
		}

        DateTime _startDateTime;
        public DateTime StartDateTime
        {
            get { return _startDateTime; }
            set
            {
                _startDateTime = value;
                OnPropertyChanged("startDateTime");
            }
        }

        DateTime _endDateTime;
        public DateTime EndDateTime
        {
            get { return _endDateTime; }
            set
            {
                _endDateTime = value;
                OnPropertyChanged("endDateTime");
            }
        }

        public List<DeviceViewModel> Devices { get; private set; }

		public ArchiveFilter GetModel()
		{
			return new ArchiveFilter()
			{
                StartDate = StartDateTime,
                EndDate = EndDateTime,
                UseSystemDate = UseSystemDate,
				Descriptions = new List<string>(JournalEvents.Where(x => x.IsEnable).Select(x => x.Name)),
				DeviceNames = new List<string>(Devices.Where(x => x.IsChecked).Select(x => x.DatabaseName)),
				Subsystems = new List<SubsystemType>(Subsystems.Where(x => x.IsEnable).Select(x => x.Subsystem)),
			};
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
            StartDateTime = EndDateTime = DateTime.Now;
            StartDateTime = StartDateTime.AddDays(-1);
            UseSystemDate = false;

			JournalTypes.ForEach(x => x.IsEnable = false);
			JournalEvents.ForEach(x => x.IsEnable = false);
			Devices.ForEach(x => x.IsChecked = false);
			Subsystems.ForEach(x => x.IsEnable = false);
		}
		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (StartDateTime > EndDateTime)
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