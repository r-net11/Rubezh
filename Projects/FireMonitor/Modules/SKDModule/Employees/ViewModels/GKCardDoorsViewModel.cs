using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class GKCardDoorsViewModel : BaseViewModel
	{
		public List<GKCardDoor> CardDoors { get; private set; }

		public GKCardDoorsViewModel(List<GKCardDoor> cardDoors)
		{
			Update(cardDoors);
		}

		public void Update(List<GKCardDoor> cardDoors)
		{
			CardDoors = cardDoors;
			InitializeDoors();
			SelectedDoor = Doors.FirstOrDefault(x => x.IsChecked);
		}

		void InitializeDoors()
		{
			Doors = new ObservableCollection<AccessGKDoorViewModel>();
			foreach (var cardDoor in CardDoors)
			{
				var door = GKManager.DeviceConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (door != null)
				{
					var doorViewModel = new AccessGKDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
					Doors.Add(doorViewModel);
				}
			}
		}

		ObservableCollection<AccessGKDoorViewModel> _doors;
		public ObservableCollection<AccessGKDoorViewModel> Doors
		{
			get { return _doors; }
			set
			{
				_doors = value;
				OnPropertyChanged(() => Doors);
			}
		}

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
	}
}