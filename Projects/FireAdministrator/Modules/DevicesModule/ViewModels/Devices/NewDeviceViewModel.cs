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
        public NewDeviceViewModel(DeviceViewModel parent)
        {
            Title = "Новое устройство";
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            _parent = parent;
        }

        DeviceViewModel _parent;

        public List<DriverViewModel> Drivers
        {
            get
            {
                List<DriverViewModel> drivers = new List<DriverViewModel>();

                foreach (var driver in FiresecManager.Configuration.Metadata.drv)
                {
                    var childClass = FiresecManager.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == driver.clsid);
                    if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == _parent.Device.Driver.clsid)))
                    {
                        if ((driver.lim_parent != null) && (driver.lim_parent != _parent.Device.Driver.id))
                            continue;
                        if (driver.acr_enabled == "1")
                            continue;
                        var availableDevice = new DriverViewModel(driver);
                        drivers.Add(availableDevice);
                    }
                }

                return drivers;
            }
        }

        DriverViewModel _selectedDriver;
        public DriverViewModel SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                OnPropertyChanged("SelectedDriver");
            }
        }

        string GetNewAddress()
        {
            return "0.0";
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedDriver != null)
            {
                Device device = new Device();
                device.DriverId = SelectedDriver.DriverId;
                device.Driver = SelectedDriver.Driver;
                if (SelectedDriver.Driver.ar_no_addr == "1")
                {
                    device.Address = "";
                }
                else
                {
                    device.Address = GetNewAddress();
                }
                _parent.Device.Children.Add(device);

                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, _parent.Source);
                deviceViewModel.Parent = _parent;
                _parent.Children.Add(deviceViewModel);

                foreach (var childDriver in FiresecManager.Configuration.Metadata.drv)
                {
                    var childClass = FiresecManager.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                    if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == deviceViewModel.Device.Driver.clsid)))
                    {
                        if ((childDriver.lim_parent != null) && (childDriver.lim_parent != deviceViewModel.Device.Driver.id))
                            continue;
                        if (childDriver.acr_enabled == "1")
                        {
                            if ((childDriver.shortName == "МПТ") || (childDriver.shortName == "Выход"))
                                continue;

                            int minAddress = Convert.ToInt32(childDriver.acr_from);
                            int maxAddress = Convert.ToInt32(childDriver.acr_to);
                            for (int i = minAddress; i <= maxAddress; i++)
                            {
                                Device childDevice = new Device();
                                childDevice.DriverId = childDriver.id;
                                childDevice.Driver = childDriver;
                                childDevice.Address = i.ToString();
                                device.Children.Add(childDevice);

                                DeviceViewModel childDeviceViewModel = new DeviceViewModel();
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
            Close(false);
        }
    }

    public class DriverViewModel
    {
        public Firesec.Metadata.configDrv Driver { get; private set; }

        public DriverViewModel(Firesec.Metadata.configDrv driver)
        {
            Driver = driver;
        }

        public string DriverId
        {
            get { return Driver.id; }
        }
        public string DriverName
        {
            get { return Driver.shortName; }
        }
        public string ImageSource
        {
            get { return Driver.ImageSource(); }
        }
    }
}
