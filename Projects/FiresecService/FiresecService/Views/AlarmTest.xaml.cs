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

            _devicesId = new List<string>();
            _devicesId.Add(_deviceId);
            _devicesId.Add(_deviceId2);
            _devicesId.Add(_deviceId3);
            DevicesList = new ObservableCollection<DeviceList>();
            AvailableDevices = new ObservableCollection<DeviceList>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if ((device.Driver.IsZoneDevice) || (device.Driver.IsZoneLogicDevice))
                {
                    if (_devicesId.Contains(device.Id) == false)
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

        List<string> _devicesId;
        string _deviceId = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.16";
        string _deviceId2 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17";
        string _deviceId3 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/799686B6-9CFA-4848-A0E7-B33149AB940C:1.18";
        

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

//public void StateChanged(string state)
//{
//    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == _deviceId);
//    var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == _deviceId);
//    foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
//    {
//        if (Enum.GetName(typeof(StateType), stateType) == state)
//        {
//            var deviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.StateType == _stateType);
//            if (deviceDriverState != null)
//            {
//                deviceDriverState.IsActive = false;
//            }
//            _stateType = stateType;
//            var newDeviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.StateType == _stateType);
//            if (newDeviceDriverState != null)
//            {
//                newDeviceDriverState.IsActive = true;
//            }
//            deviceDriverState = null;
//            newDeviceDriverState = null;
//            CallbackManager.OnDeviceStateChanged(deviceState);
//        }
//    }
//}

//string deviceId2 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17";
//string deviceId3 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/799686B6-9CFA-4848-A0E7-B33149AB940C:1.18";
//var deviceState2 = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId2);
//var deviceState3 = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId3);

//deviceState.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Fire).IsActive = true;
//deviceState2.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Attention).IsActive = true;
//deviceState3.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Failure).IsActive = true;

//CallbackManager.OnDeviceStateChanged(deviceState);
//CallbackManager.OnDeviceStateChanged(deviceState2);
//CallbackManager.OnDeviceStateChanged(deviceState3);