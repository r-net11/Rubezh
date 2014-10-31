using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class AccessDoorViewModel : BaseViewModel
	{
		public Guid DoorUID { get; private set; }
		public string PresentationName { get; private set; }
		public List<CardDoor> CardDoors { get; private set; }
		Action<AccessDoorViewModel> OnChecked;

		public AccessDoorViewModel(SKDDoor door, List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			DoorUID = door.UID;
			PresentationName = door.PresentationName;
			HasEnter = door.InDeviceUID != Guid.Empty;
			HasExit = door.OutDeviceUID != Guid.Empty && door.DoorType == DoorType.TwoWay;

			EnterSchedules = new ObservableCollection<CardScheduleItem>();
			ExitSchedules = new ObservableCollection<CardScheduleItem>();
			foreach (var schedule in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				EnterSchedules.Add(new CardScheduleItem(schedule.ID, schedule.Name));
				ExitSchedules.Add(new CardScheduleItem(schedule.ID, schedule.Name));
			}

			Initialize(cardDoors, onChecked);
		}

		public AccessDoorViewModel(GKDoor door, List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			DoorUID = door.UID;
			PresentationName = door.PresentationName;
			HasEnter = door.EnterDeviceUID != Guid.Empty;
			HasExit = door.ExitDeviceUID != Guid.Empty && door.DoorType == GKDoorType.TwoWay;

			EnterSchedules = new ObservableCollection<CardScheduleItem>();
			ExitSchedules = new ObservableCollection<CardScheduleItem>();
			foreach (var schedule in GKManager.DeviceConfiguration.Schedules)
			{
				EnterSchedules.Add(new CardScheduleItem(schedule.No, schedule.Name));
				ExitSchedules.Add(new CardScheduleItem(schedule.No, schedule.Name));
			}

			Initialize(cardDoors, onChecked);
		}

		void Initialize(List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			CardDoors = cardDoors;
			OnChecked = onChecked;

			var cardDoor = CardDoors.FirstOrDefault(x => x.DoorUID == DoorUID);
			if (cardDoor != null)
			{
				_isChecked = true;
				SelectedEnterSchedule = EnterSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.EnterScheduleNo);
				if (SelectedEnterSchedule == null)
					SelectedEnterSchedule = EnterSchedules.FirstOrDefault();
				SelectedExitSchedule = ExitSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.ExitScheduleNo);
				if (SelectedExitSchedule == null)
					SelectedExitSchedule = ExitSchedules.FirstOrDefault();
			}
			else
			{
				SelectedEnterSchedule = EnterSchedules.FirstOrDefault();
				SelectedExitSchedule = ExitSchedules.FirstOrDefault();
			}
		}

		public bool HasEnter { get; private set; }
		public bool HasExit { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (OnChecked != null)
					OnChecked(this);
			}
		}

		ObservableCollection<CardScheduleItem> _enterSchedules;
		public ObservableCollection<CardScheduleItem> EnterSchedules
		{
			get { return _enterSchedules; }
			set
			{
				_enterSchedules = value;
				OnPropertyChanged(() => EnterSchedules);
			}
		}

		CardScheduleItem _selectedEnterSchedule;
		public CardScheduleItem SelectedEnterSchedule
		{
			get { return _selectedEnterSchedule; }
			set
			{
				_selectedEnterSchedule = value;
				OnPropertyChanged(() => SelectedEnterSchedule);
			}
		}

		ObservableCollection<CardScheduleItem> _exitSchedules;
		public ObservableCollection<CardScheduleItem> ExitSchedules
		{
			get { return _exitSchedules; }
			set
			{
				_exitSchedules = value;
				OnPropertyChanged(() => ExitSchedules);
			}
		}

		CardScheduleItem _selectedExitSchedule;
		public CardScheduleItem SelectedExitSchedule
		{
			get { return _selectedExitSchedule; }
			set
			{
				_selectedExitSchedule = value;
				OnPropertyChanged(() => SelectedExitSchedule);
			}
		}
	}

	public class CardScheduleItem
	{
		public CardScheduleItem(int scheduleNo, string name)
		{
			ScheduleNo = scheduleNo;
			Name = name;
		}

		public int ScheduleNo { get; private set; }
		public string Name { get; private set; }
	}
}