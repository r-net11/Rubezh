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
		public List<XZone> Zones { get; set; }
		public List<XDevice> Devices { get; set; }

		public ClauseViewModel(XClause clause)
		{
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);

			SelectedStateType = clause.StateType;
			SelectedClauseJounOperationType = clause.ClauseJounOperationType;
			SelectedClauseOperationType = clause.ClauseOperationType;
			Zones = clause.Zones.ToList();
			Devices = clause.Devices.ToList();

            ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
            ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();
			ClauseJounOperationTypes = Enum.GetValues(typeof(ClauseJounOperationType)).Cast<ClauseJounOperationType>().ToList();

			StateTypes = new List<XStateType>();
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
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						Devices = new List<XDevice>();
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
	}
}