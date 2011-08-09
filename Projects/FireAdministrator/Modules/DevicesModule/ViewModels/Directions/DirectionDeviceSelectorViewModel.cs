using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DirectionDeviceSelectorViewModel : DialogContent
    {
        public DirectionDeviceSelectorViewModel()
        {
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void Initialize(Direction direction, bool isRm)
        {
            string driverName = isRm ? "Релейный исполнительный модуль РМ-1" : "Кнопка разблокировки автоматики ШУЗ в направлении";

            HashSet<Device> devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                {
                    if (device.Driver.DriverName == driverName)
                    {
                        bool canAdd = device.Parent.Children.Any(x => (x.Driver.IsZoneDevice) && (direction.Zones.Contains(x.ZoneNo)));
                        if (canAdd)
                        {
                            device.AllParents.ForEach(x => { devices.Add(x); });
                            devices.Add(device);
                        }
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            SelectedDevice = Devices.FirstOrDefault(x => x.HasChildren == false);
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

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

        bool CanSave(object obj)
        {
            if (SelectedDevice != null)
            {
                return (SelectedDevice.HasChildren == false);
            }
            return false;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if (SelectedDevice.Device.UID != null)
                SelectedDevice.Device.UID = Guid.NewGuid().ToString();

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}