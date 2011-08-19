using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GroupDetailsViewModel : DialogContent
    {
        Device _device;

        public GroupDetailsViewModel(Device device)
        {
            Title = "Свойства группы ПДУ";
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            _device = device;

            InitializeDevices();
            InitializeAvailableDevices();
        }

        void InitializeDevices()
        {
            Devices = new ObservableCollection<GroupDeviceViewModel>();

            if (_device.PDUGroupLogic == null)
                return;

            foreach (var groupDevice in _device.PDUGroupLogic.Devices)
            {
                var groupDeviceViewModel = new GroupDeviceViewModel();
                groupDeviceViewModel.Initialize(groupDevice);
                Devices.Add(groupDeviceViewModel);
            }

            SelectedDevice = Devices.FirstOrDefault();
        }

        void InitializeAvailableDevices()
        {
            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (Devices.Any(x => x.Device.Id == device.Id))
                    continue;

                if (
                    (device.Driver.DriverName == "Релейный исполнительный модуль РМ-1")
                    || (device.Driver.DriverName == "Модуль дымоудаления-1.02//3")
                    || (device.Driver.DriverName == "Модуль речевого оповещения")
                    //|| (device.Driver.DriverName == "Модуль пожаротушения")
                    || (device.Driver.DriverName == "Технологическая адресная метка АМ1-Т")
                    )
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
            }

            AvailableDevices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, AvailableDevices);
                deviceViewModel.IsExpanded = true;
                AvailableDevices.Add(deviceViewModel);
            }

            foreach (var device in AvailableDevices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = AvailableDevices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            SelectedAvailableDevice = AvailableDevices.FirstOrDefault(x => x.HasChildren == false);
        }

        ObservableCollection<GroupDeviceViewModel> _devices;
        public ObservableCollection<GroupDeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        GroupDeviceViewModel _selectedDevice;
        public GroupDeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        ObservableCollection<DeviceViewModel> _availableDevices;
        public ObservableCollection<DeviceViewModel> AvailableDevices
        {
            get { return _availableDevices; }
            set
            {
                _availableDevices = value;
                OnPropertyChanged("AvailableDevices");
            }
        }

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

        bool CanAdd(object obj)
        {
            if (SelectedAvailableDevice != null)
            {
                if (SelectedAvailableDevice.HasChildren == false)
                {
                    if ((SelectedAvailableDevice.Device.Driver.DriverName == "Технологическая адресная метка АМ1-Т") &&
                        (Devices.Any(x => x.Device.Driver.DriverName == "Технологическая адресная метка АМ1-Т")))
                        return false;

                    return true;
                }
            }
            return false;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var groupDeviceViewModel = new GroupDeviceViewModel();
            groupDeviceViewModel.Initialize(SelectedAvailableDevice.Device);
            Devices.Add(groupDeviceViewModel);
            InitializeAvailableDevices();
        }

        bool CanRemove(object obj)
        {
            if (SelectedDevice != null)
            {
                return true;
            }
            return false;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Devices.Remove(SelectedDevice);
            InitializeAvailableDevices();
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            _device.PDUGroupLogic = new PDUGroupLogic();

            _device.PDUGroupLogic.AMTPreset = Devices.Any(x => x.Device.Driver.DriverName == "Технологическая адресная метка АМ1-Т");
            foreach (var device in Devices)
            {
                var pDUGroupDevice = new PDUGroupDevice();

                if (device.Device.UID == null)
                {
                    device.Device.UID = Guid.NewGuid().ToString();
                }
                pDUGroupDevice.DeviceUID = device.Device.UID;
                pDUGroupDevice.IsInversion = device.IsInversion;
                pDUGroupDevice.OnDelay = device.OnDelay;
                pDUGroupDevice.OffDelay = device.OffDelay;
                _device.PDUGroupLogic.Devices.Add(pDUGroupDevice);
            }
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}