using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
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
            if (deviceViewModel.Device.ZoneLogic != null)
            {
                device.ZoneLogic = deviceViewModel.Device.ZoneLogic;


                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.ZoneLogic.expr));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, deviceViewModel.Device.ZoneLogic);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                string message = Encoding.GetEncoding("windows-1251").GetString(bytes);
                message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                message = message.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"windows-1251\"?>");

                message = message.Replace("\n", "").Replace("\r", "");

                message = message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("=", "&#061;");
            }
            return device;
        }

        public void Initialize()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device device = FiresecManager.CurrentConfiguration.RootDevice;

            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Parent = null;
            deviceViewModel.Initialize(device, Devices);
            Devices.Add(deviceViewModel);
            AddDevice(device, deviceViewModel);

            CollapseChild(Devices[0]);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                parentDeviceViewModel.Children.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
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

        public override void Dispose()
        {
        }
    }
}
