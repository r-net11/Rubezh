using System;
using System.Linq;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;
using GKModule.Plans.Designer;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using GKModule.Events;
using RubezhAPI.GK;

namespace GKModule.Plans.ViewModels
{
	public class GKDoorPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementGKDoor _elementGKDoor;
		DoorsViewModel _doorsViewModel;

		public GKDoorPropertiesViewModel(DoorsViewModel doorsViewModel, ElementGKDoor elementGKDoor)
		{
			Title = "Свойства фигуры: Точка доступа";
			_elementGKDoor = elementGKDoor;
			_doorsViewModel = doorsViewModel;

			GKDoors = doorsViewModel.Doors;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			if (_elementGKDoor.DoorUID != Guid.Empty)
				SelectedGKDoor = GKDoors.FirstOrDefault(x => x.Door.UID == _elementGKDoor.DoorUID);

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
				GKPlanExtension.Instance.Cache.BuildSafe<GKDoor>();
				GKPlanExtension.Instance.SetItem<GKDoor>(_elementGKDoor, createGKDoorEventArg.GKDoor);
				Update(doorUID);
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
			Guid doorUID = _elementGKDoor.DoorUID;
			GKPlanExtension.Instance.SetItem<GKDoor>(_elementGKDoor, SelectedGKDoor.Door);

			if (doorUID != _elementGKDoor.DoorUID)
				Update(doorUID);
			_doorsViewModel.SelectedDoor = Update(_elementGKDoor.DoorUID);
			return base.Save();
		}
		DoorViewModel Update(Guid doorUID)
		{
			var door = GKDoors.FirstOrDefault(x => x.Door.UID == doorUID);
			if (door != null)
				door.Update();
			return door;
		}
	}
}