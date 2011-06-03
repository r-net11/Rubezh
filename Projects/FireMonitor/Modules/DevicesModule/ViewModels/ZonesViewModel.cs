using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using DevicesModule.Events;
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
            FiresecManager.States.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
        }

        public ObservableCollection<ZoneViewModel> Zones
        {
            get
            {
                List<ZoneViewModel> zones = new List<ZoneViewModel>();

                foreach (var zone in FiresecManager.Configuration.Zones)
                {
                    ZoneViewModel zoneViewModel = new ZoneViewModel();
                    zoneViewModel.Initialize(zone);
                    zones.Add(zoneViewModel);
                }

                zones.Sort(delegate(ZoneViewModel zone1, ZoneViewModel zone2)
                {
                    return System.Convert.ToInt32(zone1.No) - System.Convert.ToInt32(zone2.No);
                });

                return new ObservableCollection<ZoneViewModel>(zones);
            }
        }

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
            ZoneState zoneState = FiresecManager.States.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            if (zoneState != null)
            {
                ZoneViewModel zoneViewModel = Zones.FirstOrDefault(x => x.No == zoneNo);
                if (zoneViewModel != null)
                {
                    zoneViewModel.State = zoneState.State;
                }
            }
        }

        void OnDeviceStateChanged(string id)
        {
            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == id);
            if (deviceState != null)
            {
                if (Devices != null)
                {
                    DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Id == id);
                    if (deviceViewModel != null)
                    {
                        deviceViewModel.Update();
                        //deviceViewModel.MainState = deviceState.State;
                    }
                }
            }
        }

        void CurrentStates_ZoneStateChanged(string zoneNo)
        {
            ZoneState zoneState = FiresecManager.States.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            ZoneViewModel zoneViewModel = Zones.FirstOrDefault(x => x.No == zoneNo);
            zoneViewModel.State = zoneState.State;
        }

        public void Select(string zoneNo)
        {
            if (string.IsNullOrEmpty(zoneNo) == false)
            {
                SelectedZone = Zones.FirstOrDefault(x => x.No == zoneNo);
            }
        }

        void InitializeDevices()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device rooDevice = FiresecManager.Configuration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Initialize(rooDevice, Devices);
            rootDeviceViewModel.IsExpanded = true;
            Devices.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (var device in parentDevice.Children)
            {
                if ((device.UderlyingZones.Contains(SelectedZone.No) == false) &&
                    (device.ZoneNo != SelectedZone.No))
                    continue;

                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        public override void Dispose()
        {
        }
    }
}
