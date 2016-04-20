using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using RubezhAPI.Models.Layouts;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DoorsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public DoorsViewModel()
		{
			IsVisibleBottomPanel = true;
		}
		public void Initialize()
		{
			Doors = new ObservableCollection<DoorViewModel>();
			foreach (var door in GKManager.DeviceConfiguration.Doors.OrderBy(x => x.No))
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
		LayoutPartAdditionalProperties _properties;
		public LayoutPartAdditionalProperties Properties
		{
			get { return _properties; }
			set
			{
				_properties = value;
				IsVisibleBottomPanel = (_properties != null) ? _properties.IsVisibleBottomPanel : false;
			}
		}
		bool _isVisibleBottomPanel;
		public bool IsVisibleBottomPanel
		{
			get { return _isVisibleBottomPanel; }
			set
			{
				_isVisibleBottomPanel = value;
				OnPropertyChanged(() => IsVisibleBottomPanel);
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