using System.Collections.ObjectModel;
using System.Linq;
using Localization.Automation.ViewModels;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DoorSelectionViewModel : SaveCancelDialogViewModel
	{
		public DoorSelectionViewModel(SKDDoor door)
		{
			Title = CommonViewModels.ChooseDoor;
			Doors = new ObservableCollection<DoorViewModel>();
			SKDManager.Doors.ForEach(x => Doors.Add(new DoorViewModel(x)));
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
