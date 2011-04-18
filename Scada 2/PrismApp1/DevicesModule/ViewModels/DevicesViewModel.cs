using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using ClientApi;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            Current = this;
        }

        public void Initilize()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            AllDeviceViewModels = new ObservableCollection<DeviceViewModel>();

            Device rooDevice = ServiceClient.CurrentConfiguration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Initialize(rooDevice, AllDeviceViewModels);
            //rootDeviceViewModel.IsExpanded = false;
            DeviceViewModels.Add(rootDeviceViewModel);
            AllDeviceViewModels.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);

            ExpandChild(AllDeviceViewModels[0]);

            ClientManager.Start();
            ServiceClient.CurrentStates.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
        }

        void CurrentStates_DeviceStateChanged(string path)
        {
            DeviceViewModel deviceViewModel = DeviceViewModels.FirstOrDefault(x => x.Device.Path == path);

            deviceViewModel.InitializeParameters();
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, AllDeviceViewModels);
                //deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                DeviceViewModels.Add(deviceViewModel);
                AllDeviceViewModels.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = true;
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                deviceViewModel.IsExpanded = true;
                ExpandChild(deviceViewModel);
            }
        }

        public static DevicesViewModel Current { get; private set; }

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

        public override void Dispose()
        {
        }
    }
}
