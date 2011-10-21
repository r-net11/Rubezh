using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(Select);
            FiresecEventSubscriber.ZoneStateChangedEvent += OnZoneStateChanged;
            FiresecEventSubscriber.DeviceStateChangedEvent += OnDeviceStateChanged;
        }

        public void Initialize()
        {
            FiresecEventSubscriber.ZoneStateChangedEvent += new Action<ulong?>(OnZoneStateChangedEvent);
            Zones = (from Zone zone in FiresecManager.DeviceConfiguration.Zones
                     orderby zone.No
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

        void OnZoneStateChanged(ulong? zoneNo)
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

        void OnDeviceStateChanged(Guid deviceUID)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
            if (deviceState != null)
            {
                if (Devices != null)
                {
                    var deviceViewModel = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
                    if (deviceViewModel != null)
                    {
                        deviceViewModel.Update();
                        //deviceViewModel.MainState = deviceState.StateType;
                    }
                }
            }
        }

        void OnZoneStateChangedEvent(ulong? zoneNo)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            var zoneViewModel = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
            zoneViewModel.StateType = zoneState.StateType;
        }

        public void Select(ulong? zoneNo)
        {
            if (zoneNo.HasValue)
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
                if (device.Driver.IsZoneDevice && device.ZoneNo == SelectedZone.Zone.No)
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }

                if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
                {
                    foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.Zones.Contains(SelectedZone.Zone.No)))
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                Devices.Add(new DeviceViewModel(device, Devices)
                {
                    IsExpanded = true
                });
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }
        }
    }
}