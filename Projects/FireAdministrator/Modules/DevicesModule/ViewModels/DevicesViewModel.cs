using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using FiresecClient;
using System.Collections.ObjectModel;
using DevicesModule.PropertyBindings;
using System.Xml.Serialization;
using System.IO;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            Current = this;
        }

        public static DevicesViewModel Current { get; private set; }

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

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        ObservableCollection<DeviceViewModel> allDeviceViewModels;
        public ObservableCollection<DeviceViewModel> AllDeviceViewModels
        {
            get { return allDeviceViewModels; }
            set
            {
                allDeviceViewModels = value;
                OnPropertyChanged("AllDeviceViewModels");
            }
        }

        public void Save()
        {
            CurrentConfiguration currentConfiguration = new CurrentConfiguration();

            DeviceViewModel rootDeviceViewModel = Devices[0];
            Device rootDevice = DeviceViewModelToDevice(rootDeviceViewModel);
            rootDevice.Parent = null;
            
            AddDevice(rootDeviceViewModel, rootDevice);

            currentConfiguration.RootDevice = rootDevice;

            FiresecManager.SetNewConfig(currentConfiguration);
        }

        void AddDevice(DeviceViewModel parentDeviceViewModel, Device parentDevice)
        {
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                Device device = DeviceViewModelToDevice(deviceViewModel);
                device.Parent = parentDevice;
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
                device.ZoneNo = deviceViewModel.Zone.No;
            }
            if (deviceViewModel._device.ZoneLogic != null)
            {
                device.ZoneLogic = deviceViewModel._device.ZoneLogic;


                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.ZoneLogic.expr));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, deviceViewModel._device.ZoneLogic);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                string message = Encoding.GetEncoding("windows-1251").GetString(bytes);
                message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                message = message.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"windows-1251\"?>");

                message = message.Replace("\n", "").Replace("\r", "");

                message = message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("=", "&#061;");
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

        public void Initialize()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            AllDeviceViewModels = new ObservableCollection<DeviceViewModel>();

            DeviceViewModelList = new List<DeviceViewModel>();

            Device rooDevice = FiresecManager.CurrentConfiguration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Initialize(rooDevice);
            //rootDeviceViewModel.IsExpanded = false;
            Devices.Add(rootDeviceViewModel);
            AllDeviceViewModels.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);
            DeviceViewModelList.Add(rootDeviceViewModel);

            CollapseChild(AllDeviceViewModels[0]);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device);
                //deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                AllDeviceViewModels.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
                DeviceViewModelList.Add(deviceViewModel);
            }
        }

        void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = true;
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                deviceViewModel.IsExpanded = true;
                CollapseChild(deviceViewModel);
            }
        }

        List<DeviceViewModel> DeviceViewModelList;

        public override void Dispose()
        {
        }
    }
}
