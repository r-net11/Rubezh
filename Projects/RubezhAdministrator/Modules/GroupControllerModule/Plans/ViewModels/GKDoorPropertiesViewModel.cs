using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class GKDoorPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementGKDoor _elementGKDoor;
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }
		public GKDoorPropertiesViewModel(ElementGKDoor elementGKDoor, CommonDesignerCanvas designerCanvas)
		{
			Title = "Свойства фигуры: Точка доступа";
			_elementGKDoor = elementGKDoor;
			PositionSettingsViewModel = new PositionSettingsViewModel(_elementGKDoor as ElementBase, designerCanvas);
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			GKDoors = new ObservableCollection<GKDoor>(GKManager.Doors);
			if (elementGKDoor.DoorUID != Guid.Empty)
				SelectedGKDoor = GKDoors.FirstOrDefault(door => door.UID == elementGKDoor.DoorUID);

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
			PositionSettingsViewModel.SavePosition();
			GKPlanExtension.Instance.RewriteItem(_elementGKDoor, SelectedGKDoor);
			return base.Save();
		}
	}
}