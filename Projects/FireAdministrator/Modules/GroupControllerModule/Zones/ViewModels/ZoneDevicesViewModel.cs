using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupControllerModule.Models;
using Infrastructure;
using Infrastructure.Common;
using XFiresecAPI;
using FiresecClient;

namespace GroupControllerModule.ViewModels
{
    public class ZoneDevicesViewModel : BaseViewModel
    {
        XZone Zone;

        public ZoneDevicesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            Devices = new ObservableCollection<DeviceViewModel>();
            AvailableDevices = new ObservableCollection<DeviceViewModel>();
        }

        public void Initialize(XZone zone)
        {
            Zone = zone;

            var devices = new HashSet<XDevice>();
            var availableDevices = new HashSet<XDevice>();

            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                if (Zone.DeviceUIDs.Contains(device.UID))
                //if (device.Zones.Contains(Zone.No))
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
                    IsBold = Zone.DeviceUIDs.Contains(device.UID)
                    //IsBold = device.Zones.Contains(Zone.No)
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
                    IsBold = device.Driver.IsDeviceOnShleif
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
            return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.Driver.IsDeviceOnShleif));
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (Zone.DeviceUIDs.Contains(SelectedAvailableDevice.Device.UID) == false)
                Zone.DeviceUIDs.Add(SelectedAvailableDevice.Device.UID);

            if (SelectedAvailableDevice.Device.Zones.Contains(Zone.No) == false)
                SelectedAvailableDevice.Device.Zones.Add(Zone.No);

            Initialize(Zone);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.DevicesChanged = true;
        }

        public bool CanRemove()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Driver.IsDeviceOnShleif));
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Zone.DeviceUIDs.Remove(SelectedDevice.Device.UID);
            SelectedDevice.Device.Zones.Remove(Zone.No);

            Initialize(Zone);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.XDevicesChanged = true;
        }
    }
}