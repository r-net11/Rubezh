using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
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
				if (!device.Driver.HasZone)
					continue;

                if (device.ZoneUIDs.Contains(Zone.UID))
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
                else
                {
					if (device.ZoneUIDs.Count == 0)
					{
						device.AllParents.ForEach(x => { availableDevices.Add(x); });
						availableDevices.Add(device);
					}
                }
            }

            Devices.Clear();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel(device, Devices)
                {
                    IsExpanded = true,
                    IsBold = device.ZoneUIDs.Contains(Zone.UID)
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
				if ((device.Driver.DriverType == XDriverType.GKIndicator) ||
					(device.Driver.DriverType == XDriverType.GKLine) ||
					(device.Driver.DriverType == XDriverType.GKRele) ||
					(device.Driver.DriverType == XDriverType.KAUIndicator))
					continue;

                var deviceViewModel = new DeviceViewModel(device, AvailableDevices)
                {
                    IsExpanded = true,
					IsBold = device.Driver.HasZone
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

			SelectedDevice = Devices.LastOrDefault();
			SelectedAvailableDevice = AvailableDevices.LastOrDefault(); ;
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
			return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.Driver.HasZone));
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedAvailableDevice.Device.ZoneUIDs.Contains(Zone.UID) == false)
                SelectedAvailableDevice.Device.ZoneUIDs.Add(Zone.UID);

            if (SelectedAvailableDevice.Device.ZoneUIDs.Contains(Zone.UID) == false)
                SelectedAvailableDevice.Device.ZoneUIDs.Add(Zone.UID);

            Initialize(Zone);
            UpdateAvailableDevices();
			ServiceFactory.SaveService.XDevicesChanged = true;
        }

        public bool CanRemove()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Driver.HasZone));
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            SelectedDevice.Device.ZoneUIDs.Remove(Zone.UID);

            Initialize(Zone);
            UpdateAvailableDevices();
            ServiceFactory.SaveService.XDevicesChanged = true;
        }
    }
}