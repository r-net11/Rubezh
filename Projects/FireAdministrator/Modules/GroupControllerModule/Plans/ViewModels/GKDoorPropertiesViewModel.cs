using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class GKDoorPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementGKDoor _elementGKDoor;

		public GKDoorPropertiesViewModel(ElementGKDoor elementGKDoor)
		{
			Title = "Свойства фигуры: Точка доступа";
			_elementGKDoor = elementGKDoor;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			GKDoors = new ObservableCollection<DoorViewModel>(GKManager.Doors.Select(door => new DoorViewModel(door)));
			if (elementGKDoor.DoorUID != Guid.Empty)
				SelectedGKDoor = GKDoors.FirstOrDefault(door => door.Door.UID == elementGKDoor.DoorUID);

		}

		public ObservableCollection<DoorViewModel> GKDoors { get; private set; }

		DoorViewModel _selectedGKDoor;
		public DoorViewModel SelectedGKDoor
		{
			get { return _selectedGKDoor; }
			set
			{
				_selectedGKDoor = value;
				OnPropertyChanged(() => SelectedGKDoor);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		void OnCreate()
		{
			var doorUID = _elementGKDoor.DoorUID;
			var createGKDoorEventArg = new CreateGKDoorEventArg();
			ServiceFactory.Events.GetEvent<CreateGKDoorEvent>().Publish(createGKDoorEventArg);
			if (createGKDoorEventArg.GKDoor != null)
			{
				GKPlanExtension.Instance.RewriteItem(_elementGKDoor, createGKDoorEventArg.GKDoor);
				Close(true);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Publish(SelectedGKDoor.Door.UID);
			SelectedGKDoor.Update();
		}
		bool CanEdit()
		{
			return SelectedGKDoor != null;
		}
		protected override bool Save()
		{
			GKPlanExtension.Instance.RewriteItem(_elementGKDoor, SelectedGKDoor.Door);
			return base.Save();
		}
	}
}