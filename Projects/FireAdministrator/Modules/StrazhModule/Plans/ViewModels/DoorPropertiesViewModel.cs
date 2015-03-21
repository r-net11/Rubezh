﻿using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Plans.Designer;
using StrazhModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using StrazhModule.Events;

namespace StrazhModule.Plans.ViewModels
{
	public class DoorPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementDoor _elementDoor;
		private DoorsViewModel _doorsViewModel;

		public DoorPropertiesViewModel(DoorsViewModel doorsViewModel, ElementDoor elementDoor)
		{
			Title = "Свойства фигуры: Точка доступа";
			_elementDoor = elementDoor;
			_doorsViewModel = doorsViewModel;

			Doors = doorsViewModel.Doors;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			if (_elementDoor.DoorUID != Guid.Empty)
				SelectedDoor = Doors.FirstOrDefault(x => x.Door.UID == _elementDoor.DoorUID);

		}

		public ObservableCollection<DoorViewModel> Doors { get; private set; }

		private DoorViewModel _selectedDoor;
		public DoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid doorUID = _elementDoor.DoorUID;
			var createDoorEventArg = new CreateDoorEventArg();
			ServiceFactory.Events.GetEvent<CreateDoorEvent>().Publish(createDoorEventArg);
			if (createDoorEventArg.Door != null)
			{
				SKDPlanExtension.Instance.Cache.BuildSafe<SKDDoor>();
				SKDPlanExtension.Instance.SetItem<SKDDoor>(_elementDoor, createDoorEventArg.Door);
				Update(doorUID);
				Close(true);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditDoorEvent>().Publish(SelectedDoor.Door.UID);
			SelectedDoor.Update(SelectedDoor.Door);
		}
		private bool CanEdit()
		{
			return SelectedDoor != null;
		}

		protected override bool Save()
		{
			Guid doorUID = _elementDoor.DoorUID;
			SKDPlanExtension.Instance.SetItem<SKDDoor>(_elementDoor, SelectedDoor.Door);

			if (doorUID != _elementDoor.DoorUID)
				Update(doorUID);
			_doorsViewModel.SelectedDoor = Update(_elementDoor.DoorUID);
			return base.Save();
		}
		private DoorViewModel Update(Guid doorUID)
		{
			var door = Doors.FirstOrDefault(x => x.Door.UID == doorUID);
			if (door != null)
				door.Update();
			return door;
		}
	}
}