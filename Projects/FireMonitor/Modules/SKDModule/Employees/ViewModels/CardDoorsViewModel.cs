using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardDoorsViewModel : BaseViewModel
	{
		public List<CardDoor> CardDoors { get; private set; }

		public CardDoorsViewModel(List<CardDoor> cardDoors)
		{
			Update(cardDoors);
		}

		public void Update(List<CardDoor> cardDoors)
		{
			CardDoors = cardDoors;
			InitializeDoors();
			SelectedDoor = Doors.FirstOrDefault();
		}

		void InitializeDoors()
		{
			Doors = new ObservableCollection<ReadOnlyAccessDoorViewModel>();
			foreach (var cardDoor in CardDoors)
			{
				var door = SKDManager.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (door != null)
				{
					var doorViewModel = new ReadOnlyAccessDoorViewModel(door, cardDoor);
					Doors.Add(doorViewModel);
				}
			}
		}

		ObservableCollection<ReadOnlyAccessDoorViewModel> _doors;
		public ObservableCollection<ReadOnlyAccessDoorViewModel> Doors
		{
			get { return _doors; }
			set
			{
				_doors = value;
				OnPropertyChanged(() => Doors);
			}
		}

		ReadOnlyAccessDoorViewModel _selectedDoor;
		public ReadOnlyAccessDoorViewModel SelectedDoor
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