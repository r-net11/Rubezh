using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneDevicesViewModel : BaseViewModel
    {
        ulong? _zoneNo;

        public ZoneDevicesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
        }

        public void Initialize(ulong? zoneNo)
        {
            _zoneNo = zoneNo;

            var devices = new HashSet<Device>();
            var availableDevices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.IsZoneDevice)
                {
                    if (device.ZoneNo == null)
                    {
                        device.AllParents.ForEach(x => { availableDevices.Add(x); });
                        availableDevices.Add(device);
                    }

                    if (device.ZoneNo == zoneNo)
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }

                if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
                {
                    foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.Zones.Contains(zoneNo)))
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel(device, Devices)
                {
                    IsExpanded = true,
                    IsBold = device.Driver.IsZoneDevice || device.Driver.IsZoneLogicDevice
                };
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices.Where(x => x.Device.Parent != null))
            {
                var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                device.Parent = parent;
                parent.Children.Add(device);
            }

            AvailableDevices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in availableDevices)
            {
                var deviceViewModel = new DeviceViewModel(device, AvailableDevices)
                {
                    IsExpanded = true,
                    IsBold = device.Driver.IsZoneDevice
                };
                AvailableDevices.Add(deviceViewModel);
            }

            foreach (var device in AvailableDevices.Where(x => x.Device.Parent != null))
            {
                var parent = AvailableDevices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                device.Parent = parent;
                parent.Children.Add(device);
            }

            OnPropertyChanged("Devices");

            if (Devices.Count > 0)
                SelectedDevice = Devices[Devices.Count - 1];

            if (AvailableDevices.Count > 0)
                SelectedAvailableDevice = AvailableDevices[AvailableDevices.Count - 1];
        }

        public void Clear()
        {
            Devices.Clear();
            AvailableDevices.Clear();
            SelectedDevice = null;
            SelectedAvailableDevice = null;
        }

        public void UpdateAvailableDevices()
        {
            OnPropertyChanged("AvailableDevices");
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

        public ObservableCollection<DeviceViewModel> AvailableDevices { get; private set; }

        DeviceViewModel _selectedAvailableDevice;
        public DeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        public bool CanAdd()
        {
            return SelectedAvailableDevice != null && SelectedAvailableDevice.Device.Driver.IsZoneDevice;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            SelectedAvailableDevice.Device.ZoneNo = _zoneNo;
            Initialize(_zoneNo);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.DevicesChanged = true;
        }

        public bool CanRemove()
        {
            return SelectedDevice != null && SelectedDevice.Device.Driver.IsZoneDevice;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            SelectedDevice.Device.ZoneNo = null;
            Initialize(_zoneNo);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.DevicesChanged = true;
        }

        public bool CanShowZoneLogic()
        {
            return SelectedDevice != null && SelectedDevice.Device.Driver.IsZoneLogicDevice;
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            var zoneLogicViewModel = new ZoneLogicViewModel(SelectedDevice.Device);

            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel))
            {
                ServiceFactory.SaveService.DevicesChanged = true;
                Initialize(_zoneNo);
            }
        }
    }
}