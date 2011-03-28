using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using ClientApi;
using System.Collections.ObjectModel;
using ServiceApi;

namespace ServiceVisualizer
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            ConnectCommand = new RelayCommand(OnConnect);
            AddZoneCommant = new RelayCommand(OnAddZoneCommant);
            OkCommand = new RelayCommand(OnOkCommand);
            SaveCommand = new RelayCommand(OnSaveCommand);
        }

        public static ViewModel Current { get; private set; }

        ObservableCollection<DeviceViewModel> deviceViewModels;
        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return deviceViewModels; }
            set
            {
                deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
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
                OnPropertyChanged("SelectedZoneViewModel");
            }
        }

        DeviceViewModel selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOkCommand(object obj)
        {
            WindowManager.DriverId = SelectedDeviceViewModel.DriverId;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSaveCommand(object obj)
        {
            Configuration configuration = new Configuration();
            configuration.Devices = new List<Device>();
            configuration.Zones = new List<Zone>();
            foreach (DeviceViewModel deviceViewModel in DeviceViewModelList)
            {
                Device device = new Device();
                device.Address = deviceViewModel.Address;
                device.Description = deviceViewModel.Description;
                device.DriverId = deviceViewModel.DriverId;
                configuration.Devices.Add(device);
            }
            foreach (ZoneViewModel zoneViewModel in ZoneViewModels)
            {
                Zone zone = new Zone();
                zone.No = zoneViewModel.ZoneNo;
                zone.Name = zoneViewModel.ZoneName;
                zone.Description = zoneViewModel.ZoneDescription;
                zone.DetectorCount = zoneViewModel.ZoneDetectorCount;
                configuration.Zones.Add(zone);
            }


            StateConfiguration stateConfiguration = new StateConfiguration();
            stateConfiguration.ShortZones = new List<ShortZone>();
            foreach (ZoneViewModel zoneViewModel in ZoneViewModels)
            {
                ShortZone shortZone = new ShortZone();
                shortZone.No = zoneViewModel.ZoneNo;
                shortZone.Name = zoneViewModel.ZoneName;
                shortZone.Description = zoneViewModel.ZoneDescription;
                shortZone.DetectorCount = zoneViewModel.ZoneDetectorCount;
                shortZone.EvacuationTime = zoneViewModel.ZoneEvacuationTime;
                stateConfiguration.ShortZones.Add(shortZone);
            }

            DeviceViewModel rootDeviceViewModel = DeviceViewModels[0];
            ShortDevice rootShortDevice = DeviceViewModelToShortDevice(rootDeviceViewModel);
            AddShortDevice(rootDeviceViewModel, rootShortDevice);

            stateConfiguration.RootShortDevice = rootShortDevice;

            serviceClient.SetNewConfig(stateConfiguration);
        }

        void AddShortDevice(DeviceViewModel parentDeviceViewModel, ShortDevice parentShortDevice)
        {
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                ShortDevice shortDevice = DeviceViewModelToShortDevice(deviceViewModel);
                parentShortDevice.Children.Add(shortDevice);
                AddShortDevice(deviceViewModel, shortDevice);
            }
        }

        ShortDevice DeviceViewModelToShortDevice(DeviceViewModel deviceViewModel)
        {
            ShortDevice shortDevice = new ShortDevice();
            shortDevice.Children = new List<ShortDevice>();
            shortDevice.Address = deviceViewModel.Address;
            shortDevice.Description = deviceViewModel.Description;
            shortDevice.DriverId = deviceViewModel.DriverId;
            if (deviceViewModel.Zone != null)
            {
                shortDevice.Zone = deviceViewModel.Zone.ZoneNo;
            }
            return shortDevice;
        }

        public RelayCommand AddZoneCommant { get; private set; }
        void OnAddZoneCommant(object obj)
        {
            Zone zone = new Zone();
            zone.Id = "0";
            zone.Name = "новая зона";
            ZoneViewModel zoneViewModel = new ZoneViewModel();
            zoneViewModel.SetZone(zone);
            ZoneViewModels.Add(zoneViewModel);
        }

        ServiceClient serviceClient;

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect(object obj)
        {
            serviceClient = new ServiceClient();
            serviceClient.Start();
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();

            DeviceViewModelList = new List<DeviceViewModel>();

            Device rootDevice = ServiceClient.Configuration.Devices[0];

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.SetDevice(rootDevice);
            rootDeviceViewModel.IsExpanded = true;
            DeviceViewModels.Add(rootDeviceViewModel);
            AddDevice(rootDevice, rootDeviceViewModel);
            DeviceViewModelList.Add(rootDeviceViewModel);

            AddZones();

            foreach (DeviceViewModel deviceViewModel in DeviceViewModelList)
            {
                deviceViewModel.SetZone();
            }

            treeBuilder = new FiresecMetadata.TreeBuilder();
            treeBuilder.Buid();
        }

        public FiresecMetadata.TreeBuilder treeBuilder;

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.SetDevice(device);
                deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
                DeviceViewModelList.Add(deviceViewModel);
            }
        }

        List<DeviceViewModel> DeviceViewModelList;

        void AddZones()
        {
            ZoneViewModels = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in ServiceClient.Configuration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel();
                zoneViewModel.SetZone(zone);
                ZoneViewModels.Add(zoneViewModel);
            }
        }
    }
}
