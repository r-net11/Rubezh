using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient.Models;
using System.Collections.ObjectModel;
using FiresecClient;

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

            Devices = new ObservableCollection<DeviceViewModel>();

            Initialize();
        }

        void Initialize()
        {
            HashSet<Device> devices = new HashSet<Device>();

            foreach (var device in FiresecManager.Configuration.Devices)
            {
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
                DeviceViewModel deviceViewModel = new DeviceViewModel();
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
                return true;
            }
            return false;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            Devices.Add(SelectedAvailableDevice);
            Initialize();
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
            Initialize();
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
