using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using System;
using System.Linq;
using FiresecClient;
using DevicesModule.Zones.Events;

namespace DevicesModule.ViewModels
{
    public class ClauseViewModel : BaseViewModel
    {
        public ClauseViewModel()
        {
            ShowZonesCommand = new RelayCommand(OnShowZones);
            SelectDeviceCommand = new RelayCommand(OnSelectDevice);
        }

        public List<string> Zones { get; set; }
        Device _device;

        public void Initialize(Device device, Clause clause)
        {
            _device = device;
            Zones = clause.Zones.ToList();
            _selectedState = clause.State;
            SelectedOperation = clause.Operation;
            if (clause.DeviceUID != null)
            {
                SelectedDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x=>x.UID == clause.DeviceUID);
            }
        }

        public List<ZoneLogicState> States
        {
            get
            {
                switch (_device.Driver.DriverName)
                {
                    case "Задвижка":
                        return new List<ZoneLogicState>() { ZoneLogicState.Fire,
                            ZoneLogicState.Attention,
                            ZoneLogicState.MPTAutomaticOn,
                            ZoneLogicState.MPTOn,
                            ZoneLogicState.AM1TOn};

                    case "Релейный исполнительный модуль РМ-1":
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
            Zones = new List<string>();
            SelectedDevice = null;

            if ((SelectedState == ZoneLogicState.Lamp) || (SelectedState == ZoneLogicState.PCN))
            {
                ServiceFactory.Events.GetEvent<BlockClauseAddingEvent>().Publish(true);
            }
            else
            {
                ServiceFactory.Events.GetEvent<BlockClauseAddingEvent>().Publish(false);
            }
        }

        public string PresenrationZones
        {
            get
            {
                string presenrationZones = "";
                for (int i = 0; i < Zones.Count; i++)
                {
                    if (i > 0)
                        presenrationZones += ",";
                    presenrationZones += Zones[i];
                }
                return presenrationZones;
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

        ZoneLogicOperation _selectedOperation;
        public ZoneLogicOperation SelectedOperation
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
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zonesSelectionViewModel);
            if (result)
            {
                Zones = zonesSelectionViewModel.Zones;
                OnPropertyChanged("PresenrationZones");
            }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDevice()
        {
            var zoneLogicDeviceSelectionViewModel = new ZoneLogicDeviceSelectionViewModel(_device.Parent);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicDeviceSelectionViewModel);
            if (result)
            {
                SelectedDevice = zoneLogicDeviceSelectionViewModel.SelectedDevice;
            }
        }
    }
}