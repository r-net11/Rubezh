using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class GKDoorPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		ElementGKDoor _elementGKDoor;
		public GKDoorPropertiesViewModel(ElementGKDoor elementGKDoor)
		{
			Title = "Свойства фигуры: Точка доступа";
			_elementGKDoor = elementGKDoor;
			Left = (int)(_elementGKDoor.Left * _sensivityFactor);
			Top = (int)(_elementGKDoor.Top * _sensivityFactor);
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			GKDoors = new ObservableCollection<GKDoor>(GKManager.Doors);
			if (elementGKDoor.DoorUID != Guid.Empty)
				SelectedGKDoor = GKDoors.FirstOrDefault(door => door.UID == elementGKDoor.DoorUID);

		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		public ObservableCollection<GKDoor> GKDoors { get; private set; }

		GKDoor _selectedGKDoor;
		public GKDoor SelectedGKDoor
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
			ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Publish(SelectedGKDoor.UID);
			GKDoors = new ObservableCollection<GKDoor>(GKManager.Doors);
			OnPropertyChanged(() => GKDoors);
		}
		bool CanEdit()
		{
			return SelectedGKDoor != null;
		}
		protected override bool Save()
		{
			_elementGKDoor.Left = (double)Left / _sensivityFactor;
			_elementGKDoor.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(_elementGKDoor, SelectedGKDoor);
			return base.Save();
		}
	}
}