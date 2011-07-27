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

            InitializeDevices();
            InitializeAvailableDevices();
        }

        void InitializeDevices()
        {
            //Devices = new ObservableCollection<GroupDeviceViewModel>();

            //var stringGroupProperty = _device.Properties.FirstOrDefault(x => x.Name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
            //if (stringGroupProperty != null)
            //{
            //    var GroupProperty = SerializerHelper.GetGroupProperties(stringGroupProperty.Value);
            //    if ((GroupProperty != null) && (GroupProperty.device != null))
            //    {
            //        foreach (var groupDevice in GroupProperty.device)
            //        {
            //            GroupDeviceViewModel groupDeviceViewModel = new GroupDeviceViewModel();
            //            groupDeviceViewModel.Initialize(groupDevice);
            //            Devices.Add(groupDeviceViewModel);
            //        }
            //    }
            //}

            //SelectedDevice = Devices.FirstOrDefault();
        }

        void InitializeAvailableDevices()
        {
            HashSet<Device> devices = new HashSet<Device>();

            foreach (var device in FiresecManager.Configuration.Devices)
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
            GroupDeviceViewModel groupDeviceViewModel = new GroupDeviceViewModel();
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
            //Firesec.Groups.RCGroupProperties groupProperty = new Firesec.Groups.RCGroupProperties();
            //groupProperty.DevCount = Devices.Count.ToString();
            //groupProperty.AMTPreset = Devices.Any(x => x.Device.Driver.DriverName == "Технологическая адресная метка АМ1-Т") ? "1" : "0";
            //List<Firesec.Groups.RCGroupPropertiesDevice> groupDevices = new List<Firesec.Groups.RCGroupPropertiesDevice>();
            //foreach (var device in Devices)
            //{
            //    Firesec.Groups.RCGroupPropertiesDevice groupDevice = new Firesec.Groups.RCGroupPropertiesDevice();
            //    if (device.Device.UID == null)
            //    {
            //        device.Device.UID = Guid.NewGuid().ToString();
            //    }
            //    groupDevice.UID = device.Device.UID;
            //    groupDevice.Inverse = device.IsInversion ? "1" : "0";
            //    groupDevice.DelayOn = device.OnDelay.ToString();
            //    groupDevice.DelayOff = device.OffDelay.ToString();
            //    groupDevices.Add(groupDevice);
            //}
            //groupProperty.device = groupDevices.ToArray();

            //if (groupDevices.Count > 0)
            //{
            //    string stringGroupProperty = SerializerHelper.SeGroupProperty(groupProperty);
            //    Property property = new Property() { Name = "E98669E4-F602-4E15-8A64-DF9B6203AFC5", Value = stringGroupProperty };
            //    _device.Properties = new List<Property>();
            //    _device.Properties.Add(property);
            //}
            //else
            //{
            //    _device.Properties = null;
            //}

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
