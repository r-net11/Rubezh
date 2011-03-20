using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using ClientApi;
using System.Collections.ObjectModel;

namespace ServiceVisualizer
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Current = this;
            ConnectCommand = new RelayCommand(OnConnect);
        }

        public static ViewModel Current { get; private set; }

        ObservableCollection<DeviceViewModel> devicesViewModels;
        public ObservableCollection<DeviceViewModel> DevicesViewModels
        {
            get { return devicesViewModels; }
            set
            {
                devicesViewModels = value;
                OnPropertyChanged("DevicesViewModels");
            }
        }

        DeviceViewModel selectedDevicesViewModel;
        public DeviceViewModel SelectedDevicesViewModel
        {
            get { return selectedDevicesViewModel; }
            set
            {
                selectedDevicesViewModel = value;
                OnPropertyChanged("SelectedDevicesViewModel");
            }
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect(object obj)
        {
            ServiceClient serviceClient = new ServiceClient();
            serviceClient.Start();
            DevicesViewModels = new ObservableCollection<DeviceViewModel>();

            Device rootDevice = ServiceClient.Configuration.Devices[0];

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Children = new List<DeviceViewModel>();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Name = rootDevice.DriverName;
            rootDeviceViewModel.Address = rootDevice.PresentationAddress;
            rootDeviceViewModel.Zone = rootDevice.Zone;
            rootDeviceViewModel.State = rootDevice.State;
            rootDeviceViewModel.States = "";
            foreach(string state in rootDevice.States)
            {
                rootDeviceViewModel.States += state + "\n";
            }
            DevicesViewModels.Add(rootDeviceViewModel);
            AddDevice(rootDevice, rootDeviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Name = device.DriverName;
                deviceViewModel.Address = device.PresentationAddress;
                deviceViewModel.Zone = device.Zone;
                deviceViewModel.State = device.State;
                deviceViewModel.States = "";
                foreach (string state in device.States)
                {
                    deviceViewModel.States += state + "\n";
                }
                parentDeviceViewModel.Children.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
