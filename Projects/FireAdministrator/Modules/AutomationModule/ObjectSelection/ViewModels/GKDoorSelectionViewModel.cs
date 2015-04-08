using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GKDoorSelectionViewModel : SaveCancelDialogViewModel
	{
		public GKDoorSelectionViewModel(GKDoor door)
		{
			Title = "Выбор точки доступа";
			Doors = new ObservableCollection<DoorViewModel>();
			GKManager.Doors.ForEach(x => Doors.Add(new DoorViewModel(x)));
			if (door != null)
				SelectedDoor = Doors.FirstOrDefault(x => x.GKDoor.UID == door.UID);
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
