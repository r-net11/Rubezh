using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(Select);
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        public void Initialize()
        {
            FiresecEventSubscriber.ZoneStateChangedEvent += new Action<string>(OnZoneStateChangedEvent);
            Zones = (from Zone zone in FiresecManager.DeviceConfiguration.Zones
                       orderby int.Parse(zone.No)
                       select new ZoneViewModel(zone)).ToList();

            if (Zones.Count > 0)
            SelectedZone = Zones[0];
        }

        public List<ZoneViewModel> Zones { get; private set; }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                InitializeDevices();
                OnPropertyChanged("SelectedZone");
            }
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        void OnZoneStateChanged(string zoneNo)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            if (zoneState != null)
            {
                var zoneViewModel = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
                if (zoneViewModel != null)
                {
                    zoneViewModel.StateType = zoneState.StateType;
                }
            }
        }

        void OnDeviceStateChanged(string id)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            if (deviceState != null)
            {
                if (Devices != null)
                {
                    var deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);
                    if (deviceViewModel != null)
                    {
                        deviceViewModel.Update();
                        //deviceViewModel.MainState = deviceState.StateType;
                    }
                }
            }
        }

        void OnZoneStateChangedEvent(string zoneNo)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            var zoneViewModel = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
            zoneViewModel.StateType = zoneState.StateType;
        }

        public void Select(string zoneNo)
        {
            if (string.IsNullOrEmpty(zoneNo) == false)
            {
                SelectedZone = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
            }
        }

        void InitializeDevices()
        {
            if (SelectedZone == null)
                return;

            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.IsZoneDevice)
                {
                    if (device.ZoneNo == SelectedZone.Zone.No)
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }

                if (device.Driver.IsZoneLogicDevice)
                {
                    if (device.ZoneLogic != null)
                    {
                        foreach (var clause in device.ZoneLogic.Clauses)
                        {
                            if (clause.Zones.Contains(SelectedZone.Zone.No))
                            {
                                device.AllParents.ForEach(x => { devices.Add(x); });
                                devices.Add(device);
                            }
                        }
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }
        }
    }
}