using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class AccessDoorsSelectationViewModel : BaseViewModel
	{
		Organisation Organisation;
		public List<CardDoor> CardDoors { get; private set; }

		public AccessDoorsSelectationViewModel(Organisation organisation, List<CardDoor> cardDoors)
		{
			Organisation = organisation;
			CardDoors = cardDoors;
			if (CardDoors == null)
				CardDoors = new List<CardDoor>();

			Doors = new ObservableCollection<AccessDoorViewModel>();
			foreach (var door in SKDManager.SKDConfiguration.Doors)
			{
				if (Organisation.DoorUIDs.Any(y => y == door.UID))
				{
					var accessDoorViewModel = new AccessDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
					Doors.Add(accessDoorViewModel);
				}
			}
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				if (Organisation.DoorUIDs.Any(y => y == door.UID))
				{
					var accessDoorViewModel = new AccessDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
					Doors.Add(accessDoorViewModel);
				}
			}
			SelectedDoor = Doors.FirstOrDefault(x => x.IsChecked);
		}

		public ObservableCollection<AccessDoorViewModel> Doors { get; private set; }

		AccessDoorViewModel _selectedDoor;
		public AccessDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
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
						DoorUID = door.DoorUID,
						EnterScheduleNo = door.SelectedEnterSchedule != null ? door.SelectedEnterSchedule.ScheduleNo : 0,
						ExitScheduleNo = door.SelectedExitSchedule != null ? door.SelectedExitSchedule.ScheduleNo : 0,
					};
					CardDoors.Add(cardDoor);
				}
			}
			return CardDoors;
		}
	}
}