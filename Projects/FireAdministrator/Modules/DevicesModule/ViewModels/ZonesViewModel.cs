using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            EditCommand = new RelayCommand(OnEdit);
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in FiresecManager.CurrentConfiguration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
        }

        ObservableCollection<ZoneViewModel> _zones;
        public ObservableCollection<ZoneViewModel> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;

                InitializeAvaliableDevices();
                InitializeDevices();

                OnPropertyChanged("SelectedZone");
            }
        }

        void InitializeAvaliableDevices()
        {
            List<Device> resultDevices = new List<Device>();
            foreach (Device device in FiresecManager.CurrentConfiguration.AllDevices)
            {
                var driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                if (driver.minZoneCardinality != "0")
                {
                    if (string.IsNullOrEmpty(device.ZoneNo))
                    {
                        List<Device> allParents = device.AllParents;
                        foreach (var parentDevice in allParents)
                        {
                            if (resultDevices.Any(x => x.Id == parentDevice.Id) == false)
                            {
                                resultDevices.Add(parentDevice);
                            }
                        }
                        resultDevices.Add(device);
                    }
                }
            }

            var _Devices = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in resultDevices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                _Devices.Add(deviceViewModel);
            }

            foreach (var _device in _Devices)
            {
                if (_device._device.Parent != null)
                {
                    var id = _device._device.Parent.Id;
                    var parent = _Devices.FirstOrDefault(x => x._device.Id == id);
                    _device.Parent = parent;
                }
            }

            AvaliableDevices = _Devices;
        }

        void InitializeDevices()
        {
            List<Device> resultDevices = new List<Device>();
            foreach (Device device in FiresecManager.CurrentConfiguration.AllDevices)
            {
                var driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x=>x.id == device.DriverId);
                if (driver.minZoneCardinality != "0")
                {
                    if (device.ZoneNo == SelectedZone.No)
                    {
                        List<Device> allParents = device.AllParents;
                        foreach (var parentDevice in allParents)
                        {
                            if (resultDevices.Any(x => x.Id == parentDevice.Id) == false)
                            {
                                resultDevices.Add(parentDevice);
                            }
                        }
                        resultDevices.Add(device);
                    }
                }
            }

            var _Devices = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in resultDevices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                _Devices.Add(deviceViewModel);
            }

            foreach (var _device in _Devices)
            {
                if (_device._device.Parent != null)
                {
                    var id = _device._device.Parent.Id;
                    var parent = _Devices.FirstOrDefault(x => x._device.Id == id);
                    _device.Parent = parent;
                }
            }

            Devices = _Devices;
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

        ObservableCollection<DeviceViewModel> _avaliableDevices;
        public ObservableCollection<DeviceViewModel> AvaliableDevices
        {
            get { return _avaliableDevices; }
            set
            {
                _avaliableDevices = value;
                OnPropertyChanged("AvaliableDevices");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            Zone zone = new Zone();
            zone.No = "0";
            zone.Name = "Новая зона";
            ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
            Zones.Add(zoneViewModel);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (SelectedZone != null)
            {
                var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить зону " + SelectedZone.PresentationName, "Подтверждение", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                    Zones.Remove(SelectedZone);
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedZone != null)
            {
                ZoneDetailsViewModel zoneDetailsViewModel = new ZoneDetailsViewModel();
                zoneDetailsViewModel.Initialize(SelectedZone._zone);
                ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
                SelectedZone.Update();
            }
        }

        public void Save()
        {
            CurrentConfiguration currentConfiguration = new CurrentConfiguration();
            currentConfiguration.Zones = new List<Zone>();
            foreach (ZoneViewModel zoneViewModel in Zones)
            {
                Zone zone = new Zone();
                zone.No = zoneViewModel.No;
                zone.Name = zoneViewModel.Name;
                zone.Description = zoneViewModel.Description;
                zone.DetectorCount = zoneViewModel.DetectorCount;
                zone.EvacuationTime = zoneViewModel.EvacuationTime;
                currentConfiguration.Zones.Add(zone);
            }
        }

        public override void Dispose()
        {
        }
    }
}
