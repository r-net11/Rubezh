using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using FiresecApi;
using FiresecClient.Models;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            CopyCommand = new RelayCommand(OnCopy, CanCopy);
            CutCommand = new RelayCommand(OnCut, CanCut);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
        }

        public void Initialize()
        {
            BuildTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

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

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            var device = FiresecManager.Configuration.RootDevice;

            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Parent = null;
            deviceViewModel.Initialize(device, Devices);
            Devices.Add(deviceViewModel);
            AddDevice(device, deviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (var device in parentDevice.Children)
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
            parentDeviceViewModel.IsExpanded = false;

            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.Device.Driver.Category() != DeviceCategory.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        bool CanCopy(object obj)
        {
            return true;
        }

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            copyDeviceViewModel = new DeviceViewModel();
            copyDeviceViewModel.Device = new Device();
            copyDeviceViewModel.Device.Driver = SelectedDevice.Device.Driver;
            copyDeviceViewModel.Device.DriverId = SelectedDevice.Device.DriverId;
            copyDeviceViewModel.Device.Address = SelectedDevice.Device.Address;
        }

        DeviceViewModel copyDeviceViewModel;

        bool CanCut(object obj)
        {
            return true;
        }

        public RelayCommand CutCommand { get; private set; }
        void OnCut()
        {
            _bufferDeviceViewModel = SelectedDevice;
        }

        DeviceViewModel _bufferDeviceViewModel;

        bool CanPaste(object obj)
        {
            return true;
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            copyDeviceViewModel.Device.Parent = SelectedDevice.Device;
            copyDeviceViewModel.Parent = SelectedDevice;
            SelectedDevice.Children.Add(copyDeviceViewModel);
            SelectedDevice.Device.Children.Add(copyDeviceViewModel.Device);

            SelectedDevice.Update();

            return;

            _bufferDeviceViewModel.Parent.Children.Remove(_bufferDeviceViewModel);
            _bufferDeviceViewModel.Parent.Device.Children.Remove(_bufferDeviceViewModel.Device);

            _bufferDeviceViewModel.Device.Parent = SelectedDevice.Device;
            _bufferDeviceViewModel.Parent = SelectedDevice;
            SelectedDevice.Children.Add(_bufferDeviceViewModel);
            SelectedDevice.Device.Children.Add(_bufferDeviceViewModel.Device);

            _bufferDeviceViewModel.Parent.Update();
            SelectedDevice.Update();
        }

        public override void OnShow()
        {
            DevicesMenuViewModel devicesMenuViewModel = new DevicesMenuViewModel(CopyCommand, CutCommand, PasteCommand);
            ServiceFactory.Layout.ShowMenu(devicesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
