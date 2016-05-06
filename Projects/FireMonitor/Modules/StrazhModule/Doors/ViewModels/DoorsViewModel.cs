using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Doors = new ObservableCollection<DoorViewModel>();
			foreach (var door in SKDManager.Doors.OrderBy(x => x.No))
			{
				var doorViewModel = new DoorViewModel(door);
				Doors.Add(doorViewModel);
			}
			SelectedDoor = Doors.FirstOrDefault();
		}

		ObservableCollection<DoorViewModel> _doors;
		public ObservableCollection<DoorViewModel> Doors
		{
			get { return _doors; }
			set
			{
				_doors = value;
				OnPropertyChanged(() => Doors);
			}
		}

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