using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlDoorStepViewModel : BaseViewModel, IStepViewModel
	{
		ControlDoorArguments ControlDoorArguments { get; set; }
		public ControlDoorStepViewModel(ControlDoorArguments controlDoorArguments)
		{
			ControlDoorArguments = controlDoorArguments;
			Commands = new ObservableCollection<DoorCommandType>
			{
				DoorCommandType.Open, DoorCommandType.Close
			};
			OnPropertyChanged(() => Commands);
			SelectDoorCommand = new RelayCommand(OnSelectDoor);
			UpdateContent();
		}

		public ObservableCollection<DoorCommandType> Commands { get; private set; }

		DoorCommandType _selectedCommand;
		public DoorCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDoorArguments.DoorCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		DoorViewModel _selectedDoor;
		public DoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				ControlDoorArguments.DoorUid = Guid.Empty;
				if (_selectedDoor != null)
				{
					ControlDoorArguments.DoorUid = _selectedDoor.Door.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDoor);
			}
		}
		
		public RelayCommand SelectDoorCommand { get; private set; }
		private void OnSelectDoor()
		{
			var doorSelectationViewModel = new DoorSelectionViewModel(SelectedDoor != null ? SelectedDoor.Door : null);
			if (DialogService.ShowModalWindow(doorSelectationViewModel))
			{
				SelectedDoor = doorSelectationViewModel.SelectedDoor;
			}
		}

		public void UpdateContent()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			if (ControlDoorArguments.DoorUid != Guid.Empty)
			{
				var door = SKDManager.Doors.FirstOrDefault(x => x.UID == ControlDoorArguments.DoorUid);
				SelectedDoor = door != null ? new DoorViewModel(door) : null;
				SelectedCommand = ControlDoorArguments.DoorCommandType;
			}
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
