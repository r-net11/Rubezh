using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using ClientApi;
using DevicesModule.Events;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(OnZoneSelected);
        }

        public void Initialize()
        {
            ZoneViewModels = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in ServiceClient.CurrentConfiguration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel();
                zoneViewModel.SetZone(zone);
                ZoneViewModels.Add(zoneViewModel);
            }
        }

        void OnZoneSelected(Zone zone)
        {
            SelectedZoneViewModel = ZoneViewModels.FirstOrDefault(x=>x.ZoneNo == zone.No);
        }

        ObservableCollection<ZoneViewModel> zoneViewModels;
        public ObservableCollection<ZoneViewModel> ZoneViewModels
        {
            get { return zoneViewModels; }
            set
            {
                zoneViewModels = value;
                OnPropertyChanged("ZoneViewModels");
            }
        }

        ZoneViewModel selectedZoneViewModel;
        public ZoneViewModel SelectedZoneViewModel
        {
            get { return selectedZoneViewModel; }
            set
            {
                selectedZoneViewModel = value;
                InitializeDevices();
                OnPropertyChanged("SelectedZoneViewModel");
            }
        }

        void InitializeDevices()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device rooDevice = ServiceClient.CurrentConfiguration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Initialize(rooDevice, Devices);
            rootDeviceViewModel.IsExpanded = true;
            Devices.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                if ((device.UderlyingZones.Contains(SelectedZoneViewModel.ZoneNo) == false) &&
                    (device.ZoneNo != SelectedZoneViewModel.ZoneNo))
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

        ObservableCollection<DeviceViewModel> devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        public override void Dispose()
        {
        }
    }
}
