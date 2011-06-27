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
            _parentDeviceViewModel = parent;
        }

        DeviceViewModel _parentDeviceViewModel;

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Configuration.Drivers
                       where _parentDeviceViewModel.Device.Driver.AvaliableChildren.Contains(driver.Id)
                       select driver;
            }
        }

        Driver _selectedDriver;
        public Driver SelectedDriver
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
                device.Driver = SelectedDriver;

                device.Address = "";
                if (SelectedDriver.HasAddress)
                {
                    device.Address = GetNewAddress();
                }

                _parentDeviceViewModel.Device.Children.Add(device);

                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, _parentDeviceViewModel.Source);
                deviceViewModel.Parent = _parentDeviceViewModel;
                _parentDeviceViewModel.Children.Add(deviceViewModel);

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
                        childDeviceViewModel.Initialize(childDevice, _parentDeviceViewModel.Source);
                        childDeviceViewModel.Parent = deviceViewModel;
                        deviceViewModel.Children.Add(childDeviceViewModel);
                    }

                    deviceViewModel.IsExpanded = true;
                }

                _parentDeviceViewModel.Update();
            }

            FiresecManager.Configuration.Update();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
