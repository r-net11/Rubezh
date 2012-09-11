using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<Guid> Zones { get; set; }
		public List<Guid> Devices { get; set; }
		StateLogicViewModel _stateLogicViewModel;

		public ClauseViewModel(StateLogicViewModel stateLogicViewModel, XClause clause)
		{
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);

			_stateLogicViewModel = stateLogicViewModel;
			SelectedStateType = clause.StateType;
			SelectedClauseJounOperationType = clause.ClauseJounOperationType;
			SelectedClauseOperationType = clause.ClauseOperationType;
			Zones = clause.Zones.ToList();
			Devices = clause.Devices.ToList();

			ClauseJounOperationTypes = Enum.GetValues(typeof(ClauseJounOperationType)).Cast<ClauseJounOperationType>().ToList();
			ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();

			StateTypes = new List<XStateType>();
			StateTypes.Add(XStateType.Attention);
			StateTypes.Add(XStateType.Fire1);
			StateTypes.Add(XStateType.Fire2);
			StateTypes.Add(XStateType.Test);
			StateTypes.Add(XStateType.Failure);

			SelectedStateType = clause.StateType;
		}

		public List<ClauseJounOperationType> ClauseJounOperationTypes { get; private set; }
		public List<ClauseOperationType> ClauseOperationTypes { get; private set; }

		ClauseJounOperationType _selectedClauseJounOperationType;
		public ClauseJounOperationType SelectedClauseJounOperationType
		{
			get { return _selectedClauseJounOperationType; }
			set
			{
				_selectedClauseJounOperationType = value;
				OnPropertyChanged("SelectedClauseJounOperationType");
			}
		}

		ClauseOperationType _selectedClauseOperationType;
		public ClauseOperationType SelectedClauseOperationType
		{
			get { return _selectedClauseOperationType; }
			set
			{
				_selectedClauseOperationType = value;

				switch (value)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						Zones = new List<Guid>();
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						Devices = new List<Guid>();
						break;
				}
				OnPropertyChanged("SelectedClauseOperationType");
				OnPropertyChanged("PresenrationZones");
				OnPropertyChanged("PresenrationDevices");
				OnPropertyChanged("CanSelectZones");
				OnPropertyChanged("CanSelectDevices");
			}
		}

		public List<XStateType> StateTypes { get; private set; }

		XStateType _selectedStateType;
		public XStateType SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				OnPropertyChanged("SelectedStateType");
			}
		}

		bool _showJoinOperator;
		public bool ShowJoinOperator
		{
			get { return _showJoinOperator; }
			set
			{
				_showJoinOperator = value;
				OnPropertyChanged("ShowJoinOperator");
			}
		}

		public string PresenrationZones
		{
			get { return XManager.GetCommaSeparatedZones(Zones); }
		}

		public string PresenrationDevices
		{
			get { return XManager.GetCommaSeparatedDevices(Devices); }
		}

		public bool CanSelectZones
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllZones || SelectedClauseOperationType == ClauseOperationType.AnyZone); }
		}

		public bool CanSelectDevices
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice); }
		}

		public RelayCommand SelectZonesCommand { get; private set; }
		void OnSelectZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(_stateLogicViewModel._deviceDetailsViewModel.Device, Zones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Zones = zonesSelectationViewModel.Zones;
				OnPropertyChanged("PresenrationZones");
			}
		}

		public RelayCommand SelectDevicesCommand { get; private set; }
		void OnSelectDevices()
		{
			var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				Devices = devicesSelectationViewModel.DevicesList;
				OnPropertyChanged("PresenrationDevices");
			}
		}
	}
}