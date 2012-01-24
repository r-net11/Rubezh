using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevicesModule.Zones.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ClauseViewModel : BaseViewModel
    {
        public List<ulong?> Zones { get; set; }
        Device _device;

        public ClauseViewModel(Device device, Clause clause)
        {
            ShowZonesCommand = new RelayCommand(OnShowZones);
            SelectDeviceCommand = new RelayCommand(OnSelectDevice);

            _device = device;
            Zones = clause.Zones.ToList();
            _selectedState = clause.State;
            SelectedOperation = clause.Operation;

            if (clause.DeviceUID != Guid.Empty)
                SelectedDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
        }

        public List<ZoneLogicState> States
        {
            get
            {
                switch (_device.Driver.DriverType)
                {
                    case DriverType.Valve:
                    case DriverType.PumpStation:
                    case DriverType.MDU:
                    case DriverType.MRO:
                        return new List<ZoneLogicState>() {
                            ZoneLogicState.Fire,
                            ZoneLogicState.Attention,
                            ZoneLogicState.MPTAutomaticOn,
                            ZoneLogicState.MPTOn};

                    case DriverType.RM_1:
                        var states = Enum.GetValues(typeof(ZoneLogicState)).Cast<ZoneLogicState>().ToList();
                        states.Remove(ZoneLogicState.Failure);
                        return states;
                }
                return Enum.GetValues(typeof(ZoneLogicState)).Cast<ZoneLogicState>().ToList();
            }
        }

        public List<ZoneLogicOperation> Operations
        {
            get
            {
                var operations = new List<ZoneLogicOperation>();
                operations.Add(ZoneLogicOperation.All);
                operations.Add(ZoneLogicOperation.Any);

                return operations;
            }
        }

        public bool CanSelectOperation
        {
            get
            {
                return (SelectedState == ZoneLogicState.Fire) ||
                    (SelectedState == ZoneLogicState.Attention) ||
                    (SelectedState == ZoneLogicState.MPTAutomaticOn) ||
                    (SelectedState == ZoneLogicState.MPTOn) ||
                    (SelectedState == ZoneLogicState.Alarm) ||
                    (SelectedState == ZoneLogicState.GuardSet) ||
                    (SelectedState == ZoneLogicState.GuardUnSet);
            }
        }

        public bool CanSelectZones
        {
            get
            {
                return (SelectedState != ZoneLogicState.Failure) &&
                    (SelectedState != ZoneLogicState.AM1TOn);
            }
        }

        public bool CanSelectDevice
        {
            get
            {
                return (SelectedState == ZoneLogicState.AM1TOn);
            }
        }

        void Update()
        {
            OnPropertyChanged("CanSelectOperation");
            OnPropertyChanged("CanSelectZones");
            OnPropertyChanged("CanSelectDevice");
            Zones = new List<ulong?>();
            SelectedDevice = null;

            ServiceFactory.Events.GetEvent<CurrentClauseStateChangedEvent>().Publish(SelectedState);
        }

        public string PresenrationZones
        {
            get
            {
                var presenrationZones = new StringBuilder();
                for (int i = 0; i < Zones.Count; ++i)
                {
                    if (i > 0)
                        presenrationZones.Append(", ");
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Zones[i]);
                    if (zone != null)
                        presenrationZones.Append(zone.PresentationName);
                }

                return presenrationZones.ToString();
            }
        }

        ZoneLogicState _selectedState;
        public ZoneLogicState SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
                Update();
            }
        }

        ZoneLogicOperation? _selectedOperation;
        public ZoneLogicOperation? SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                _selectedOperation = value;
                OnPropertyChanged("SelectedOperation");
            }
        }

        Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            var zonesSelectionViewModel = new ZonesSelectionViewModel();
            zonesSelectionViewModel.Initialize(_device, Zones, SelectedState);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zonesSelectionViewModel))
            {
                Zones = zonesSelectionViewModel.Zones;
                OnPropertyChanged("PresenrationZones");
            }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDevice()
        {
            var zoneLogicDeviceSelectionViewModel = new ZoneLogicDeviceSelectionViewModel(_device.Parent);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicDeviceSelectionViewModel))
                SelectedDevice = zoneLogicDeviceSelectionViewModel.SelectedDevice;
        }
    }
}