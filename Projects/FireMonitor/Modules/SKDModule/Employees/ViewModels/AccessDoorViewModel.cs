using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class AccessDoorViewModel : BaseViewModel
	{
		Action<AccessDoorViewModel> _onChecked;

		#region Constructors
		public AccessDoorViewModel(SKDDoor door, List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			DoorUID = door.UID;
			No = door.No;
			Name = door.Name;
			HasEnter = door.InDeviceUID != Guid.Empty;

			EnterSchedules = new ObservableCollection<CardScheduleItem>();
			foreach (var schedule in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				EnterSchedules.Add(new CardScheduleItem(schedule.ID, schedule.Name));
			}

			Initialize(cardDoors, onChecked);
		}
		#endregion

		#region Properties
		public Guid DoorUID { get; private set; }
		public int No { get; set; }
		public string Name { get; private set; }
		public List<CardDoor> CardDoors { get; private set; }
		public bool HasEnter { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (_onChecked != null)
					_onChecked(this);
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

		#endregion

		void Initialize(List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			CardDoors = cardDoors;
			_onChecked = onChecked;

			var cardDoor = CardDoors.FirstOrDefault(x => x.DoorUID == DoorUID);
			if (cardDoor != null)
			{
				_isChecked = true;
				SelectedEnterSchedule = EnterSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.EnterScheduleNo) ??
				                        EnterSchedules.FirstOrDefault();
			}
			else
			{
				SelectedEnterSchedule = EnterSchedules.FirstOrDefault();
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