using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<XZone> Zones { get; set; }
		public List<XDevice> Devices { get; set; }
		public List<XDirection> Directions { get; set; }

		public ClauseViewModel(XClause clause)
		{
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);
			SelectDirectionCommand = new RelayCommand(OnSelectDirections);

			SelectedClauseJounOperationType = clause.ClauseJounOperationType;
			SelectedClauseOperationType = clause.ClauseOperationType;
			Zones = clause.Zones.ToList();
			Devices = clause.Devices.ToList();
			Directions = clause.Directions.ToList();

            ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
            ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();
			ClauseJounOperationTypes = Enum.GetValues(typeof(ClauseJounOperationType)).Cast<ClauseJounOperationType>().ToList();

            StateTypes = new ObservableCollection<XStateType>();
			StateTypes.Add(XStateType.Attention);
			StateTypes.Add(XStateType.Fire1);
			StateTypes.Add(XStateType.Fire2);
			StateTypes.Add(XStateType.Test);
			StateTypes.Add(XStateType.Failure);

            SelectedClauseConditionType = clause.ClauseConditionType;
			SelectedStateType = clause.StateType;
		}

        public List<ClauseConditionType> ClauseConditionTypes { get; private set; }
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

        ClauseConditionType _selectedClauseConditionType;
        public ClauseConditionType SelectedClauseConditionType
        {
            get { return _selectedClauseConditionType; }
            set
            {
                _selectedClauseConditionType = value;
                OnPropertyChanged("SelectedClauseConditionType");
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
                        Zones = new List<XZone>();
                        Directions = new List<XDirection>();
                        StateTypes = new ObservableCollection<XStateType>();
                        StateTypes.Add(XStateType.Attention);
                        StateTypes.Add(XStateType.Fire1);
                        StateTypes.Add(XStateType.Fire2);
                        StateTypes.Add(XStateType.Test);
                        StateTypes.Add(XStateType.Failure);
                        SelectedStateType = StateTypes.FirstOrDefault();
                        break;

                    case ClauseOperationType.AllZones:
                    case ClauseOperationType.AnyZone:
                        Devices = new List<XDevice>();
                        Directions = new List<XDirection>();
                        StateTypes = new ObservableCollection<XStateType>();
                        StateTypes.Add(XStateType.Attention);
                        StateTypes.Add(XStateType.Fire1);
                        StateTypes.Add(XStateType.Fire2);
                        SelectedStateType = StateTypes.FirstOrDefault();
                        break;

                    case ClauseOperationType.AllDirections:
                    case ClauseOperationType.AnyDirection:
                        Zones = new List<XZone>();
                        Devices = new List<XDevice>();
                        StateTypes = new ObservableCollection<XStateType>();
                        StateTypes.Add(XStateType.On);
                        SelectedStateType = StateTypes.FirstOrDefault();
                        break;
                }
                OnPropertyChanged("SelectedClauseOperationType");
                OnPropertyChanged("PresenrationZones");
                OnPropertyChanged("PresenrationDevices");
                OnPropertyChanged("PresenrationDirections");
                OnPropertyChanged("CanSelectZones");
                OnPropertyChanged("CanSelectDevices");
                OnPropertyChanged("CanSelectDirections");
            }
        }

        ObservableCollection<XStateType> _stateTypes;
        public ObservableCollection<XStateType> StateTypes
        {
            get { return _stateTypes; }
            set
            {
                _stateTypes = value;
                OnPropertyChanged("StateTypes");
            }
        }

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

		public string PresenrationDirections
		{
			get { return XManager.GetCommaSeparatedDirections(Directions); }
		}

		public bool CanSelectZones
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllZones || SelectedClauseOperationType == ClauseOperationType.AnyZone); }
		}

		public bool CanSelectDevices
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice); }
		}

		public bool CanSelectDirections
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDirections || SelectedClauseOperationType == ClauseOperationType.AnyDirection); }
		}

		public RelayCommand SelectZonesCommand { get; private set; }
		void OnSelectZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
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

		public RelayCommand SelectDirectionCommand { get; private set; }
		void OnSelectDirections()
		{
			var directionsSelectationViewModel = new DirectionsSelectationViewModel(Directions);
			if (DialogService.ShowModalWindow(directionsSelectationViewModel))
			{
				Directions = directionsSelectationViewModel.Directions;
				OnPropertyChanged("PresenrationDirections");
			}
		}
	}
}