using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessDoorsSelectationViewModel : BaseViewModel
	{
		Organisation Organisation;
		public List<CardDoor> CardDoors { get; private set; }
		Guid? ParentUID;

		public AccessDoorsSelectationViewModel(Organisation organisation, List<CardDoor> cardDoors, Guid? parentUID)
		{
			Organisation = organisation;
			CardDoors = cardDoors;

			InitializeDoors();
			ParentUID = parentUID;
		}

		void InitializeDoors()
		{
			Doors = new ObservableCollection<AccessDoorViewModel>();
			var organisationDoors = SKDManager.SKDConfiguration.Doors.Where(x => Organisation.DoorUIDs.Any(y => y == x.UID));
			foreach (var door in organisationDoors)
			{
				var accessDoorViewModel = new AccessDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
				Doors.Add(accessDoorViewModel);
			}
		}

		public ObservableCollection<AccessDoorViewModel> Doors { get; private set; }

		AccessDoorViewModel _selectedDoor;
		public AccessDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged("SelectedDoor");
			}
		}

		public List<CardDoor> GetCardDoors()
		{
			CardDoors = new List<CardDoor>();
			foreach (var door in Doors)
			{
				if (door.IsChecked)
				{
					var cardDoor = new CardDoor()
					{
						DoorUID = door.Door.UID,
						IsAntiPassback = door.IsAntiPassback,
						IsComission = door.IsComission,
						EnterIntervalType = door.SelectedEnterTimeCreteria.IntervalType,
						EnterIntervalID = door.SelectedEnterTimeType != null ? door.SelectedEnterTimeType.ScheduleID : 0,
						ExitIntervalType = door.SelectedExitTimeCreteria.IntervalType,
						ExitIntervalID = door.SelectedExitTimeType != null ? door.SelectedExitTimeType.ScheduleID : 0,
						ParentUID = ParentUID
					};
					CardDoors.Add(cardDoor);
				}
			}
			return CardDoors;
		}
	}
}