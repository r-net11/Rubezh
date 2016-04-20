using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class GKDoorSelectionViewModel : SaveCancelDialogViewModel
	{
		public GKDoorSelectionViewModel(GKDoor door)
		{
			Title = "Выбор точки доступа";
			Doors = new ObservableCollection<DoorViewModel>();
			GKManager.Doors.ForEach(x => Doors.Add(new DoorViewModel(x)));
			if (door != null)
				SelectedDoor = Doors.FirstOrDefault(x => x.Door.UID == door.UID);
			if (SelectedDoor == null)
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
	}
}
