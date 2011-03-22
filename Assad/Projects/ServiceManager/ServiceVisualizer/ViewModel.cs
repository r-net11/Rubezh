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
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            ConnectCommand = new RelayCommand(OnConnect);
            ShowZoneCommant = new RelayCommand(OnShowZoneCommant);
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

        public RelayCommand ShowZoneCommant { get; private set; }
        void OnShowZoneCommant(object obj)
        {
            System.Windows.Window window = new System.Windows.Window();
            window.Show();
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect(object obj)
        {
            ServiceClient serviceClient = new ServiceClient();
            serviceClient.Start();
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();

            Device rootDevice = ServiceClient.Configuration.Devices[0];

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.SetDevice(rootDevice);
            rootDeviceViewModel.IsExpanded = true;
            DeviceViewModels.Add(rootDeviceViewModel);
            AddDevice(rootDevice, rootDeviceViewModel);

            AddZones();
        }

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
            }
        }

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
