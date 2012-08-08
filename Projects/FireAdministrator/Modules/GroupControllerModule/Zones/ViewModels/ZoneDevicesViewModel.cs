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
				if (!CanBeInZone(device.Driver))
					continue;

                if (Zone.DeviceUIDs.Contains(device.UID))
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
				if ((device.Driver.DriverType == XDriverType.GKIndicator) ||
					(device.Driver.DriverType == XDriverType.GKLine) ||
					(device.Driver.DriverType == XDriverType.GKRele) ||
					(device.Driver.DriverType == XDriverType.KAUIndicator))
					continue;

                var deviceViewModel = new DeviceViewModel(device, AvailableDevices)
                {
                    IsExpanded = true,
					IsBold = CanBeInZone(device.Driver)
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
			return ((SelectedAvailableDevice != null) && (CanBeInZone(SelectedAvailableDevice.Driver)));
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
            return ((SelectedDevice != null) && (CanBeInZone(SelectedDevice.Driver)));
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

		bool CanBeInZone(XDriver driver)
		{
			return driver.HasZone;
			if (driver.IsDeviceOnShleif == false)
				return false;
			if (driver.AutoChildCount > 0)
				return false;
			return true;
		}
    }
}