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

			foreach (var zone in Doors)
			{
				if (zone.IsChecked)
					zone.ExpandToThis();
			}
			SelectedDoor = Doors.FirstOrDefault(x => x.IsChecked);
		}

		void InitializeDoors()
		{
			Doors = new ObservableCollection<AccessDoorViewModel>();
			foreach (var door in SKDManager.SKDConfiguration.Doors)
			{
				var doorViewModel = new AccessDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
				Doors.Add(doorViewModel);
			}

			//foreach (var cardZone in CardDoors)
			//{
			//	var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == cardZone.DoorUID);
			//	if (zone != null)
			//	{
			//	}
			//}
		}

		public ObservableCollection<AccessDoorViewModel> Doors;

		AccessDoorViewModel _selectedDoor;
		public AccessDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("Selecteddoor");
			}
		}
	}
}