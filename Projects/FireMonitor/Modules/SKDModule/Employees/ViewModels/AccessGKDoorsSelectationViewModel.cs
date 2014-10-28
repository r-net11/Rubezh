using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class AccessGKDoorsSelectationViewModel : BaseViewModel
	{
		Organisation Organisation;
		public List<GKCardDoor> CardDoors { get; private set; }

		public AccessGKDoorsSelectationViewModel(Organisation organisation, List<GKCardDoor> cardDoors)
		{
			Organisation = organisation;
			CardDoors = cardDoors;

			Doors = new ObservableCollection<AccessGKDoorViewModel>();
			var organisationDoors = GKManager.DeviceConfiguration.Doors.Where(x => Organisation.GKDoorUIDs.Any(y => y == x.UID));
			foreach (var door in organisationDoors)
			{
				var accessDoorViewModel = new AccessGKDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
				Doors.Add(accessDoorViewModel);
			}
			SelectedDoor = Doors.FirstOrDefault(x => x.IsChecked);
		}

		public ObservableCollection<AccessGKDoorViewModel> Doors { get; private set; }

		AccessGKDoorViewModel _selectedDoor;
		public AccessGKDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
			}
		}

		public List<GKCardDoor> GetCardDoors()
		{
			CardDoors = new List<GKCardDoor>();
			foreach (var door in Doors)
			{
				if (door.IsChecked)
				{
					var cardDoor = new GKCardDoor()
					{
						DoorUID = door.Door.UID,
						EnterIntervalID = door.SelectedEnterTimeType != null ? door.SelectedEnterTimeType.ScheduleID : 0,
						ExitIntervalID = door.SelectedExitTimeType != null ? door.SelectedExitTimeType.ScheduleID : 0,
					};
					CardDoors.Add(cardDoor);
				}
			}
			return CardDoors;
		}
	}
}