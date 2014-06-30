using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DoorsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static DoorsViewModel Current { get; private set; }
		public DoorsViewModel()
		{
			Current = this;
		}

		public void Initialize()
		{
			Doors = new ObservableCollection<DoorViewModel>();
			foreach (var door in SKDManager.Doors)
			{
				var doorViewModel = new DoorViewModel(door);
				Doors.Add(doorViewModel);
			}
			SelectedDoor = Doors.FirstOrDefault();
		}

		public ObservableCollection<DoorViewModel> Doors { get; private set; }

		DoorViewModel _selectedDoor;
		public DoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
			}
		}

		public void Select(Guid doorUID)
		{
			if (doorUID != Guid.Empty)
			{
				SelectedDoor = Doors.FirstOrDefault(x => x.Door.UID == doorUID);
			}
		}
	}
}