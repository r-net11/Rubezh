using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;

namespace CurrentDeviceModule.ViewModels
{
    public class DeviceTreeViewModel : BaseViewModel
    {
        public DeviceTreeViewModel()
        {
            OkCommand = new RelayCommand(OnOk);
            CanselCommand = new RelayCommand(OnCansel);
        }

        public void Initialize()
        {
            BuildDeviceTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

        Guid _deviceId;
        public Guid DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        ObservableCollection<ElementTreeViewModel> _devices;
        public ObservableCollection<ElementTreeViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        ElementTreeViewModel _selectedDevice;
        public ElementTreeViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        void BuildDeviceTree()
        {
            Devices = new ObservableCollection<ElementTreeViewModel>();
            var device = FiresecManager.DeviceConfiguration.RootDevice;
            AddDevice(device, null);
        }

        ElementTreeViewModel AddDevice(Device device, ElementTreeViewModel parentElementTreeViewModel)
        {
            ElementTreeViewModel elementTreeViewModel = new ElementTreeViewModel();
            elementTreeViewModel.Parent = parentElementTreeViewModel;
            elementTreeViewModel.Initialize(device, Devices);

            var indexOf = Devices.IndexOf(parentElementTreeViewModel);
            Devices.Insert(indexOf + 1, elementTreeViewModel);

            foreach (var childDevice in device.Children)
            {
                var childElementTreeViewModel = AddDevice(childDevice, elementTreeViewModel);
                elementTreeViewModel.Children.Add(childElementTreeViewModel);
            }

            return elementTreeViewModel;
        }

        public void CollapseChild(ElementTreeViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        public void ExpandChild(ElementTreeViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            DeviceId = SelectedDevice.DeviceId;
            OnClosing();
        }

        public RelayCommand CanselCommand { get; private set; }
        void OnCansel()
        {
            DeviceId = Guid.Empty;
            OnClosing();
        }

        public event Action Closing;
        void OnClosing()
        {
            if (Closing != null)
                Closing();
        }
    }
}
