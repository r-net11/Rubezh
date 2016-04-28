using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.ViewModels
{
	public class SKDZoneViewModel : BaseViewModel
	{
		public GKSKDZone Zone { get; private set; }

		public SKDZoneViewModel(GKSKDZone zone)
		{
			ManageOutputDoorsCommand = new RelayCommand(OnManageOutputDoors);
			ManageInputDoorsCommand = new RelayCommand(OnManageInputDoors);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			OutputDoors = new ObservableCollection<DoorSkdZoneViewModel>();
			InputDoors = new ObservableCollection<DoorSkdZoneViewModel>();
			Zone = zone;
			zone.PlanElementUIDsChanged += Update;
			Update();
		}
		ObservableCollection<DoorSkdZoneViewModel> _outputDoors;
		public ObservableCollection<DoorSkdZoneViewModel> OutputDoors
		{
			get { return _outputDoors; }
			set
			{
				_outputDoors = value;
				OnPropertyChanged(() => OutputDoors);
			}
		}
		ObservableCollection<DoorSkdZoneViewModel> _inputDoors;
		public ObservableCollection<DoorSkdZoneViewModel> InputDoors
		{
			get { return _inputDoors; }
			set
			{
				_inputDoors = value;
				OnPropertyChanged(() => InputDoors);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			_visualizationState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);

			OutputDoors = new ObservableCollection<DoorSkdZoneViewModel>(GKManager.Doors.Where(x => x.EnterZoneUID == Zone.UID).Select(x => new DoorSkdZoneViewModel(x, this)));
			InputDoors = new ObservableCollection<DoorSkdZoneViewModel>(GKManager.Doors.Where(x => x.ExitZoneUID == Zone.UID).Select(x => new DoorSkdZoneViewModel(x, this)));
		}
		public RelayCommand ManageOutputDoorsCommand { get; private set; }
		void OnManageOutputDoors()
		{
			var beforeDoors = new List<GKDoor>(OutputDoors.Select(x => x.Door));
			var doorsSelectationViewModel = new DoorsSelectationViewModel(beforeDoors);
			if (DialogService.ShowModalWindow(doorsSelectationViewModel))
			{
				var afterDoors = doorsSelectationViewModel.Doors;
				foreach (var door in beforeDoors)
				{
					if (!afterDoors.Contains(door))
						door.EnterZoneUID = Guid.Empty;
				}
				afterDoors.ForEach(door => door.EnterZoneUID = Zone.UID);
				ServiceFactory.SaveService.GKChanged = true;
			}
			Update();
		}
		public RelayCommand ManageInputDoorsCommand { get; private set; }
		void OnManageInputDoors()
		{
			var beforeDoors = new List<GKDoor>(InputDoors.Select(x => x.Door));
			var doorsTypes = new List<GKDoorType> { GKDoorType.AirlockBooth, GKDoorType.Barrier, GKDoorType.Turnstile, GKDoorType.TwoWay };
			var doorsSelectationViewModel = new DoorsSelectationViewModel(beforeDoors, doorsTypes);
			if (DialogService.ShowModalWindow(doorsSelectationViewModel))
			{
				var afterDoors = doorsSelectationViewModel.Doors;
				foreach (var door in beforeDoors)
				{
					if (!afterDoors.Contains(door))
						door.ExitZoneUID = Guid.Empty;
				}
				afterDoors.ForEach(door => door.ExitZoneUID = Zone.UID);
				ServiceFactory.SaveService.GKChanged = true;
			}
			Update();
		}
		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs.Count > 0; }
		}
		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Zone.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Zone.PlanElementUIDs);
		}

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
		}
	}
}