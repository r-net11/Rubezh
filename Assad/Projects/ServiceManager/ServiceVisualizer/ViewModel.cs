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
            CurrentConfiguration currentConfiguration = new CurrentConfiguration();
            currentConfiguration.Zones = new List<Zone>();
            foreach (ZoneViewModel zoneViewModel in ZoneViewModels)
            {
                Zone zone = new Zone();
                zone.No = zoneViewModel.ZoneNo;
                zone.Name = zoneViewModel.ZoneName;
                zone.Description = zoneViewModel.ZoneDescription;
                zone.DetectorCount = zoneViewModel.ZoneDetectorCount;
                zone.EvacuationTime = zoneViewModel.ZoneEvacuationTime;
                currentConfiguration.Zones.Add(zone);
            }

            DeviceViewModel rootDeviceViewModel = DeviceViewModels[0];
            Device rootDevice = DeviceViewModelToDevice(rootDeviceViewModel);
            AddDevice(rootDeviceViewModel, rootDevice);

            currentConfiguration.RootDevice = rootDevice;

            serviceClient.SetNewConfig(currentConfiguration);
        }

        void AddDevice(DeviceViewModel parentDeviceViewModel, Device parentDevice)
        {
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                Device device = DeviceViewModelToDevice(deviceViewModel);
                parentDevice.Children.Add(device);
                AddDevice(deviceViewModel, device);
            }
        }

        Device DeviceViewModelToDevice(DeviceViewModel deviceViewModel)
        {
            Device device = new Device();
            device.Children = new List<Device>();
            device.Address = deviceViewModel.Address;
            device.Description = deviceViewModel.Description;
            device.DriverId = deviceViewModel.DriverId;
            if (deviceViewModel.Zone != null)
            {
                device.ZoneNo = deviceViewModel.Zone.ZoneNo;
            }
            device.Properties = new List<Property>();
            foreach (StringProperty stringProperty in deviceViewModel.StringProperties)
            {
                Property property = new Property();
                property.Name = stringProperty.PropertyName;
                property.Value = stringProperty.Text;
                device.Properties.Add(property);
            }
            foreach (BoolProperty boolProperty in deviceViewModel.BoolProperties)
            {
                Property property = new Property();
                property.Name = boolProperty.PropertyName;
                property.Value = boolProperty.IsChecked ? "1" : "0";
                device.Properties.Add(property);
            }
            foreach (EnumProperty enumProperty in deviceViewModel.EnumProperties)
            {
                Property property = new Property();
                property.Name = enumProperty.PropertyName;
                property.Value = enumProperty.SelectedValue;
                device.Properties.Add(property);
            }
            return device;
        }

        public RelayCommand AddZoneCommant { get; private set; }
        void OnAddZoneCommant(object obj)
        {
            Zone zone = new Zone();
            zone.No = "0";
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

            Device rooDevice = ServiceClient.CurrentConfiguration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.SetDevice(rooDevice);
            rootDeviceViewModel.IsExpanded = true;
            DeviceViewModels.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);
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

            foreach (Zone zone in ServiceClient.CurrentConfiguration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel();
                zoneViewModel.SetZone(zone);
                ZoneViewModels.Add(zoneViewModel);
            }
        }
    }
}
