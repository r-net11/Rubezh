using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent
    {
        public Device Device { get; private set; }
        public DeviceState DeviceState { get; private set; }
        public DeviceControlViewModel DeviceControlViewModel { get; private set; }
        DeviceControls.DeviceControl _deviceControl;

        public DeviceDetailsViewModel(Guid deviceUID)
        {
            Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == Device.UID);
            if (DeviceState != null)
                DeviceState.StateChanged += new Action(deviceState_StateChanged);
            DeviceControlViewModel = new DeviceControlViewModel(Device);

            Title = Device.Driver.ShortName + " " + Device.DottedAddress;
        }

        public Driver Driver
        {
            get { return Device.Driver; }
        }

        void deviceState_StateChanged()
        {
            if ((DeviceState != null) && (_deviceControl != null))
            {
                _deviceControl.StateType = DeviceState.StateType;
            }

            OnPropertyChanged("DeviceControlContent");
        }

        public object DeviceControlContent
        {
            get
            {
                if (DeviceState != null)
                    _deviceControl = new DeviceControls.DeviceControl()
                    {
                        DriverId = Device.Driver.UID,
                        Width = 50,
                        Height = 50,
                        StateType = DeviceState.StateType
                    };

                return _deviceControl;
            }
        }

        public string ConnectedTo
        {
            get { return Device.ConnectedTo; }
        }

        public string PresentationZone
        {
            get { return Device.GetPersentationZone(); }
        }

        public List<string> Parameters
        {
            get
            {
                var parameters = new List<string>();
                if ((DeviceState != null) && (DeviceState.Parameters != null))
                    foreach (var parameter in DeviceState.Parameters)
                    {
                        if (parameter.Visible)
                        {
                            if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
                                continue;

                            parameters.Add(parameter.Caption + " - " + parameter.Value);
                        }
                    }
                return parameters;
            }
        }

        bool _isValveControlSelected;
        public bool IsValveControlSelected
        {
            get { return _isValveControlSelected; }
            set
            {
                _isValveControlSelected = value;
                OnPropertyChanged("IsValveControlSelected");
            }
        }

        public void StartValveTimer(int timeLeft)
        {
            IsValveControlSelected = true;
            DeviceControlViewModel.StartTimer(timeLeft);
        }
    }
}