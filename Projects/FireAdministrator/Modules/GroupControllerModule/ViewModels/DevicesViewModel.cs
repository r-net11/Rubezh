using Infrastructure.Common;
using GroupControllerModule.Models;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Infrastructure;

namespace GroupControllerModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        ConfigurationConverter ConfigurationConverter;

        public DevicesViewModel()
        {
            ConvertCommand = new RelayCommand(OnConvert);
        }

        public RelayCommand ConvertCommand { get; private set; }
        void OnConvert()
        {
            ConfigurationConverter = new ConfigurationConverter();
            ConfigurationConverter.Convert();
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
                if (value != null)
                    value.ExpantToThis();
                OnPropertyChanged("SelectedDevice");
            }
        }

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            var xRootDevice = XManager.DeviceConfiguration.RootDevice;
            AddDevice(xRootDevice, null);
        }

        public DeviceViewModel AddDevice(XDevice xDevice, DeviceViewModel parentDeviceViewModel)
        {
            var xDeviceViewModel = new DeviceViewModel(xDevice, Devices);
            xDeviceViewModel.Parent = parentDeviceViewModel;

            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, xDeviceViewModel);

            foreach (var childDevice in xDevice.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, xDeviceViewModel);
                xDeviceViewModel.Children.Add(childDeviceViewModel);
            }

            return xDeviceViewModel;
        }

        public void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        public void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if ((parentDeviceViewModel.Device.Driver.DriverType == XDriverType.GroupController)) //&&
                //(parentDeviceViewModel.Device.Driver.DriverType != XDriverType.AddressController))
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        public override void OnShow()
        {
            var devicesMenuViewModel = new DevicesMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(devicesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}