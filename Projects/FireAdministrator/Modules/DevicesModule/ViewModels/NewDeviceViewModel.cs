using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public class NewDeviceViewModel : DialogContent
    {
        public NewDeviceViewModel()
        {
            Title = "Новое устройство";
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        DeviceViewModel _parent;

        public void Initialize(DeviceViewModel parent)
        {
            _parent = parent;
            Devices = new ObservableCollection<AvailableDevice>();

            foreach (var childDriver in FiresecManager.Configuration.Metadata.drv)
            {
                var childClass = FiresecManager.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == parent.Driver.clsid)))
                {
                    if ((childDriver.lim_parent != null) && (childDriver.lim_parent != parent.Driver.id))
                        continue;
                    if (childDriver.acr_enabled == "1")
                        continue;
                    AvailableDevice availableDevice = new AvailableDevice();
                    availableDevice.Init(childDriver);
                    Devices.Add(availableDevice);
                }
            }
        }

        ObservableCollection<AvailableDevice> _devices;
        public ObservableCollection<AvailableDevice> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        AvailableDevice _selectedDevice;
        public AvailableDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedDevice != null)
            {
                string driverId = SelectedDevice.DriverId;
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                Device device = new Device();
                device.Properties = new List<Property>();
                device.DriverId = driverId;
                var driver = FiresecManager.Configuration.Metadata.drv.FirstOrDefault(x => x.id == driverId);
                if (driver.ar_no_addr == "1")
                {
                    device.Address = "";
                }
                else
                {
                    device.Address = "0.0";
                }
                deviceViewModel.Initialize(device, _parent.Source);
                deviceViewModel.Parent = _parent;
                _parent.Children.Add(deviceViewModel);

                foreach (var childDriver in FiresecManager.Configuration.Metadata.drv)
                {
                    var childClass = FiresecManager.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                    if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == deviceViewModel.Driver.clsid)))
                    {
                        if ((childDriver.lim_parent != null) && (childDriver.lim_parent != deviceViewModel.Driver.id))
                            continue;
                        if (childDriver.acr_enabled == "1")
                        {
                            if ((childDriver.shortName == "МПТ") || (childDriver.shortName == "Выход"))
                                continue;

                            int minAddress = Convert.ToInt32(childDriver.acr_from);
                            int maxAddress = Convert.ToInt32(childDriver.acr_to);
                            for (int i = minAddress; i <= maxAddress; i++)
                            {
                                DeviceViewModel childDeviceViewModel = new DeviceViewModel();
                                Device childDevice = new Device();
                                childDevice.Properties = new List<Property>();
                                childDevice.DriverId = childDriver.id;
                                childDevice.Address = i.ToString();
                                childDeviceViewModel.Initialize(childDevice, _parent.Source);
                                childDeviceViewModel.Parent = deviceViewModel;
                                deviceViewModel.Children.Add(childDeviceViewModel);
                            }

                            deviceViewModel.IsExpanded = true;
                        }
                    }
                }

                _parent.Update();
                _parent.IsExpanded = false;
                _parent.IsExpanded = true;
            }

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(true);
        }
    }

    public class AvailableDevice
    {
        public void Init(Firesec.Metadata.configDrv driver)
        {
            DriverId = driver.id;
            DriverName = driver.shortName;

            string ImageName;
            if (!string.IsNullOrEmpty(driver.dev_icon))
            {
                ImageName = driver.dev_icon;
            }
            else
            {
                var metadataClass = FiresecManager.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == driver.clsid);
                ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
            }

            ImageSource = @"C:\Program Files\Firesec\Icons\" + ImageName + ".ico";
        }

        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string ImageSource { get; set; }
    }
}
