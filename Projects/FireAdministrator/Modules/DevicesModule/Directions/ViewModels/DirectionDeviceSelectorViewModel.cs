using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DirectionDeviceSelectorViewModel : SaveCancelDialogContent
    {
        public DirectionDeviceSelectorViewModel(Direction direction, DriverType driverType)
        {
            Title = "Выбор устройства";

            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverType == driverType)
                {
                    if (device.Parent.Children.Any(x => x.Driver.IsZoneDevice && direction.Zones.Contains(x.ZoneNo)))
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices.Where(x => x.Device.Parent != null))
            {
                var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                device.Parent = parent;
                parent.Children.Add(device);
            }

            SelectedDevice = Devices.FirstOrDefault(x => x.HasChildren == false);
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

        protected override bool CanSave()
        {
            if (SelectedDevice != null)
                return SelectedDevice.HasChildren == false;
            return false;
        }
    }
}