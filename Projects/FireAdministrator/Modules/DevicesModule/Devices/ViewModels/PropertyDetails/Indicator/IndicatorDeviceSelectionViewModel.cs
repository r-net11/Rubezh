using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class IndicatorDeviceSelectionViewModel : SaveCancelDialogViewModel
    {
        public IndicatorDeviceSelectionViewModel()
        {
            Title = "Выбор устройства";
            InitializeDevices();
        }

        void InitializeDevices()
        {
            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverType == DriverType.Exit)
                    continue;

                if ((device.Driver.IsOutDevice) ||
                    (device.Driver.IsZoneLogicDevice) ||
                    (device.Driver.DriverType == DriverType.AM1_T) ||
                    (device.Driver.DriverType == DriverType.Pump) ||
                    (device.Driver.DriverType == DriverType.JokeyPump) ||
                    (device.Driver.DriverType == DriverType.Compressor) ||
                    (device.Driver.DriverType == DriverType.DrenazhPump) ||
                    (device.Driver.DriverType == DriverType.CompensationPump)
                )
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
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
        }

        public ObservableCollection<DeviceViewModel> Devices { get; set; }

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
    }
}