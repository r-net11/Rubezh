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
	public class AccessGKDoorViewModel : BaseViewModel
	{
		public GKDoor Door { get; private set; }
		public List<GKCardDoor> CardDoors { get; private set; }
		Action<AccessGKDoorViewModel> OnChecked;

		public AccessGKDoorViewModel(GKDoor door, List<GKCardDoor> cardDoors, Action<AccessGKDoorViewModel> onChecked)
		{
			Door = door;
			CardDoors = cardDoors;
			OnChecked = onChecked;

			if (CardDoors == null)
				CardDoors = new List<GKCardDoor>();

			EnterTimeTypes = new ObservableCollection<GKCardTimeItem>();
			foreach (var schedule in GKManager.DeviceConfiguration.Schedules)
			{
				var cardTimeItem = new GKCardTimeItem(schedule.No, schedule.Name);
				EnterTimeTypes.Add(cardTimeItem);
			}

			ExitTimeTypes = new ObservableCollection<GKCardTimeItem>();
			foreach (var schedule in GKManager.DeviceConfiguration.Schedules)
			{
				var cardTimeItem = new GKCardTimeItem(schedule.No, schedule.Name);
				ExitTimeTypes.Add(cardTimeItem);
			}

			var cardDoor = CardDoors.FirstOrDefault(x => x.DoorUID == door.UID);
			if (cardDoor != null)
			{
				_isChecked = true;
				SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault(x => x.ScheduleID == cardDoor.EnterIntervalID);
				if (SelectedEnterTimeType == null)
					SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault();
				SelectedExitTimeType = ExitTimeTypes.FirstOrDefault(x => x.ScheduleID == cardDoor.ExitIntervalID);
				if (SelectedExitTimeType == null)
					SelectedExitTimeType = ExitTimeTypes.FirstOrDefault();
			}
			else
			{
				SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault();
				SelectedExitTimeType = ExitTimeTypes.FirstOrDefault();
			}
		}

		public bool HasEnter
		{
			get { return Door.EnterDeviceUID != Guid.Empty; }
		}

		public bool HasExit
		{
			get { return Door.ExitDeviceUID != Guid.Empty && Door.DoorType == GKDoorType.TwoWay; }
		}

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

		ObservableCollection<GKCardTimeItem> _enterTimeTypes;
		public ObservableCollection<GKCardTimeItem> EnterTimeTypes
		{
			get { return _enterTimeTypes; }
			set
			{
				_enterTimeTypes = value;
				OnPropertyChanged(() => EnterTimeTypes);
			}
		}

		GKCardTimeItem _selectedEnterTimeType;
		public GKCardTimeItem SelectedEnterTimeType
		{
			get { return _selectedEnterTimeType; }
			set
			{
				_selectedEnterTimeType = value;
				OnPropertyChanged(() => SelectedEnterTimeType);
			}
		}

		ObservableCollection<GKCardTimeItem> _exitTimeTypes;
		public ObservableCollection<GKCardTimeItem> ExitTimeTypes
		{
			get { return _exitTimeTypes; }
			set
			{
				_exitTimeTypes = value;
				OnPropertyChanged(() => ExitTimeTypes);
			}
		}

		GKCardTimeItem _selectedExitTimeType;
		public GKCardTimeItem SelectedExitTimeType
		{
			get { return _selectedExitTimeType; }
			set
			{
				_selectedExitTimeType = value;
				OnPropertyChanged(() => SelectedExitTimeType);
			}
		}
	}

	public class GKCardTimeItem
	{
		public GKCardTimeItem(int scheduleID, string name)
		{
			ScheduleID = scheduleID;
			Name = name;
		}

		public int ScheduleID { get; private set; }
		public string Name { get; private set; }
	}
}