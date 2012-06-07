using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneLogicDeviceSelectionViewModel : SaveCancelDialogViewModel
    {
        public ZoneLogicDeviceSelectionViewModel(Device parentDevice)
        {
            Title = "Выбор устройства";

            Devices = new List<Device>(parentDevice.Children.Where(x => x.Driver.DriverType == DriverType.AM1_T));
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