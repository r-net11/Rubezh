using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;

namespace FiresecService.Views
{
    public partial class AlarmTest : Window, INotifyPropertyChanged
    {
        public AlarmTest()
        {
            InitializeComponent();
            DataContext = this;

            _devicesUId = new List<string>();
            _devicesUId.Add(_deviceUId);
            _devicesUId.Add(_deviceUId2);
            _devicesUId.Add(_deviceUId3);
            DevicesList = new ObservableCollection<DeviceList>();
            AvailableDevices = new ObservableCollection<DeviceList>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if ((device.Driver.IsZoneDevice) || (device.Driver.IsZoneLogicDevice))
                {
                    if (_devicesUId.Contains(device.UID.ToString()) == false)
                    {
                        AvailableDevices.Add(new DeviceList(device));
                    }
                    else
                    {
                        DevicesList.Add(new DeviceList(device));
                    }
                }
            }

            if (DevicesList.Count > 0)
            {
                SelectedDeviceList = DevicesList[0];
            }
            if (AvailableDevices.Count > 0)
            {
                SelectedAvailableDevice = AvailableDevices[0];
            }
        }

        List<string> _devicesUId;
        string _deviceUId = "2d85b630-5bf2-441b-b5a7-7cc617313515";
        string _deviceUId2 = "cd552c27-bcd7-425d-9fe2-7618456d2e49";
        string _deviceUId3 = "8b375fc9-599f-4d7e-8add-eacbfe83d5d9";
        

        public ObservableCollection<DeviceList> DevicesList { get; set; }
        public ObservableCollection<DeviceList> AvailableDevices { get; set; }

        DeviceList _selectedDeviceList;
        public DeviceList SelectedDeviceList
        {
            get { return _selectedDeviceList; }
            set
            {
                _selectedDeviceList = value;
                OnPropertyChanged("SelectedDeviceList");
            }
        }

        DeviceList _selectedAvailableDevice;
        public DeviceList SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        public StateType StateType
        {
            get { return SelectedDeviceList.State; }
            set
            {
                if (SelectedDeviceList != null)
                {
                    SelectedDeviceList.State = value;
                    OnPropertyChanged("StateType");
                    OnPropertyChanged("SelectedDeviceList");
                    OnPropertyChanged("DevicesList");
                }
            }
        }

        public List<StateType> AvailableStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).OfType<StateType>()); }
        }

        public class DeviceList
        {
            public DeviceList() { }
            public DeviceList(Device device)
            {
                Device = device;
            }

            Device Device { get; set; }

            public string DeviceName
            {
                get { return Device.Driver.ShortName + " - " + Device.PresentationAddress; }
            }

            public string ZoneNo
            {
                get
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
                    if (zone != null)
                    {
                        return (zone.No + "." + zone.Name);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public StateType State
            {
                get
                {
                    var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == Device.UID);
                    if (deviceState != null)
                    {
                        return deviceState.StateType;
                    }
                    else
                    {
                        return StateType.No;
                    }
                }
                set
                {
                    var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == Device.UID);
                    var stateType = deviceState.StateType;
                    var deviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.StateType == stateType);
                    //if (deviceDriverState != null)
                    //{
                    //    deviceDriverState.IsActive = false;
                    //}
                    var newDeviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.StateType == value);
                    //if (newDeviceDriverState != null)
                    //{
                    //    newDeviceDriverState.IsActive = true;
                    //}
                    CallbackManager.OnDeviceStateChanged(deviceState);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAvailableDevice != null)
            {
                DevicesList.Add(SelectedAvailableDevice);
                AvailableDevices.Remove(SelectedAvailableDevice);
                if (AvailableDevices.Count > 0)
                {
                    SelectedAvailableDevice = AvailableDevices[0];
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectedDeviceList != null)
            {
                AvailableDevices.Add(SelectedDeviceList);
                DevicesList.Remove(SelectedDeviceList);
                if (DevicesList.Count > 0)
                {
                    SelectedDeviceList = DevicesList[0];
                }
            }
        }
    }
}

