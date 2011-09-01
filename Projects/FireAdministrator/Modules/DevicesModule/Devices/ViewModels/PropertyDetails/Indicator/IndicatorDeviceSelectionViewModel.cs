using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class IndicatorDeviceSelectionViewModel : SaveCancelDialogContent
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
                if (device.Driver.DriverName == "Выход")
                    continue;

                if ((device.Driver.IsOutDevice) || (device.Driver.IsZoneLogicDevice)
                    || (device.Driver.DriverName == "Технологическая адресная метка АМ1-Т")
                    || (device.Driver.DriverName == "Насос")
                    || (device.Driver.DriverName == "Жокей-насос")
                    || (device.Driver.DriverName == "Компрессор")
                    || (device.Driver.DriverName == "Дренажный насос")
                    || (device.Driver.DriverName == "Насос компенсации утечек")
                    )
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
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
