using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicDeviceSelectionViewModel : SaveCancelDialogContent
    {
        public ZoneLogicDeviceSelectionViewModel(Device parentDevice)
        {
            Title = "Выбор устройства";

            Devices = new List<Device>();
            foreach (var device in parentDevice.Children)
            {
                if (device.Driver.DriverName == "Технологическая адресная метка АМ1-Т")
                    Devices.Add(device);
            }
        }

        public List<Device> Devices { get; private set; }

        Device _selectedDevice;
        public Device SelectedDevice
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
