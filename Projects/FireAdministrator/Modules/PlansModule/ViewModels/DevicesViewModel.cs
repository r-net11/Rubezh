using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class DevicesViewModel : BaseViewModel
    {
        public DevicesViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceAddedEvent>().Subscribe(OnDeviceChanged);
            ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Subscribe(OnDeviceChanged);
            Devices = new ObservableCollection<DeviceViewModel>();

            Update();
        }

        public void Update()
        {
            Devices.Clear();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                var deviceViewModel = new DeviceViewModel(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

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

        void OnDeviceChanged(Guid deviceUID)
        {
            var device = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (device != null)
            {
                device.Update();
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
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }
    }
}
