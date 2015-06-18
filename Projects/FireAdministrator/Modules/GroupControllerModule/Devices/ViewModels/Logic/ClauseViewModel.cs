using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace GKModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<GKDevice> Devices { get; set; }
		public List<GKMPT> MPTs { get; set; }
		public List<GKDoor> Doors { get; set; }
		GKDevice Device;

		public ClauseViewModel(GKDevice device, GKClause clause)
		{
			Device = device;
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);
			SelectMPTsCommand = new RelayCommand(OnSelectMPTs);
			SelectDoorsCommand = new RelayCommand(OnSelectDoors);

			ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
			ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();
			SelectedClauseOperationType = clause.ClauseOperationType;

			Devices = clause.Devices.ToList();
			MPTs = clause.MPTs.ToList();
			Doors = clause.Doors.ToList();

			SelectedClauseConditionType = clause.ClauseConditionType;
			SelectedStateType = StateTypes.FirstOrDefault(x => x.StateBit == clause.StateType);
		}

		public List<ClauseConditionType> ClauseConditionTypes { get; private set; }
		public List<ClauseOperationType> ClauseOperationTypes { get; private set; }

		ClauseConditionType _selectedClauseConditionType;
		public ClauseConditionType SelectedClauseConditionType
		{
			get { return _selectedClauseConditionType; }
			set
			{
				_selectedClauseConditionType = value;
				OnPropertyChanged(() => SelectedClauseConditionType);
			}
		}

		ClauseOperationType _selectedClauseOperationType;
		public ClauseOperationType SelectedClauseOperationType
		{
			get { return _selectedClauseOperationType; }
			set
			{
				_selectedClauseOperationType = value;
				var oldSelectedStateType = SelectedStateType != null ? SelectedStateType.StateBit : GKStateBit.Test;

				switch (value)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						StateTypes = new ObservableCollection<StateTypeViewModel>();
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Norm));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Fire2));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Fire1));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.On));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Off));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.TurningOn));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.TurningOff));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Failure));
						break;

					case ClauseOperationType.AllMPTs:
					case ClauseOperationType.AnyMPT:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Off),
							new StateTypeViewModel(value, GKStateBit.TurningOn),
							new StateTypeViewModel(value, GKStateBit.Norm)
						};
						break;

					case ClauseOperationType.AnyDoor:
					case ClauseOperationType.AllDoors:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Fire1)
						};
						break;
				}
				if (StateTypes.Any(x => x.StateBit == oldSelectedStateType))
				{
					SelectedStateType = StateTypes.FirstOrDefault(x => x.StateBit == oldSelectedStateType);
				}
				if (SelectedStateType == null)
				{
					SelectedStateType = StateTypes.FirstOrDefault();
				}
				OnPropertyChanged(() => SelectedClauseOperationType);
				OnPropertyChanged(() => PresenrationDevices);
				OnPropertyChanged(() => PresenrationMPTs);
				OnPropertyChanged(() => PresenrationDoors);
				OnPropertyChanged(() => CanSelectDevices);
				OnPropertyChanged(() => CanSelectMPTs);
				OnPropertyChanged(() => CanSelectDoors);
			}
		}

		ObservableCollection<StateTypeViewModel> _stateTypes;
		public ObservableCollection<StateTypeViewModel> StateTypes
		{
			get { return _stateTypes; }
			set
			{
				_stateTypes = value;
				OnPropertyChanged(() => StateTypes);
			}
		}

		StateTypeViewModel _selectedStateType;
		public StateTypeViewModel SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				OnPropertyChanged(() => SelectedStateType);
			}
		}

		public string PresenrationDevices
		{
			get { return GKManager.GetCommaSeparatedDevices(Devices); }
		}

		public string PresenrationMPTs
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(MPTs), new List<ModelBase>(GKManager.MPTs)); }
		}

		public string PresenrationDoors
		{
			get
			{
				var name = GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Doors), new List<ModelBase>(GKManager.Doors));
				return name;
			}
		}

		public bool CanSelectDevices
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice); }
		}

		public bool CanSelectMPTs
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllMPTs || SelectedClauseOperationType == ClauseOperationType.AnyMPT); }
		}
		public bool CanSelectDoors
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDoors || SelectedClauseOperationType == ClauseOperationType.AnyDoor); }
		}

		public RelayCommand SelectDevicesCommand { get; private set; }
		void OnSelectDevices()
		{
			var sourceDevices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.IsNotUsed)
					continue;
				if (Device != null && Device.DriverType != GKDriverType.GKRele)
				{
					if (!device.Driver.IsDeviceOnShleif && Device.Driver.IsDeviceOnShleif)
						continue;
				}
				if (device.Driver.AvailableStateBits.Contains(SelectedStateType.StateBit))
					sourceDevices.Add(device);
			}
			var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				Devices = devicesSelectationViewModel.DevicesList;
				OnPropertyChanged(() => PresenrationDevices);
			}
		}
		public RelayCommand SelectMPTsCommand { get; private set; }
		void OnSelectMPTs()
		{
			var mptsSelectationViewModel = new MPTsSelectationViewModel(MPTs);
			if (DialogService.ShowModalWindow(mptsSelectationViewModel))
			{
				MPTs = mptsSelectationViewModel.MPTs;
				OnPropertyChanged(() => PresenrationMPTs);
			}
		}

		public RelayCommand SelectDoorsCommand { get; private set; }
		void OnSelectDoors()
		{
			var doorsSelectationViewModel = new DoorsSelectationViewModel(Doors);
			if (DialogService.ShowModalWindow(doorsSelectationViewModel))
			{
				Doors = doorsSelectationViewModel.Doors;
				OnPropertyChanged(() => PresenrationDoors);
			}
		}
	}
}