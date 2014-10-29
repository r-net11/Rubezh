using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessDoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
		public List<CardDoor> CardDoors { get; private set; }
		Action<AccessDoorViewModel> OnChecked;

		public AccessDoorViewModel(SKDDoor door, List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			Door = door;
			CardDoors = cardDoors;
			OnChecked = onChecked;

			if (CardDoors == null)
				CardDoors = new List<CardDoor>();

			EnterTimeTypes = new ObservableCollection<CardTimeItem>();
			ExitTimeTypes = new ObservableCollection<CardTimeItem>();
			foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				EnterTimeTypes.Add(new CardTimeItem(interval.ID, interval.Name));
				ExitTimeTypes.Add(new CardTimeItem(interval.ID, interval.Name));
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
			get { return Door.InDeviceUID != Guid.Empty; }
		}

		public bool HasExit
		{
			get { return Door.OutDeviceUID != Guid.Empty && Door.DoorType == DoorType.TwoWay; }
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

		ObservableCollection<CardTimeItem> _enterTimeTypes;
		public ObservableCollection<CardTimeItem> EnterTimeTypes
		{
			get { return _enterTimeTypes; }
			set
			{
				_enterTimeTypes = value;
				OnPropertyChanged(() => EnterTimeTypes);
			}
		}

		CardTimeItem _selectedEnterTimeType;
		public CardTimeItem SelectedEnterTimeType
		{
			get { return _selectedEnterTimeType; }
			set
			{
				_selectedEnterTimeType = value;
				OnPropertyChanged(() => SelectedEnterTimeType);
			}
		}

		ObservableCollection<CardTimeItem> _exitTimeTypes;
		public ObservableCollection<CardTimeItem> ExitTimeTypes
		{
			get { return _exitTimeTypes; }
			set
			{
				_exitTimeTypes = value;
				OnPropertyChanged(() => ExitTimeTypes);
			}
		}

		CardTimeItem _selectedExitTimeType;
		public CardTimeItem SelectedExitTimeType
		{
			get { return _selectedExitTimeType; }
			set
			{
				_selectedExitTimeType = value;
				OnPropertyChanged(() => SelectedExitTimeType);
			}
		}
	}

	public class CardTimeItem
	{
		public CardTimeItem(int scheduleID, string name)
		{
			ScheduleID = scheduleID;
			Name = name;
		}

		public int ScheduleID { get; private set; }
		public string Name { get; private set; }
	}
}