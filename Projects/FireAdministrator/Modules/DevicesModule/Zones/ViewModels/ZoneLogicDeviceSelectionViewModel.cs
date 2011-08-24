using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicDeviceSelectionViewModel : DialogContent
    {
        public ZoneLogicDeviceSelectionViewModel(Device parentDevice)
        {
            Title = "Выбор устройства";

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

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

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
