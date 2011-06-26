using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Windows;
using FiresecClient.Models;

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
                foreach (var driverId in _parent.Device.Driver.AvaliableChildren)
                {
                    var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x=>x.Id == driverId);
                    var availableDriver = new DriverViewModel(driver);
                    drivers.Add(availableDriver);
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
                device.Driver = SelectedDriver.NewDriver;
                if (SelectedDriver.NewDriver.HasNoAddress)
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

                foreach (var autoCreateDriverId in deviceViewModel.Device.Driver.AutoCreateChildren)
                {
                    var autoCreateDriver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == autoCreateDriverId);

                    if ((autoCreateDriver.ShortName == "МПТ") || (autoCreateDriver.ShortName == "Выход"))
                        continue;

                    for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
                    {
                        Device childDevice = new Device();
                        childDevice.Driver = autoCreateDriver;
                        childDevice.Address = i.ToString();
                        device.Children.Add(childDevice);

                        DeviceViewModel childDeviceViewModel = new DeviceViewModel();
                        childDeviceViewModel.Initialize(childDevice, _parent.Source);
                        childDeviceViewModel.Parent = deviceViewModel;
                        deviceViewModel.Children.Add(childDeviceViewModel);
                    }

                    deviceViewModel.IsExpanded = true;
                }

                _parent.Update();
            }

            FiresecManager.Configuration.FillAllDevices();
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
        public Driver NewDriver { get; private set; }

        public DriverViewModel(Driver driver)
        {
            NewDriver = driver;
        }

        public string DriverId
        {
            get { return NewDriver.Id; }
        }
        public string DriverName
        {
            get { return NewDriver.ShortName; }
        }
        public string ImageSource
        {
            get { return NewDriver.ImageSource; }
        }
    }
}
