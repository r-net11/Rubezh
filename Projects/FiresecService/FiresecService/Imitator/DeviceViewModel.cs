using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Imitator
{
    public class DeviceViewModel : INotifyPropertyChanged
    {
        public DeviceViewModel(DeviceState deviceState)
        {
            DeviceState = deviceState;

            DriverStates = new List<DeviceStateViewModel>(
                DeviceState.Device.Driver.States.Select(driverState => new DeviceStateViewModel(driverState, ChangeState))
            );

            foreach (var deviceDriverState in deviceState.States)
            {
                var state = DriverStates.FirstOrDefault(x => x.DriverState.Code == deviceDriverState.Code);
                state._isActive = true;
            }
        }

        public DeviceState DeviceState { get; private set; }

        public string Name
        {
            get { return DeviceState.Device.Driver.ShortName + " - " + DeviceState.Device.DottedAddress; }
        }

        public StateType StateType
        {
            get { return DeviceState.StateType; }
        }

        public List<DeviceStateViewModel> DriverStates { get; private set; }

        public void ChangeState()
        {
            var deviceStates = new List<DeviceState>();
            DeviceState.States = new List<DeviceDriverState>(
                DriverStates.Where(state => state.IsActive).
                Select(state => new DeviceDriverState()
                {
                    Code = state.DriverState.Code,
                    Time = DateTime.Now
                })
            );

            CallbackManager.OnDeviceStatesChanged(deviceStates);

            OnPropertyChanged("State");
        }

        public int Level
        {
            get { return DeviceState.Device.PlaceInTree.Split('\\').Count() - 1; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
