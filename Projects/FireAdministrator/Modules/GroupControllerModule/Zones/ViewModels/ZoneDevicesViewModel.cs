using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupControllerModule.Models;
using Infrastructure;
using Infrastructure.Common;

namespace GroupControllerModule.ViewModels
{
    public class ZoneDevicesViewModel : BaseViewModel
    {
        ulong _zoneNo;

        public ZoneDevicesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            Devices = new ObservableCollection<DeviceViewModel>();
            AvailableDevices = new ObservableCollection<DeviceViewModel>();
        }

        public void Initialize(ulong zoneNo)
        {
            _zoneNo = zoneNo;

            var devices = new HashSet<XDevice>();
            var availableDevices = new HashSet<XDevice>();

            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                if (device.Zones.Contains(zoneNo))
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
                else
                {
                    device.AllParents.ForEach(x => { availableDevices.Add(x); });
                    availableDevices.Add(device);
                }
            }

            Devices.Clear();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel(device, Devices)
                {
                    IsExpanded = true,
                    IsBold = device.Zones.Contains(zoneNo)
                };
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices.Where(x => x.Device.Parent != null))
            {
                var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                device.Parent = parent;
                parent.Children.Add(device);
            }

            AvailableDevices.Clear();
            foreach (var device in availableDevices)
            {
                var deviceViewModel = new DeviceViewModel(device, AvailableDevices)
                {
                    IsExpanded = true,
                    IsBold = device.Zones.Contains(zoneNo)
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
            return SelectedAvailableDevice != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedAvailableDevice.Device.Zones.Contains(_zoneNo) == false)
                SelectedAvailableDevice.Device.Zones.Add(_zoneNo);
            Initialize(_zoneNo);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.DevicesChanged = true;
        }

        public bool CanRemove()
        {
            return SelectedDevice != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            SelectedDevice.Device.Zones.Remove(_zoneNo);
            Initialize(_zoneNo);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.XDevicesChanged = true;
        }
    }
}