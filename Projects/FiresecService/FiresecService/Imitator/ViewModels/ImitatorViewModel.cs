using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace FiresecService.ViewModels
{
    public class ImitatorViewModel : DialogContent
    {
        public ImitatorViewModel()
        {
            Title = "Имитатор устройств";
            Devices = new ObservableCollection<DeviceViewModel>();

            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                var deviceViewModel = new DeviceViewModel(deviceState);
                Devices.Add(deviceViewModel);
            }
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
    }
}
