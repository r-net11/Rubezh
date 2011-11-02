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
        public DeviceDetailsViewModel(Guid deviceUID)
        {
            _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == _device.UID);
            if (DeviceState != null)
                DeviceState.StateChanged += new Action(deviceState_StateChanged);
            DeviceControlViewModel = new DeviceControlViewModel(_device);

            Title = _device.Driver.ShortName + " " + _device.DottedAddress;
        }

        Device _device;
        public DeviceState DeviceState { get; private set; }
        DeviceControls.DeviceControl _deviceControl;
        public DeviceControlViewModel DeviceControlViewModel { get; private set; }

        public Driver Driver
        {
            get { return _device.Driver; }
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
                        DriverId = _device.Driver.UID,
                        Width = 50,
                        Height = 50,
                        StateType = DeviceState.StateType
                    };

                return _deviceControl;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (_device.Parent != null)
                {
                    return _device.Parent.Driver.Name;
                }
                return null;
            }
        }

        public string PresentationZone
        {
            get { return _device.GetPersentationZone(); }
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
    }
}