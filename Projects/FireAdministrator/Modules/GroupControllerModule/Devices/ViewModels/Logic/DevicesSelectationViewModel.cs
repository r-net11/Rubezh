using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DevicesSelectationViewModel : SaveCancelDialogContent
    {
        public DevicesSelectationViewModel(List<Guid> devicesList)
        {
            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
            Title = "Выбор устройства";

            DevicesList = new List<Guid>(devicesList);

            UpdateDevices();
        }

        void UpdateDevices()
        {
            AvailableDevices = new ObservableCollection<XDevice>();
            Devices = new ObservableCollection<XDevice>();
            SelectedAvailableDevice = null;
            SelectedDevice = null;

            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.IsDeviceOnShleif)
                {
                    if (DevicesList.Contains(device.UID))
                        Devices.Add(device);
                    else
                        AvailableDevices.Add(device);
                }
            }

            if (Devices.Count > 0)
                SelectedDevice = Devices[0];

            if (AvailableDevices.Count > 0)
                SelectedAvailableDevice = AvailableDevices[0];

            OnPropertyChanged("Devices");
            OnPropertyChanged("AvailableDevices");
        }

        public List<Guid> DevicesList { get; private set; }
        public ObservableCollection<XDevice> AvailableDevices { get; private set; }
        public ObservableCollection<XDevice> Devices { get; private set; }

        XDevice _selectedAvailableDevice;
        public XDevice SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        XDevice _selectedDevice;
        public XDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public bool CanAddOne()
        {
            return (SelectedAvailableDevice != null);
        }

        public bool CanAddAll()
        {
            return (AvailableDevices.Count > 0);
        }

        public bool CanRemoveOne()
        {
            return (SelectedDevice != null);
        }

        public bool CanRemoveAll()
        {
            return (Devices.Count > 0);
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            DevicesList.Add(SelectedAvailableDevice.UID);
            UpdateDevices();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var deviceViewModel in AvailableDevices)
            {
                DevicesList.Add(deviceViewModel.UID);
            }
            UpdateDevices();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            DevicesList.Remove(SelectedDevice.UID);
            UpdateDevices();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            DevicesList.Clear();
            UpdateDevices();
        }

        protected override void Save(ref bool cancel)
        {
            DevicesList = new List<Guid>();
            foreach (var device in Devices)
            {
                DevicesList.Add(device.UID);
            }
        }
    }
}