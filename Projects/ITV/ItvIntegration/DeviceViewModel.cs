using System;
using System.ComponentModel;
using FiresecAPI.Models;
using FiresecClient;
using System.Collections.Generic;

namespace ItvIntegration
{
    public class DeviceViewModel : INotifyPropertyChanged
    {
        public DeviceViewModel(DeviceState deviceState)
        {
            DeviceState = deviceState;
            _stateType = deviceState.StateType;
            //FiresecClient.FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(FiresecEventSubscriber_DeviceStateChangedEvent);
            deviceState.StateChanged += new Action(deviceState_StateChanged);
        }

        void deviceState_StateChanged()
        {
            StateType = DeviceState.StateType;
        }

        void FiresecEventSubscriber_DeviceStateChangedEvent(Guid uid)
        {
            if (DeviceState.UID == uid)
            {
                StateType = DeviceState.StateType;
            }
        }

        public DeviceState DeviceState { get; private set; }

        public string Name
        {
            get { return DeviceState.Device.Driver.ShortName + " - " + DeviceState.Device.DottedAddress; }
        }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
            }
        }

        public void ExecuteCommand(string commandName)
        {
            if (DeviceState.Device.Driver.IsIgnore)
            {
                var deviceUIDs = new List<Guid>();
                deviceUIDs.Add(DeviceState.UID);

                switch (commandName)
                {
                    case "Dasable":
                        FiresecManager.AddToIgnoreList(deviceUIDs);
                        break;

                    case "Enable":
                        FiresecManager.RemoveFromIgnoreList(deviceUIDs);
                        break;
                }
            }

            if (DeviceState.Device.Driver.DriverType == DriverType.Valve)
            {
                switch (commandName)
                {
                    case "BoltClose":
                    case "BoltStop":
                    case "BoltOpen":
                    case "BoltAutoOn":
                    case "BoltAutoOff":
                        FiresecManager.ExecuteCommand(DeviceState.UID, commandName);
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
