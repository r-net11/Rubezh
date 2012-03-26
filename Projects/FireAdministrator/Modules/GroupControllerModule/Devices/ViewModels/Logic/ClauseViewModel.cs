using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupControllerModule.Models;
using Infrastructure;
using Infrastructure.Common;

namespace GroupControllerModule.ViewModels
{
    public class ClauseViewModel : BaseViewModel
    {
        public List<short> Zones { get; set; }
        public List<Guid> Devices { get; set; }
        StateLogicViewModel _stateLogicViewModel;

        public ClauseViewModel(StateLogicViewModel stateLogicViewModel, XClause clause)
        {
            SelectZonesCommand = new RelayCommand(OnSelectZones);
            SelectDevicesCommand = new RelayCommand(OnSelectDevices);

            _stateLogicViewModel = stateLogicViewModel;
            SelectedClauseJounOperationType = clause.ClauseJounOperationType;
            SelectedClauseOperandType = clause.ClauseOperandType;
            SelectedClauseOperationType = clause.ClauseOperationType;
            Zones = clause.Zones.ToList();
            Devices = clause.Devices.ToList();

            ClauseJounOperationTypes = Enum.GetValues(typeof(ClauseJounOperationType)).Cast<ClauseJounOperationType>().ToList();
            ClauseOperandTypes = Enum.GetValues(typeof(ClauseOperandType)).Cast<ClauseOperandType>().ToList();
            ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();
        }

        public List<ClauseJounOperationType> ClauseJounOperationTypes { get; private set; }
        public List<ClauseOperandType> ClauseOperandTypes { get; private set; }
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

        ClauseOperandType _selectedClauseOperandType;
        public ClauseOperandType SelectedClauseOperandType
        {
            get { return _selectedClauseOperandType; }
            set
            {
                _selectedClauseOperandType = value;
                OnPropertyChanged("SelectedClauseOperandType");

                switch(value)
                {
                    case ClauseOperandType.Device:
                        Zones = new List<short>();
                        break;

                    case ClauseOperandType.Zone:
                        Devices = new List<Guid>();
                        break;
                }
                OnPropertyChanged("PresenrationZones");
                OnPropertyChanged("PresenrationDevices");
                OnPropertyChanged("CanSelectZones");
                OnPropertyChanged("CanSelectDevices");
            }
        }

        ClauseOperationType _selectedClauseOperationType;
        public ClauseOperationType SelectedClauseOperationType
        {
            get { return _selectedClauseOperationType; }
            set
            {
                _selectedClauseOperationType = value;
                OnPropertyChanged("SelectedClauseOperationType");
            }
        }

        public string PresenrationZones
        {
            get
            {
                var presenrationZones = new StringBuilder();
                for (int i = 0; i < Zones.Count; i++)
                {
                    if (i > 0)
                        presenrationZones.Append(", ");
                    var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Zones[i]);
                    if (zone != null)
                        presenrationZones.Append(zone.PresentationName);
                }

                return presenrationZones.ToString();
            }
        }

        public string PresenrationDevices
        {
            get
            {
                var presenrationDevices = new StringBuilder();
                for (int i = 0; i < Devices.Count; i++)
                {
                    if (i > 0)
                        presenrationDevices.Append(", ");
                    var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == Devices[i]);
                    if (device != null)
                        presenrationDevices.Append(device.Driver.ShortName + " - " + device.Address);
                }

                return presenrationDevices.ToString();
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

        public bool CanSelectZones
        {
            get
            {
                return SelectedClauseOperandType == ClauseOperandType.Zone;
            }
        }

        public bool CanSelectDevices
        {
            get
            {
                return SelectedClauseOperandType == ClauseOperandType.Device;
            }
        }

        public RelayCommand SelectZonesCommand { get; private set; }
        void OnSelectZones()
        {
            var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zonesSelectationViewModel))
            {
                Zones = zonesSelectationViewModel.Zones;
                OnPropertyChanged("PresenrationZones");
            }
        }

        public RelayCommand SelectDevicesCommand { get; private set; }
        void OnSelectDevices()
        {
            var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices);
            if (ServiceFactory.UserDialogs.ShowModalWindow(devicesSelectationViewModel))
            {
                Devices = devicesSelectationViewModel.DevicesList;
                OnPropertyChanged("PresenrationDevices");
            }
        }
    }
}