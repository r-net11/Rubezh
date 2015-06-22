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
