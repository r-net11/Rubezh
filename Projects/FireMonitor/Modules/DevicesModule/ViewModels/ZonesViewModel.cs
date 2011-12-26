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

        public void Select(ulong? zoneNo)
        {
            if (zoneNo.HasValue)
                SelectedZone = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
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

            var deviceViewModels = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                deviceViewModels.Add(new DeviceViewModel(device, deviceViewModels)
                {
                    IsExpanded = true,
                    IsBold = device.Driver.IsZoneDevice || device.Driver.IsZoneLogicDevice
                });
            }

            foreach (var device in deviceViewModels)
            {
                if (device.Device.Parent != null)
                {
                    var parent = deviceViewModels.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            Devices = deviceViewModels;
        }
    }
}